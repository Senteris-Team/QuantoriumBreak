using System;
using System.Collections;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SimpleShoot : MonoBehaviour
{
    public SteamVR_Action_Boolean fireAction;
    public SteamVR_Action_Boolean removeTheMagazineAction;

    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform barrelLocation;
    public Transform casingExitLocation;

    [SerializeField]
    private GameObject magazineReceiver;

    private Transform magazine;
    private Interactable interactable;

    public float shotPower = 100f;
    public float bulletLifeTime = 10f;
    private short bulletsInTheMagazine;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;
        interactable = GetComponent<Interactable>();
        foreach (Transform child in transform)
        {
            if (child.tag == "Magazine")
            {
                magazine = child;
                bulletsInTheMagazine = magazine.GetComponent<MagazinStorage>().bulletAmmout;
                break;
            }
        }
    }

    void Update()
    {
        if (interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            if (fireAction[source].stateDown) Shoot();
            if (removeTheMagazineAction[source].stateDown) RemoveTheMagazine();
        }
    }

    void Shoot()
    {
        if (CoolDataBase.shots == 0) CoolDataBase.startTime = DateTime.Now;
        if (bulletsInTheMagazine != 0)
        {
            GameObject bullet;
            GameObject tempFlash;

            bulletsInTheMagazine--;
            CoolDataBase.shots++;

            bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            Destroy(bullet, bulletLifeTime);
            Destroy(tempFlash, 0.5f);
            //  Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation).GetComponent<Rigidbody>().AddForce(casingExitLocation.right * 100f);
        }
    }

    void CasingRelease()
    {
        GameObject casing;
        casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        casing.GetComponent<Rigidbody>().AddExplosionForce(550f, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, UnityEngine.Random.Range(100f, 500f), UnityEngine.Random.Range(10f, 1000f)), ForceMode.Impulse);
    }

    void RemoveTheMagazine()
    {
        //GameObject.FindGameObjectsWithTag?     
        
        Rigidbody rigidbody = magazine.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        magazine.GetComponent<MagazinStorage>().bulletAmmout = bulletsInTheMagazine;
        bulletsInTheMagazine = 0;

        magazine.GetComponent<Interactable>().enabled = true;
        StartCoroutine(TurnOnCollisionMagazine());
    }
    IEnumerator TurnOnCollisionMagazine()
    {
        magazine.transform.Translate(0, -0.15f, 0);
        yield return new WaitForSeconds(0.15f);
        magazine.GetComponent<MeshCollider>().isTrigger = false;
        magazine.transform.parent = null;
        magazine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.tag == "Magazine" && magazine == null) InsertTheMagazine(other);
    }

    void InsertTheMagazine(Collider magazineCollider)
    {
        magazine = magazineCollider.transform;

        MeshCollider mcm = magazine.GetComponent<MeshCollider>();
        Rigidbody rigidbody = magazine.GetComponent<Rigidbody>();
        magazine.GetComponent<Interactable>().enabled = false;
        if (mcm.isTrigger == true) return; // Если он уже успел выпасть, то код продолжается

        mcm.isTrigger = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        magazine.transform.parent = this.gameObject.transform;
        magazine.transform.localPosition = new Vector3(-0.0005000038f, 0.055f, 0.033f);
        magazine.transform.localRotation = Quaternion.Euler(0, 90, 11);

        bulletsInTheMagazine = magazine.GetComponent<MagazinStorage>().bulletAmmout;
    }
}
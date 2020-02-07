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

    private Interactable interactable;

    public float shotPower = 100f;
    public float bulletLifeTime = 10f;
    public short maxBulletsInTheStore = 7;
    private short bulletsInTheStore;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;
        interactable = GetComponent<Interactable>();
        bulletsInTheStore = maxBulletsInTheStore;
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
        if (bulletsInTheStore != 0)
        {
            GameObject bullet;
            GameObject tempFlash;

            bulletsInTheStore--;
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
        foreach (Transform child in transform)
        {
            if (child.tag == "Magazine")
            {
                Rigidbody rigidbody = child.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
                StartCoroutine(TurnOnCollisionMagazine(child));
                break;
            }
        }
    }
    IEnumerator TurnOnCollisionMagazine(Transform child)
    {
        yield return new WaitForSeconds(0.1f);
        child.GetComponent<MeshCollider>().isTrigger = false;
        child.transform.parent = null;
    }

    void InsertTheMagazine()
    {
        bulletsInTheStore = maxBulletsInTheStore;
    }
}
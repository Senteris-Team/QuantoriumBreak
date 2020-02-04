using System;
using UnityEngine;
using UnityEngine.UI;

public class HitsTextUpdater : MonoBehaviour
{
    public Text myText;
    int KPD;
    TimeSpan deltaDateTime;

    void Start()
    {
        CoolDataBase.startTime = DateTime.MinValue;
    }

    void Update()
    {
        if (CoolDataBase.shots == 0) deltaDateTime = TimeSpan.Zero;
        else deltaDateTime = DateTime.Now - CoolDataBase.startTime;
        deltaDateTime = TimeSpan.FromSeconds(Math.Round(deltaDateTime.TotalSeconds, 1));
        if (CoolDataBase.hits == 0) KPD = 0;
        else KPD = CoolDataBase.hits * 100 / CoolDataBase.shots;
        myText.text = "Попаданий: " + CoolDataBase.hits +
                    "\nВыстрелов: " + CoolDataBase.shots +
                    "\n" + KPD.ToString() + "%" +
                    "\nВремени прошло: " + deltaDateTime.TotalSeconds+"c";
    }
}

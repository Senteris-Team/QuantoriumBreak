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
        if (CoolDataBase.startTime == DateTime.MinValue) deltaDateTime = TimeSpan.MinValue;
        else deltaDateTime = DateTime.Now - CoolDataBase.startTime;
        if (CoolDataBase.hits == 0) KPD = 0;
        else KPD = CoolDataBase.hits * 100 / CoolDataBase.shots;
        myText.text = "Попаданий: " + CoolDataBase.hits +
                    "\nВыстрелов: " + CoolDataBase.shots +
                    "\n" + KPD.ToString() + "%" +
                    "\n Времени прошло: " + deltaDateTime.TotalSeconds;
    }
}

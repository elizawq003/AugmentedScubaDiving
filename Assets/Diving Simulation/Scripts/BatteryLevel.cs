using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BatteryLevel : MonoBehaviour
{
    private Slider slider;
    public InformationManager iM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        SetMaxBatteryLevel(iM.GetMaxBatteryLevel());

        
    }

    public void SetMaxBatteryLevel (float maxLevel){
        slider.maxValue = maxLevel;
        slider.value = maxLevel; // Start with max!

    }

    public void SetBatteryLevel (float level){
        slider.value = level;
    }



    // Update is called once per frame
    void Update()
    {
        SetBatteryLevel(iM.GetBatteryLevel());
        
    }
}

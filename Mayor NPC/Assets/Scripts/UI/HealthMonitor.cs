using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthMonitor : MonoBehaviour
{
    //get the Combatant on this object
    [SerializeField]private Combatant combatant;
    //slider
    [SerializeField]private Slider slider;

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = combatant.GetMaxHealth();
        slider.value = combatant.healthRemaining;
    }
}

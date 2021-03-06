﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class TimeUI : MonoBehaviour
{
    //Reference to the timer text
    private TextMeshProUGUI timeText;


    // Start is called before the first frame update
    void Start()
    {
        timeText = GetComponentInChildren<TextMeshProUGUI>();
        //Add the time updater to the New Second Event
        WorldTime.NewSecondEvent += TimeUpdater;
    }

    private void TimeUpdater()
    {
        timeText.text = WorldTime.GetWorldTime().GetTime();
    }
}

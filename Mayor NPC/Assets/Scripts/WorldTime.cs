﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    //Static instance of this WorldTimer
    private static WorldTime instance;
    //WorldTime value
    TimeSpan time = TimeSpan.FromMinutes(0);
    //4am in minutes since midnight
    private int startTime = 240;

    public static WorldTime GetWorldTime()
    {
        if(instance == null)
        {
            //see if there is an instance in the game
            if(FindObjectOfType<WorldTime>() == null)
            {
                //add one to the Game Manager
                instance  = FindObjectOfType<GameManager>().gameObject.AddComponent<WorldTime>();

            }
        }
        return instance;
    }

    //Event Handlers
    public delegate void NewDay();
    public static event NewDay NewDayEvent;

    public delegate void NewMinute();
    /// <summary>
    /// Called once for every world time update
    /// </summary>
    public static event NewMinute NewSecondEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log(instance.gameObject.name + " Already has a WorldTime component " + gameObject.name + "is trying to duplicate");
            Destroy(this);
        }
        StartCoroutine(GameTime());
    }


    //10 minutes of gameplay
    IEnumerator GameTime()
    {
        //4am - 12am
        //each minute is 2 hours
        // each 30 seconds is 1 hour
        // each second is 2 minutes
        int minutes = startTime;

        while (true)
        {
            yield return new WaitForSeconds(.5f);
            minutes++;
            time = TimeSpan.FromMinutes(minutes);
        }
    }


}

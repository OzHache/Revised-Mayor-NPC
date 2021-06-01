using System;
using System.Collections;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    //Static instance of this WorldTimer
    private static WorldTime instance;
    //WorldTime value
    private TimeSpan time = TimeSpan.FromMinutes(0);
    public string GetTime() { return time.ToString(@"hh\:mm"); }
    //4am in minutes since midnight
    private readonly int startTime = 240;
    // Pause Time
    private bool pauseTime = false;
    private bool isNewDay = false;

    public void PauseDay()
    {
        pauseTime = !pauseTime;
    }
    private void Start()
    {
        GameManager.NewDayEvent += DayReset;
        GameManager.StartDay += PauseDay;
    }

    public static WorldTime GetWorldTime()
    {
        if (instance == null)
        {
            //see if there is an instance in the game
            if (FindObjectOfType<WorldTime>() == null)
            {
                //add one to the Game Manager
                instance = FindObjectOfType<GameManager>().gameObject.AddComponent<WorldTime>();

            }
        }
        return instance;
    }

    //Event Handlers
    public delegate void NewDay();
    public static event NewDay NewDayEvent;

    public delegate void NewSecond();
    /// <summary>
    /// Called once for every world time update
    /// </summary>
    public static event NewSecond NewSecondEvent;

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
    private void DayReset()
    {
        PauseDay();
        isNewDay = true;
        //wait forStartNewDay

    }


    //10 minutes of gameplay
    IEnumerator GameTime()
    {
        // 4am - 12am
        // each minute is 2 hours
        // each 30 seconds is 1 hour
        // each second is 2 minutes
        int minutes = startTime;

        while (true)
        {
            while (pauseTime)
            {
                if (isNewDay && minutes != startTime)
                {
                    minutes = startTime;
                }
                yield return null;

            }
            yield return new WaitForSeconds(.5f);
            minutes++;
            time = TimeSpan.FromMinutes(minutes);
            if (NewSecondEvent != null)
            {
                NewSecondEvent();
            }
        }
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;

public class UIManager : MonoBehaviour
{
    //Static reference
    private static UIManager instance;

    //References to each of the UIs
    [SerializeField] private UIController playerInventory;

    //Dictionary for relating characters to UIControlelrs

    Dictionary<char, UIController> UIControllerDict = new Dictionary<char, UIController>();
    //list of valid user key inputs
    private List<char> validUserInput = new List<char>();

    //Currently Active UI
    UIController ActiveUI;
    //returns true if this singleton has an active UI
    public static bool isUIActive { get { return instance.ActiveUI != null; } }

    public delegate void UIActivated();
    public static event UIActivated UIHasActivated;

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }
        BuildDictionary();

    }

    /// <summary>
    /// Build the UIController Dictionary
    /// </summary>
    private void BuildDictionary()
    {
        //build the dictionary
        if (playerInventory != null)
        {
            UIControllerDict.Add('e', playerInventory);
        }

        BuildReferenceArray();
    }
    /// <summary>
    /// Build an array of valid chars in the dictionary
    /// </summary>
    private void BuildReferenceArray()
    {
        foreach(KeyValuePair<char, UIController> entry in UIControllerDict)
        {
            validUserInput.Add(entry.Key);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        bool checkvalue = (ActiveUI != null);
        string inputThisFrame = Input.inputString;
        if (inputThisFrame.Length == 0)
            return;
        //seperate the first value this frame
        char input = inputThisFrame[0];

        //see if this is a valid input
        if (validUserInput.Contains(input))
        {
            if (ActiveUI == UIControllerDict[input] || ActiveUI == null)
            {
                //Activate or deactivate
                bool isUIActive = UIControllerDict[input].Activate();
                if (isUIActive)
                {
                    ActiveUI = UIControllerDict[input];
                }
                else
                {
                    ActiveUI = null;
                }
            }

        }

        //see if the checkvalue has been changed
        if(checkvalue != (ActiveUI != null))
        {
            if (UIHasActivated != null)
            {
                UIHasActivated();
            }
        }
    }
}

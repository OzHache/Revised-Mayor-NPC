using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shroom : Collectable
{
    //Identify the object
    public override Dictionary<UIButtonValues, string> Identify()
    {
        return interactionDescriptions;
    }

    protected override void Activate(string message)
    {
        InteractionTypes action = InteractionTypes.Unused;
        if(System.Enum.IsDefined(typeof(InteractionTypes), message))
        {
            action = (InteractionTypes)System.Enum.Parse(typeof(InteractionTypes), message);
        }
        


        switch (action)
        {
            case InteractionTypes.Take:
                Debug.Log("Take");
                //Send a message to the Game Manager to take the object

                break;
            case InteractionTypes.Use:
                Debug.Log("Use");
                break;
            case InteractionTypes.Misc:
                Debug.Log("Misc");

                break;
        }
    }

    protected override void FillInteractionDescription()
    {
        

        var i = 0;
        foreach (InteractionTypes iType in interactions)
        {
            switch (i)
            {
                case 0:
                    //fill the first action
                    interactionDescriptions.Add(UIButtonValues.Action_1, iType.ToString());
                    break;
                case 1:
                    //Fill the second action
                    interactionDescriptions.Add(UIButtonValues.Action_2, iType.ToString());
                    break;
                case 2:
                    //fill the third action
                    interactionDescriptions.Add(UIButtonValues.Action_3, iType.ToString());

                    break;
                default:
                    Debug.Log(i + " There is an extra itype beyond the allowed 3");
                    break;
            }
            i++;
        }
    }

    protected override void Setup()
    {
        //Make sure there is a description
        if (descriptionOfObject == null)
        {
            descriptionOfObject = "???";
        }
        interactionDescriptions.Add(UIButtonValues.Description, descriptionOfObject);
        FillInteractionDescription();
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

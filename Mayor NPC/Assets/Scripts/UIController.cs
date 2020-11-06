using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Canvas))]
public class UIController : MonoBehaviour
{
    private Canvas myCanvas;
    // Start is called before the first frame update
    void Start()
    {
        myCanvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal bool Activate()
    {
        //toggle enabled
        myCanvas.enabled = (!myCanvas.enabled);
        //return status
        return myCanvas.enabled;
    }
}

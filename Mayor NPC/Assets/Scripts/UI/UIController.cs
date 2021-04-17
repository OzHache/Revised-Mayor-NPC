using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Canvas))]
public class UIController : MonoBehaviour
{
    [SerializeField] List<GameObject> m_activations;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal bool Activate()
    {
        var isActive = false;
        if (m_activations != null)
        {
            isActive = m_activations[0].activeSelf;
            foreach (var item in m_activations)
            {
                item.SetActive(!item.activeSelf);
            }
        }
        
        //return status
        return isActive;
    }
}

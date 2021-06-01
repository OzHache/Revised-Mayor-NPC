using System.Collections.Generic;
using UnityEngine;
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
        bool isActive = false;
        if (m_activations != null)
        {
            isActive = m_activations[0].activeSelf;
            foreach (GameObject item in m_activations)
            {
                item.SetActive(!item.activeSelf);
            }
        }

        //return status
        return isActive;
    }
}

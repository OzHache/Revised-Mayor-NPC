using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    [SerializeField] private List<Villager> m_villagers;
    private static VillagerManager s_instance;
    // Start is called before the first frame update
    void Start()
    {
        s_instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Activate(string name)
    {
        foreach(var villager in s_instance.m_villagers){
            if(villager.name == name)
            {
                villager.Activate();
            }
        }
    }
}

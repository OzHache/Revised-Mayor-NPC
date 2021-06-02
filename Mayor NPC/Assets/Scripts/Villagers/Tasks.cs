using System.Collections;
using UnityEngine;

public interface ITasks
{
    //take in the resource we asked for an process the request

    //exampels:
    //Food - consume 
    //Weath - confirm that wealth is at the appropriate level
    //House level - that I have a house of this level required
    //Walls to build walls at the designated locations
    //Work - work during work hourse
    //Lounge - lounge during socializing hours


    void Satisfy(Resource.ResourceType resource, int amount);
}

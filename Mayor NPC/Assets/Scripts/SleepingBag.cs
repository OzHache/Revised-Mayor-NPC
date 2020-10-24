using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingBag : MonoBehaviour
{
    /// <summary>
    /// Sleep tell the GameManager that sleep is happening in a safe Space
    /// </summary>
    // Start is called before the first frame update
   public void Sleep()
    {
        GameManager.GetGameManager().PlayerSleep(isSafe: true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenEvents : MonoBehaviour
{
    /// <summary>
    /// Fade to dark screen, Activate the dialogue choices,
    /// Respawn character if they choose to keep going
    /// Give them a chance to quit
    /// </summary>
    


    [SerializeField] private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
    }

    internal void Activate()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Start");
    }
    public void Deactivate()
    {
        GameManager.GetGameManager().PauseAction(false);
        gameObject.SetActive(false);

    }

    public void DeactivateCamera()
    {
        Camera.main.GetComponent<CameraFollow>().SetActive(false);
    }
    public void ActivateCamera()
    {
        Camera.main.GetComponent<CameraFollow>().SetActive(false);
    }
    public void StopGame()
    {
        Application.Quit();
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Combatant))]
public class EnemyBehaviour : MonoBehaviour
{
    //Reference to the player game object
    [SerializeField]GameObject player;
    // Enemy vision scalars
    [SerializeField] private float visionRange = 5f;
    public bool canSeePlayer = false;
    private Vector3 playerLastPosition;
    [SerializeField] private float newLookInterval = 2f;
    [SerializeField] private float activeLookInterval = .5f;
    //Enemy movement scalar
    [SerializeField] private float speed;

    private Combatant combatant;
    //Track if is attacking for the Coroutine
    private bool isAttacking = false;

    //Loot table
    [SerializeField] private InventoryItem m_loot;

    // Start is called before the first frame update
    void Start()
    {
        //Set references
        player = GameManager.GetGameManager().Player;
        combatant = GetComponent<Combatant>();
        StartCoroutine(LookForPlayer());

        
        combatant.SetOnDeath(()=>
        {
            //Set up the on Death action on the combatant
            PlayerActions action;
            action.m_action = Quest.ActionType.Kill;
            action.m_keyWord = "Enemies";
            action.m_number = 1;
            QuestManager.GetQuestManager().UpdateQuests(action);
            
            Dropper.GetDropper().Drop(m_loot);
        });
    }

    // Update is called once per frame
    void Update()
    {

        if (canSeePlayer && !isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    //Couroutine to attack the player
    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        while (canSeePlayer)
        {
            if (GameManager.GetGameManager().isGamePaused)
                yield return null;
            //if we are too far to attack, move closer
            if(Vector3.Distance(transform.position, player.transform.position) > combatant.meleeRange)
            {
                MoveToPlayer();
            }
            else
            {
                combatant.Melee(player.GetComponent<Combatant>());
            }
            yield return null;
            
        }
        isAttacking = false;
    }
    
    //Move to the players last known position
    private void MoveToPlayer()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, playerLastPosition, speed * Time.deltaTime);
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y) * -1 + 50;
    }

    //Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .1f);
        Gizmos.DrawSphere(transform.position, visionRange);
        if (canSeePlayer)
        {
            Gizmos.color = Color.yellow;
            //Gizmos.DrawRay(new Ray(transform.position, player.transform.position - transform.position));
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }

    IEnumerator LookForPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(canSeePlayer?activeLookInterval:newLookInterval);

            //direction of the player
            Ray2D ray2D = new Ray2D(transform.position, player.transform.position - transform.position);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray2D.origin, ray2D.direction, visionRange);
            if (hits.Length > 0)

            {
                if (hits[0])
                {
                    //If this is the player then I can see the player
                    if (hits[0].transform == player.transform)
                    {
                        canSeePlayer = true;
                        playerLastPosition = hits[0].transform.position;
                        continue;
                    }
                }

            }
            canSeePlayer = false;
            continue;
        }
    }
    /// <summary>
    /// reset all applicable traits to what they are when first spawned
    /// </summary>
    internal void Restart()
    {
        canSeePlayer = false;   
    }

    /// Basic behaviour: See if player is in visible range, 

}

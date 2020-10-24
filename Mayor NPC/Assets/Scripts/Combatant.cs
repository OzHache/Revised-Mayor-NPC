using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// SuperCombat script for any class character that can engage in combat
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Weapon))]
public class Combatant : MonoBehaviour
{
    

    //Default to is not the player
    [SerializeField] private bool isPlayer = false;
    //Class scalars
    [SerializeField] private int health = 10;
    private int healthRemaining;
    //MeleeRange
    [SerializeField] internal float meleeRange = 1f;
    //Probably dependant on weapons but for now this is a simple 50/50
    [SerializeField] private float chanceToCrit = 0.01f;
    [SerializeField] private WeaponItem weaponItem;
    private bool isWeaponReady = true;

    //manage defence

    private void Start()
    {
        RestartCharacter();
    }

    //all classes can melee
    virtual public void Melee(Combatant opposition)
    {
        //The weapon is still cooling down
        if (!isWeaponReady)
            return;
        bool didMiss = true;
        var hit = Random.Range(0f, 1f);
        //if there is no weapon assigned, use basic melee
        if (weaponItem == null)
        {
            if (hit > .4)
            {
                opposition.TakeDamage(1);
                didMiss = false;
                
            }
            //Missed
            Debug.Log("Miss");
        }
        //see if we hit
        else if(hit <= weaponItem.hitChance)
        {
            //See if it is a crit
            if(hit > 1 - chanceToCrit)
            {
                opposition.TakeDamage(rawDamage: weaponItem.maxDamage * weaponItem.critMultiplyer);
            }
            else
            {
                opposition.TakeDamage(rawDamage: weaponItem.hitDamage);
            }
            Debug.Log(gameObject.name + " Hit " + opposition.gameObject.name);
            didMiss = false;
        }
        
        else
            Debug.Log("Miss");
        StartCoroutine(WeaponCoolDown(didMiss));
    }

    private void TakeDamage(int rawDamage)
    {
        //manage defense;
        //manage skills like dodge
        healthRemaining -= rawDamage;
        Debug.Log(gameObject.name +" "+ healthRemaining);
        if (!gameObject.CompareTag("Player"))
        {
            var i = 1;
        }
        if(healthRemaining <= 0)
        {
            this.enabled = false;
            RestartCharacter();
            OnMouseExit();
            gameObject.SetActive(false);
        }
    }

    //register that the mosue has been set on this object
    private void OnMouseEnter()
    {
        MouseUI.GetMouseUI().MouseOver();
    }
    private void OnMouseExit()
    {
        MouseUI.GetMouseUI().MouseExit();
    }
    
    IEnumerator WeaponCoolDown(bool didMiss)
    {
        isWeaponReady = false;
        float coolDown;
        //Cool down is half on a miss
        if (weaponItem == null)
        {
            coolDown = 1f;
        }
        else
        {
            coolDown = didMiss ? weaponItem.coolDown / 2 : weaponItem.coolDown;
        }
        while (coolDown > 0f)
        {
            yield return null;
            coolDown -= Time.deltaTime;
            
        }
        isWeaponReady = true;
    }
    //Any actions needed to reset the character back to normal
    public void RestartCharacter()
    {
        healthRemaining = health;
        if (GetComponent<EnemyBehaviour>()!= null)
        {
            GetComponent<EnemyBehaviour>().Restart();
        }
    }
}

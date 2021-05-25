using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

//Quest actions are a collection of quests and triggered actions used on each CutScene
using QuestsActions = 
    //Quests
    System.Tuple<System.Collections.Generic.List<Quest>, 
    //Actions at the begining of the Quest
    System.Collections.Generic.List<System.Action>,  
    //Actions triggered at the end of the Quest
    System.Collections.Generic.List<System.Action>>;
//repository for creating cutscene missions. Eventually this will need to be seriealized
public class GameLoop
{

    List<QuestsActions> m_cutScenes = new List<QuestsActions>();
    public void Initialize()
    {
        //create all the needed events
        CutSceneOne();
        CutSceneTwo();
        CutSceneThree();

    }

    private void CutSceneThree()
    {
        ///Collect the Key
        Quest collectKey = new Quest(Quest.ActionType.Collect, "Key", 1, null);
        //Open and unlock the Gate
        Quest openGate = new Quest(Quest.ActionType.Unlock, "Gate", 1, null);

        QuestsActions questActions = new QuestsActions(
            //this Quest
            new List<Quest> { collectKey, openGate },
            //Start CutScene go to the beacon
            new List<Action> 
            { () =>
                {
                    // set the Key to be able to be interacted with
                    var key = GameObject.Find("Key1").GetComponent<KeyObject>();
                    Assert.IsNotNull(key);
                    if (key != null)
                    {
                        key.m_isInteractable = true;
                    }
                }
            },
            //End of Cut Scene
            //See new NPC Arrive off the coast
            new List<Action> {
                //Activate the first villager
                ()=>GameObject.Find("WorldDialogue").GetComponent<CharacterDialogue>().SetDialogueID("CutScene4"),
                //Pan to the new villager
                () => Camera.main.gameObject.GetComponent<CameraFollow>().MoveToTarget(GameObject.Find("VillageCenter"), 5.0f)
            });
        m_cutScenes.Add(questActions);

    }

    private void CutSceneTwo()
    {
        ///Kill 3 enemies
        Quest enemiesQuest = new Quest(Quest.ActionType.Kill, "Enemies", 3, ()=>
        {
            // set the Beacon to be able to be interacted with
            var beacon = GameObject.Find("Beacon").GetComponent<Beacon>();
            Assert.IsNotNull(beacon);
            if(beacon != null) 
            {
                beacon.m_isInteractable = true;
            }
            
        });
        Quest recoverBeacon = new Quest(Quest.ActionType.Use, "Beacon", 1, null);
        QuestsActions questActions = new QuestsActions(
            //this Quest
            new List<Quest> { enemiesQuest, recoverBeacon },
            //Start CutScene go to the beacon
            new List<Action> { () => Camera.main.gameObject.GetComponent<CameraFollow>().MoveToTarget(GameObject.Find("Beacon"), 3.0f),
            ()=>EnemySpawner.GetSpawner().SpawnWave(size: 3)
            },
            //End of Cut Scene
            //See new NPC Arrive off the coast
            new List<Action> {
                //Set up the Next Dialogue
                ()=>GameObject.Find("WorldDialogue").GetComponent<CharacterDialogue>().SetDialogueID("CutScene3"),
                //Activate the first villager
                () => VillagerManager.Activate("Villager1"),
                //Pan to the new villager
                () => Camera.main.gameObject.GetComponent<CameraFollow>().MoveToTarget(GameObject.Find("Villager1"), 3.0f),
                //Start up Scene 2
                ()=>GameObject.Find("OldLady").GetComponent<OldLady>().StartScene(2)
            });
        m_cutScenes.Add(questActions);
    }

    private void CutSceneOne()
    {

        //Player will need to build a house
        Quest quest = new Quest(Quest.ActionType.Build, "Building", 1, () => EndQuest(0));
        QuestsActions questsActions = new QuestsActions(
        //This Quest
        new List<Quest>() { quest },
        //Start Of CutScene
        //Go to the Building and then come back
        new List<Action>() {
            () => Camera.main.gameObject.GetComponent<CameraFollow>().MoveToTarget(GameObject.Find("Building"), 3.0f),
            ()=>GameManager.GetGameManager().PauseAction(false)
        },
        //End of CutScene
        //Start Tracking the OldLady
         new List<Action>() {
             ()=>GameObject.Find("WorldDialogue").GetComponent<CharacterDialogue>().SetDialogueID("CutScene2"),
             () => Camera.main.gameObject.GetComponent<CameraFollow>().ChangeTarget(GameObject.Find("OldLady")),
             MoveTo("OldLady",GameManager.GetGameManager().Player),
         //Pause the player and any enemies
            ()=>GameObject.Find("OldLady").GetComponent<OldLady>().StartScene(1)
         }); 
        m_cutScenes.Add(questsActions);
    }

    private Action MoveTo(string ActorName, GameObject position)
    {
        //Debug.Log("Moving");
        return ()=> GameObject.Find(ActorName).GetComponent<Villager>().Move(position);
    }

    public List<Quest> GetQuest(int sceneNumber)
    {
        if (m_cutScenes.Count > sceneNumber)
        {
            return m_cutScenes[sceneNumber].Item1;
        }
        return null;
    }
    //Called when we start a cutscene quest
    public void StartQuest(int sceneNumber)
    {
        if (m_cutScenes.Count > sceneNumber)
        {
            if(m_cutScenes[sceneNumber].Item2.Count > 0)
            {
                foreach(var action in m_cutScenes[sceneNumber].Item2)
                {
                    if(action != null)
                    action.Invoke();
                }
            }
        }
    }
    //called when we complete a cutscene quest
    public void EndQuest(int sceneNumber)
    {
        if (m_cutScenes.Count > sceneNumber)
        {
            if (m_cutScenes[sceneNumber].Item3.Count > 0)
            {
                foreach (var action in m_cutScenes[sceneNumber].Item3)
                {
                    if (action != null)
                        action.Invoke();
                }
            }
        }
    }


}

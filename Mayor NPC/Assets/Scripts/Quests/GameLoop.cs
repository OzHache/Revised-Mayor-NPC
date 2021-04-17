using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

    }

    private void CutSceneOne()
    {

        //Player will need to build a house
        Quest quest = new Quest(Quest.ActionType.Build, "Building", 1, () => EndQuest(1));
        QuestsActions questsActions = new QuestsActions(
        //This Quest
        new List<Quest>() { quest },
        //Start Of CutScene
        //Go to the Building and then come back
        new List<Action>() { () => Camera.main.gameObject.GetComponent<CameraFollow>().MoveToTarget(GameObject.Find("Building"), 3.0f) },
        //End of CutScene
        //Start Tracking the OldLady
         new List<Action>() { () =>                   Camera.main.gameObject.GetComponent<CameraFollow>().ChangeTarget(GameObject.Find("OldLady")),
         //Pause the player and any enemies
           ()=>GameManager.GetGameManager().PauseAction(),
           ()=>GameObject.Find("OldLady").GetComponent<OldLady>().StartScene(2)
         });
        m_cutScenes.Add(questsActions);
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

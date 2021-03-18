using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    private static QuestUI s_instance;
    [SerializeField] GameObject m_logPrefab;
    [SerializeField] List<Quest> m_startingQuests;
    public static QuestUI GetQuestUI() { return s_instance; }

    [SerializeField]
    void Awake()
    {
        s_instance = this;
    }

    private void Start()
    {
        QuestManager.GetQuestManager().Initialize(m_startingQuests);
    }

    public  QuestLog AddQuest(Quest quest)
    {
        var log = Instantiate(m_logPrefab, transform);
        log.GetComponent<QuestLog>().SetQuest(quest);
        return log.GetComponent<QuestLog>();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    private static QuestUI s_instance;
    private readonly Dictionary<Quest, GameObject> m_logInstances = new Dictionary<Quest, GameObject>();
    [SerializeField] GameObject m_logPrefab;
    [SerializeField] List<Quest> m_startingQuests;
    [SerializeField] List<Quest> m_allQuest;
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

    public QuestLog AddQuest(Quest quest)
    {
        GameObject log = Instantiate(m_logPrefab, transform);
        log.GetComponent<QuestLog>().SetQuest(quest);
        m_logInstances[quest] = log;
        return log.GetComponent<QuestLog>();
    }

    public void RemoveQuest(Quest quest)
    {
        GameObject log = m_logInstances[quest];
        Destroy(log);
        m_logInstances.Remove(quest);
    }
}

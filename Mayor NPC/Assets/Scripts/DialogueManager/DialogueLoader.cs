using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    private static DialogueLoader s_dialogueLoader;

    public const string m_path = "Dialogue";
    private readonly Dictionary<string, DialogueContainer> m_dialogues = new Dictionary<string, DialogueContainer>();
    public static DialogueLoader GetLoader()
    {
        if (s_dialogueLoader == null)
        {
            Debug.LogError("There is no DialogueLoader in the scene or it is trying to be accesses before it has been loaded");
            return null;
        }
        return s_dialogueLoader;
    }
    public DialogueContainer GetDialogueContainerFor(string id)
    {
        DialogueContainer container;
        if (m_dialogues.TryGetValue(id, out container))
        {
            return container;
        }
        else
        {
            return null;
        }
    }
    private void Awake()
    {
        s_dialogueLoader = this;
        //load the container
        Load(m_path);
    }

    private void Load(string path)
    {
        //load all from  the given path
        TextAsset[] _xmls = Resources.LoadAll<TextAsset>(path);
        foreach (TextAsset _xml in _xmls)
        {
            //serialize the dialogue contianer
            XmlSerializer serializer = new XmlSerializer(typeof(DialogueContainer));

            StringReader reader = new StringReader(_xml.text);
            //Deserilaize out of the string reader as a dialogue container
            DialogueContainer container = serializer.Deserialize(reader) as DialogueContainer;
            reader.Close();
            m_dialogues.Add(container.m_treeOwner, container);
        }
    }
}

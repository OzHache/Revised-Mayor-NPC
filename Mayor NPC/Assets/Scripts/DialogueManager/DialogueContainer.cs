using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("DialogueTree")]
public class DialogueContainer
{
    [XmlAttribute("owner")]
    public string m_treeOwner;
    [XmlArray("Branches")]
    [XmlArrayItem("Branch")]
    public List<DialogueBranch> m_dialogueBranches = new List<DialogueBranch>();



    public bool GetNextBranch(out DialogueBranch branch, string choiceId = "")
    {

        branch = null;
        //early out if this has no branches
        if (m_dialogueBranches.Count == 0)
        {
            return false;
        }
        //deal with choice
        //find this branch
        //choiceId will be blank the first time so get the first child
        if (choiceId == "")
        {
            branch = m_dialogueBranches[0];
        }
        else
        {
            foreach (DialogueBranch dBranch in m_dialogueBranches)
            {
                if (dBranch.m_id == choiceId)
                {
                    branch = dBranch;
                    break;
                }
            }
        }
        //return if we have succeeded
        return branch != null;
    }

}


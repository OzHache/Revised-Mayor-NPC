using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class DialogueBranch
{
    //Each dialogue branch has the following information on load
    [XmlAttribute("id")]
    public string m_id;
    [XmlElement("Text")]
    public string m_maintext;
    [XmlArray("Choices")]
    [XmlArrayItem("Choice")]
    public List<Choice> m_choices = new List<Choice>();

}

public struct Choice
{
    [XmlElement("ChoiceID")]
    public string m_choiceId;
    [XmlElement("ChoiceText")]
    public string m_choiceText;


}

using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueParser
{
    public static List<DialogueLine> ParseDialogue(TextAsset xmlFile, string setId)
    {
        XDocument xml = XDocument.Parse(xmlFile.text);

        List<DialogueLine> results = new List<DialogueLine>();

        XElement set = xml.Root.Element("DialogueSet");

        foreach (var dialogueSet in xml.Root.Elements("DialogueSet"))
        {
            if (dialogueSet.Attribute("id").Value == setId)
            {
                foreach (var line in dialogueSet.Elements("Line"))
                {
                    results.Add(new DialogueLine
                    {
                        speaker = line.Attribute("speaker").Value,
                        text = line.Value
                    });
                }
            }
        }

        return results;
    }
}

public class DialogueLine
{
    public string speaker;
    public string text;
}
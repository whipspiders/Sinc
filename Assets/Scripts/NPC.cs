
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC")] 
public class NPC : ScriptableObject
{
    public Sprite npcSprite;
    public string npcName;

    public TextAsset dialogueXML;


    public string[] dialoguesFirstSet =
    { 
        ""
    };


    public string[] dialoguesSecondSet =
    { 
        ""
    };

}
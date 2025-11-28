using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameBox;
    [SerializeField] private TextMeshProUGUI dialogueBox;
    public string[] dialogueSet;
    private int index;
    bool dialogueActive;
    public UnityEvent endDialogue;

    //__________________________________________


    private Queue<DialogueLine> dialogueQueue;
    private NPC currentNPC;


    //__________________________________________

    public void SetName(string context)
    {
        nameBox.text = context;
    }
    public void PrintLine(int index)
    {
        dialogueActive = true;
        ToggleDialogue(true);
        dialogueBox.text = dialogueSet[index];
    }
    IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            dialogueBox.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ToggleDialogue(bool value)
    {
        Debug.Log($"Toggled dialogue to {value}");
        nameBox.gameObject.SetActive(value);
        dialogueBox.gameObject.SetActive(value);
    }
    void OnMouseDown()
    {
        print("click");
        ShowNextLine();
    }

    public void EndDialogue()
    {
        Debug.Log("Ended dialogue");
        ToggleDialogue(false);
        dialogueActive = false;
        index = 0;
        endDialogue.Invoke();
    }

    public void EndDialogueNoEvent()
    {
        Debug.Log("Ended dialogue without triggering event");
        ToggleDialogue(false);
        dialogueActive = false;
        index = 0;
    }








//_____________________________________XML parser Ver_____________________________________________________



  public void StartDialogue(NPC npc, string setId)
    {
        currentNPC = npc;

        List<DialogueLine> lines = DialogueParser.ParseDialogue(npc.dialogueXML, setId);

        dialogueQueue = new Queue<DialogueLine>(lines);

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueQueue.Dequeue();

        // ⬇️ UPDATE UI
        nameBox.text = line.speaker;
        dialogueBox.text = line.text;
    }

    //private void EndDialogue()
    //{
        //nameBox.text = "";
        //dialogueBox.text = "";
        //Debug.Log("Dialogue finished.");
    //}
}
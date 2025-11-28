using System.Collections;
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
        if (dialogueActive)
        {
            if (index == dialogueSet.Length - 1)
            {
                EndDialogue();
            }
            else
            {
                index++;
                PrintLine(index);
            }
        }
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

}

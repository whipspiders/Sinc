using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameBox;
    [SerializeField] private TextMeshProUGUI dialogueBox;

    public UnityEvent onDialogueEnd; // GameController listens to this

    private Queue<DialogueLine> dialogueQueue;
    private bool isTyping = false;
    private string currentFullLine = "";
    private Coroutine typingCoroutine;

    public float typingSpeed = 0.03f;

    public void StartDialogue(NPC npc, string setId)
    {

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
        nameBox.text = line.speaker;

        currentFullLine = line.text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dialogueBox.text = "";

        foreach (char c in text)
        {
            dialogueBox.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public IEnumerator TypeLineExternal(string text, TextMeshProUGUI tmpAsset)
    {
        isTyping = true;
        tmpAsset.text = "";

        foreach (char c in text)
        {
            tmpAsset.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // Clicking anywhere on dialogue box collider
    private void OnMouseDown()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueBox.text = currentFullLine;
            isTyping = false;
        }
        else
        {
            ShowNextLine();
        }
    }

    private void EndDialogue()
    {
        nameBox.text = "";
        dialogueBox.text = "";

        onDialogueEnd.Invoke();
    }
}
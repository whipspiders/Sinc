// DialogueController.cs
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameBox;
    [SerializeField] private TextMeshProUGUI dialogueBox;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource audioSource;
    public UnityEvent onDialogueLineComplete; // triggered when line finished typing
    public UnityEvent onDialogueEnd;          // triggered when all lines finished

    private Queue<DialogueLine> dialogueQueue;
    private bool isTyping = false;
    private string currentFullLine = "";
    private Coroutine typingCoroutine;
    public float typingSpeed = 0.03f;

    public bool IsTyping => isTyping;

    #region Dialogue Methods
    public void StartDialogue(NPC npc, string setId)
    {
        var lines = DialogueParser.ParseDialogue(npc.dialogueXML, setId);
        dialogueQueue = new Queue<DialogueLine>(lines);
        ShowNextLine();
    }

    private void ShowNextLine()
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
            TypingSound();
        }

        isTyping = false;
        onDialogueLineComplete.Invoke();

        
    }

    void TypingSound()
    {
                    var audioInstance = Instantiate(audioSource, transform);
            audioInstance.resource = typingSound;
            audioInstance.Play();
    }

    public void CompleteLineInstantly()
    {
        if (!isTyping) return;

        StopCoroutine(typingCoroutine);
        dialogueBox.text = currentFullLine;
        isTyping = false;
        onDialogueLineComplete.Invoke();
    }

    public void AdvanceLine()
    {
        if (isTyping)
            CompleteLineInstantly();
        else
            ShowNextLine();
    }
    #endregion

    #region External TypeLine
    public IEnumerator TypeLineExternal(string text, TextMeshProUGUI tmpAsset)
    {
        isTyping = true;
        tmpAsset.text = "";

        foreach (char c in text)
        {
            tmpAsset.text += c;
            yield return new WaitForSeconds(typingSpeed);
            AudioSource.PlayClipAtPoint(typingSound, Vector3.zero, 1.0f);
            TypingSound();
        }

        isTyping = false;
        onDialogueLineComplete.Invoke();
    }
    #endregion

    #region End Dialogue
    private void EndDialogue()
    {
        nameBox.text = "";
        dialogueBox.text = "";
        onDialogueEnd.Invoke();
    }
    #endregion

    #region Input Handling
    private void OnMouseDown()
    {
        AdvanceLine();
    }
    #endregion
}

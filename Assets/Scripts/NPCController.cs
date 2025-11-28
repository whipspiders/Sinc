using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    [SerializeField] private Image npcImage;
    [SerializeField] private DialogueController dialogueController;

    public void SpawnNPC(NPC npc, string[] dialogue)
    {
        npcImage.gameObject.SetActive(true);
        npcImage.sprite = npc.npcSprite;
        dialogueController.SetName(npc.npcName);
        dialogueController.dialogueSet = dialogue;

        dialogueController.StartDialogue(npc, "quest_end");

        //dialogueController.PrintLine(0);
    }

    public virtual void DespawnNpc(NPC npc)
    {
        npcImage.gameObject.SetActive(false);
        dialogueController.EndDialogueNoEvent();
    }
}

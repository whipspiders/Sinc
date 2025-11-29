using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    [SerializeField] private Image npcImage;
    [SerializeField] private DialogueController dialogueController;

    public void SpawnNPC(NPC npc, bool minigame, string dialogueSet)
    {
        npcImage.gameObject.SetActive(true);
        npcImage.sprite = npc.npcSprite;

        dialogueController.StartDialogue(npc, dialogueSet, minigame);
    }

    public void DespawnNPC()
    {
        npcImage.gameObject.SetActive(false);
    }
}
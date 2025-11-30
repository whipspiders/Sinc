// NPCController.cs
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    [SerializeField] private Image npcImage;
    [SerializeField] private DialogueController dialogueController;

    public void SpawnNPC(NPC npc, string dialogueSet, Sprite overrideSprite = null)
    {
        npcImage.gameObject.SetActive(true);
        npcImage.sprite = overrideSprite != null ? overrideSprite : npc.npcSprite;
        dialogueController.StartDialogue(npc, dialogueSet);
    }

    public void DespawnNPC()
    {
        npcImage.gameObject.SetActive(false);
    }

    public void ChangeNPCSprite(Sprite newSprite)
    {
        npcImage.sprite = newSprite;
    }
}

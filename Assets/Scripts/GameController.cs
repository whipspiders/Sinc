
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayCounterTMP;
    [SerializeField] private UIController uiController;
    [SerializeField] private NPCController npcController;
    [SerializeField] private MiniGameController miniGameController;
    [SerializeField] private NPC[] npcs;
    private int appearances;
    private int dayCounter;
    private NPC storedNPC;

    void Awake()
    {
        npcs = Resources.LoadAll<NPC>("npc") as NPC[];
    }

    void Start()
    {
        SpawnRandomNPC();
    }
    void SpawnRandomNPC()
    {
        dayCounterTMP.text = $"День {dayCounter.ToString()}";
        appearances = 0;
        storedNPC = npcs[Random.Range(0, npcs.Length-1)];
        npcController.SpawnNPC(storedNPC, storedNPC.dialoguesFirstSet);
        appearances++;
        Debug.Log(storedNPC + "appearances count:" + appearances);
    }

    IEnumerator RespawnNpc(NPC npc)
    {
        npcController.DespawnNpc(npc);
        Debug.Log("Despawning NPC");
        yield return new WaitForSeconds(3);
        SpawnRandomNPC();
    }

    public virtual void StartMiniGame()
    {
        uiController.dialogueWindow.ToggleWindow(false);
        uiController.miniGameWindow.ToggleWindow(true);
        Debug.Log("Starting minigame");
    }

    public virtual void EndMiniGame()
    {
        uiController.dialogueWindow.ToggleWindow(true);
        uiController.miniGameWindow.ToggleWindow(false);
        npcController.SpawnNPC(storedNPC, storedNPC.dialoguesSecondSet);
        appearances++;
        Debug.Log(storedNPC + "appearances count:" + appearances);
        Debug.Log("Ending minigame");
    }

    public virtual void DialogueExit()
    {
        Debug.Log("The dialogue should end here");

        if (appearances == 1)
        {
            StartMiniGame();
        }
        else if (appearances == 2)
        {
            Debug.Log("The NPC should be respawned now");
            dayCounter++;
            
            StartCoroutine(RespawnNpc(storedNPC));
        }
    }
}
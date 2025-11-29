using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayCounterTMP;
    [SerializeField] private UIController uiController;
    [SerializeField] private NPCController npcController;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private MiniGameController miniGameController;

    private NPC[] npcs;
    private int activeNPC;

    // ===== QUEUE SYSTEM =====
    private enum NEXT_ACTION
    {
        NONE,
        START_MINIGAME,
        RESPAWN_NPC,
        GAME_END,
        MAIN_MENU
    }

    private struct DialogueResult
    {
        public NEXT_ACTION action;
        public int npcIndex;
        public string dialogueSet;
        public float delay;

        public DialogueResult(NEXT_ACTION action, int npcIndex = -1, string dialogueSet = "introduction", float delay = 0)
        {
            this.action = action;
            this.npcIndex = npcIndex;
            this.dialogueSet = dialogueSet;
            this.delay = delay;
        }
    }

    private Queue<DialogueResult> resultQueue = new Queue<DialogueResult>();

    void Awake()
    {
        npcs = Resources.LoadAll<NPC>("npc");
        npcs = npcs.OrderBy(s => s.name).ToArray();
    }

    void Start()
    {
        dialogueController.onDialogueEnd.AddListener(OnDialogueEnded);

        // CUTSCENE
        StartNPC(1, "introduction");
        NPCNext(1, "introduction", 3f);
        NPCMinigame();
        NPCNext(1, "post-surgery", 0f);

        // FIRST DAY _______________________________________________________________________________________
        //CYBORG
        NPCNext(2, "introduction", 1f);
        NPCMinigame();
        NPCNext(2, "post-surgery", 0f);

        GameEnd();


    }

    void NextDay()
    {

    }

    void NPCEnd()
    {
        resultQueue.Enqueue(new DialogueResult(NEXT_ACTION.NONE));
    }

    void InsertNextResultAtFront(DialogueResult result)
    {
        var list = resultQueue.ToList();
        list.Insert(0, result);
        resultQueue = new Queue<DialogueResult>(list);
    }

    public void NPCMinigame()
    {
        resultQueue.Enqueue(new DialogueResult(NEXT_ACTION.START_MINIGAME));
    }

    public void NPCNext(int npcIndex, string dialogueSet, float delay)
    {
        resultQueue.Enqueue(new DialogueResult(
            NEXT_ACTION.RESPAWN_NPC,
            npcIndex,
            dialogueSet,
            delay
        ));
    }

    public void StartNPC(int index, string dialogueSet)
    {
        activeNPC = index;
        npcController.SpawnNPC(npcs[index], false, dialogueSet);
    }
    private void OnDialogueEnded()
    {
        if (resultQueue.Count == 0)
        {
            npcController.DespawnNPC();
            return;
        }

        DialogueResult result = resultQueue.Dequeue();
        StartCoroutine(HandleDialogueResult(result));
    }

    private IEnumerator HandleDialogueResult(DialogueResult result)
    {
        switch (result.action)
        {
            case NEXT_ACTION.NONE:
                npcController.DespawnNPC();
                yield break;

            case NEXT_ACTION.START_MINIGAME:
                StartMinigame();
                yield break;

            case NEXT_ACTION.RESPAWN_NPC:
                npcController.DespawnNPC();
                yield return new WaitForSeconds(result.delay);
                StartNPC(result.npcIndex, result.dialogueSet);
                yield break;
        }
    }

    private void StartMinigame()
    {
        uiController.dialogueWindow.ToggleWindow(false);
        uiController.miniGameWindow.ToggleWindow(true);
        miniGameController.StartGame();
    }

    public virtual void Continue()
    {
        uiController.dialogueWindow.ToggleWindow(true);
        uiController.miniGameWindow.ToggleWindow(false);

        miniGameController.End();
    }

        // Inject NPC respawn as the NEXT action (highest priority)
        // InsertNextResultAtFront(
        //     new DialogueResult(
        //         NEXT_ACTION.RESPAWN_NPC,
        //         activeNPC,
        //         "post-surgery",
        //         0f
        //     )
        // );

    public void GameEnd()
    {
        resultQueue.Enqueue(new DialogueResult(NEXT_ACTION.GAME_END));
        Debug.Log("The game should end here");
    }

    public void LoadMenu()
    {
        resultQueue.Enqueue(new DialogueResult(NEXT_ACTION.MAIN_MENU));
        SceneManager.LoadScene("MainMenu");
    }


        public virtual void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
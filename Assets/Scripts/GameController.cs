using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dayCounterTMP;
    [SerializeField] private UIController uiController;
    [SerializeField] private NPCController npcController;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private MiniGameController miniGameController;
    [SerializeField] private GameObject blackout;
    [SerializeField] private TextMeshProUGUI blackoutTMPAsset;
    [SerializeField] private SpriteRenderer backgroundDef;
    [SerializeField] private Sprite backgroundEnd;
    private bool waitingForMiniGame = false;

    [Header("NPCs")]
    private NPC[] npcs;
    private int dayCounter = 1;

    #region Queue System
    public enum QueueActionType { SPAWN_NPC, START_MINIGAME, NEXT_DAY, GAME_END }

    public class QueueAction
    {
        public QueueActionType type;
        public int npcIndex;
        public string dialogueSet;
        public Sprite overrideSprite;
        public float delay;

        public QueueAction(QueueActionType type, int npcIndex = -1, string dialogueSet = "", Sprite overrideSprite = null, float delay = 0f)
        {
            this.type = type;
            this.npcIndex = npcIndex;
            this.dialogueSet = dialogueSet;
            this.overrideSprite = overrideSprite;
            this.delay = delay;
        }
    }

    private Queue<QueueAction> actionQueue = new Queue<QueueAction>();
    private bool isQueueRunning = false;

    public void EnqueueAction(QueueAction action)
    {
        actionQueue.Enqueue(action);
        if (!isQueueRunning)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isQueueRunning = true;

        while (actionQueue.Count > 0)
        {
            QueueAction action = actionQueue.Dequeue();

            switch (action.type)
            {
                case QueueActionType.SPAWN_NPC:
                    npcController.DespawnNPC();
                    if (action.delay > 0f)
                        yield return new WaitForSeconds(action.delay);

                    npcController.SpawnNPC(npcs[action.npcIndex], action.dialogueSet, action.overrideSprite);

                    // Wait until all dialogue lines are completed
                    bool dialogueEnded = false;
                    UnityAction dialogueEndCallback = () => dialogueEnded = true;
                    dialogueController.onDialogueEnd.AddListener(dialogueEndCallback);

                    yield return new WaitUntil(() => dialogueEnded);

                    dialogueController.onDialogueEnd.RemoveListener(dialogueEndCallback);
                    break;

                case QueueActionType.START_MINIGAME:
                    npcController.DespawnNPC();
                    uiController.dialogueWindow.ToggleWindow(false);
                    uiController.miniGameWindow.ToggleWindow(true);
                    Fade(miniGameController.miniGameContainer, 1);
                    Fade(uiController.miniGameWindow.gameObject, 1);

                    waitingForMiniGame = true;

                    miniGameController.StartGame(() =>
                    {
                        EndMiniGame();

                    });

                    // Pause queue without dequeuing next item
                    yield return new WaitUntil(() => waitingForMiniGame == false);
                    break;

                case QueueActionType.NEXT_DAY:
                    npcController.DespawnNPC();
                    Fade(blackout, 1);
                    dayCounter++;
                    dayCounterTMP.text = "День " + dayCounter;

                    yield return dialogueController.TypeLineExternal("День " + dayCounter, blackoutTMPAsset);
                    yield return new WaitForSeconds(2f);

                    Fade(blackout, 0);
                    blackoutTMPAsset.text = "";
                    break;

                case QueueActionType.GAME_END:
                    npcController.DespawnNPC();
                    Fade(blackout, 1);
                    yield return new WaitForSeconds(0.5f);
                    SceneManager.LoadScene("MainMenu");
                    yield break;
            }

            // Tiny delay for smooth transitions
            yield return null;
        }

        isQueueRunning = false;
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        npcs = Resources.LoadAll<NPC>("npc");
        npcs = npcs.OrderBy(n => n.name).ToArray();
    }

    private void Start()
    {
        dialogueController.onDialogueEnd.AddListener(OnDialogueEnd);

        Fade(blackout, 0);

        // Enqueue full game sequence
        SetupGameSequence();
    }

    private void OnDialogueEnd()
    {
        // nothing extra needed, queue waits for onDialogueEnd automatically
    }
    #endregion

    public void EndMiniGame()
    {

        if (!miniGameController) return;

        // Stop mini-game
        miniGameController.End();

        // Restore dialogue UI
        uiController.dialogueWindow.ToggleWindow(true);
        uiController.miniGameWindow.ToggleWindow(false);
        Fade(miniGameController.miniGameContainer, 0);
        Fade(uiController.miniGameWindow.gameObject, 0);

        // Allow the queue to continue past the mini-game action
        waitingForMiniGame = false;
    }
    #region Game Sequence Setup
    private void SetupGameSequence()
    {
        // ---------------- DAY 1 ----------------
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 0, "introduction"));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 1, "introduction", npcs[1].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 1, "post-surgery", npcs[1].postSurgerySprite));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 0, "inbeetwin_collector_cyborg", npcs[0].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 2, "introduction", npcs[2].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 2, "post-surgery", npcs[2].postSurgerySprite));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 0, "inbeetwin_cyborg_bird", npcs[0].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 3, "introduction", npcs[3].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 3, "post-surgery", npcs[3].postSurgerySprite));
        EnqueueAction(new QueueAction(QueueActionType.NEXT_DAY));

        // ---------------- DAY 2 ----------------
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 4, "introduction", npcs[4].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 4, "post-surgery", npcs[4].postSurgerySprite));

        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 5, "introduction", npcs[5].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 5, "post-surgery", npcs[5].postSurgerySprite));

        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 6, "introduction", npcs[6].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 6, "post-surgery", npcs[6].postSurgerySprite));

        EnqueueAction(new QueueAction(QueueActionType.NEXT_DAY));

        // ---------------- DAY 3 ----------------
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 7, "introduction", npcs[7].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 7, "post-surgery", npcs[7].postSurgerySprite));

        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 8, "introduction", npcs[8].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.START_MINIGAME));
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 8, "post-surgery", npcs[8].postSurgerySprite));

        // ---------------- END CUTSCENE ----------------
        EnqueueAction(new QueueAction(QueueActionType.SPAWN_NPC, 1, "ending", npcs[1].npcSprite, 1f));
        EnqueueAction(new QueueAction(QueueActionType.GAME_END));
    }
    #endregion

    #region Helpers
    private void Fade(GameObject obj, float value, float duration = 0.05f)
    {
        obj.transform.DOScaleX(value, duration);
    }

    public virtual void Exit()
    {
        StartCoroutine(ExitEnum());
    }

    private IEnumerator ExitEnum()
    {
        Fade(blackout, 1);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
    
}

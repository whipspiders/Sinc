using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Threading.Tasks;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayCounterTMP;
    [SerializeField] private UIController uiController;
    [SerializeField] private NPCController npcController;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private MiniGameController miniGameController;
    [SerializeField] private GameObject blackout;

    [SerializeField] private TextMeshProUGUI blackoutTMPAsset;
    [SerializeField] private SpriteRenderer backgroundDef;
    [SerializeField] private Sprite backgroundEnd;
    

    private int dayCounter = 1;

    private NPC[] npcs;
    private int activeNPC;

    // ===== QUEUE SYSTEM =====
    private enum NEXT_ACTION
    {
        START_MINIGAME,
        RESPAWN_NPC,
        GAME_END,
        NEXT_DAY
    }

    private struct DialogueResult
    {
        public NEXT_ACTION action;
        public int npcIndex;
        public string dialogueSet;
        public float delay;

        // NEW — optional sprite override
        public Sprite overrideSprite;

        public DialogueResult(
            NEXT_ACTION action,
            int npcIndex = -1,
            string dialogueSet = "introduction",
            float delay = 0,
            Sprite overrideSprite = null)
        {
            this.action = action;
            this.npcIndex = npcIndex;
            this.dialogueSet = dialogueSet;
            this.delay = delay;
            this.overrideSprite = overrideSprite;
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
        Fade(blackout, 0);

        //_______________//CUTSCENE
        StartNPC(0, "introduction");

        // FIRST DAY _______________________________________________________________________________________

        // COLLECTOR 
        NPCNext(1, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(1, "post-surgery", 0f, "post-surgery");

        //_______________//CUTSCENE
        NPCNext(0, "inbeetwin_collector_cyborg", 1f, "normal");

        //CYBORG
        NPCNext(2, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(2, "post-surgery", 0f, "post-surgery");

        //_______________//CUTSCENE
        NPCNext(0, "inbeetwin_cyborg_bird", 1f, "normal");

        //BIRD
        NPCNext(3, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(3, "post-surgery", 0f, "post-surgery");

        //DAY END

        NextDay();

        // SECOND DAY _______________________________________________________________________________________

        //CHEST
        NPCNext(4, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(4, "post-surgery", 0f, "post-surgery");

        //HEART
        NPCNext(5, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(5, "post-surgery", 0f, "post-surgery");


        //IMPLANT
        NPCNext(6, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(6, "post-surgery", 0f, "post-surgery");

        NextDay();

        // THIRD DAY _______________________________________________________________________________________

        //HEAD
        NPCNext(7, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(7, "post-surgery", 0f, "post-surgery");
        //CENTAUR
        NPCNext(8, "introduction", 1f, "normal");
        NPCMinigame();
        NPCNext(8, "post-surgery", 0f, "post-surgery");

        NextDay();

        //END CUTSCENE
        NPCNext(1, "ending", 1f, "normal");

        GameEnd();
    }

    void NextDay()
    {
        resultQueue.Enqueue(new DialogueResult(NEXT_ACTION.NEXT_DAY));
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

    // ORIGINAL
    public void NPCNext(int npcIndex, string dialogueSet, float delay, string spriteType)
    {
        Sprite chosenSprite = null;
        if (spriteType == "normal")
            chosenSprite = npcs[npcIndex].npcSprite;
        if (spriteType == "post-surgery")
            chosenSprite = npcs[npcIndex].postSurgerySprite;

        resultQueue.Enqueue(new DialogueResult(
            NEXT_ACTION.RESPAWN_NPC,
            npcIndex,
            dialogueSet,
            delay,
            chosenSprite
        ));
    }

    // NEW — NPCNext WITH SPRITE CHANGE
    public void NPCNext(int npcIndex, string dialogueSet, float delay, Sprite newSprite)
    {
        resultQueue.Enqueue(new DialogueResult(
            NEXT_ACTION.RESPAWN_NPC,
            npcIndex,
            dialogueSet,
            delay,
            newSprite
        ));
    }

    public void StartNPC(int index, string dialogueSet)
    {
        activeNPC = index;
        npcController.SpawnNPC(npcs[index], dialogueSet);
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
            case NEXT_ACTION.START_MINIGAME:
                StartMinigame();
                yield break;

            case NEXT_ACTION.RESPAWN_NPC:
                npcController.DespawnNPC();
                yield return new WaitForSeconds(result.delay);

                StartNPC(result.npcIndex, result.dialogueSet);

                // NEW — apply sprite override if provided
                if (result.overrideSprite != null)
                    npcController.ChangeNPCSprite(result.overrideSprite);

                yield break;

            case NEXT_ACTION.GAME_END:
                npcController.DespawnNPC();
                Fade(blackout, 1);
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene("MainMenu");
                yield break;

            case NEXT_ACTION.NEXT_DAY:
                npcController.DespawnNPC();
                Fade(blackout, 1);
                dayCounter++;
                dayCounterTMP.text = "Day " + dayCounter.ToString();

                StartCoroutine(dialogueController.TypeLineExternal("Day " + dayCounter.ToString(), blackoutTMPAsset));

                if (dayCounter > 3)
                {
                    dayCounterTMP.text = "...";
                    backgroundDef.sprite = backgroundEnd;
                }
                yield return new WaitForSeconds(2f);
                Fade(blackout, 0);
                blackoutTMPAsset.text = "";
                if (resultQueue.Count > 0)
                {
                    DialogueResult next = resultQueue.Dequeue();
                    yield return StartCoroutine(HandleDialogueResult(next));
                }
                yield break;
        }
    }

    private void StartMinigame()
    {
        uiController.dialogueWindow.ToggleWindow(false);
        uiController.miniGameWindow.ToggleWindow(true);
        Fade(uiController.miniGameWindow.gameObject, 1);
        Fade(miniGameController.miniGameContainer, 1);
        miniGameController.StartGame();
    }

    public virtual void Continue()
    {
        uiController.dialogueWindow.ToggleWindow(true);
        Fade(uiController.miniGameWindow.gameObject, 0);
        Fade(miniGameController.miniGameContainer, 0);
        miniGameController.End();
    }

    public void GameEnd()
    {
        resultQueue.Enqueue(new DialogueResult(NEXT_ACTION.GAME_END));
    }

    public virtual void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    void FullFade(GameObject gameObject, float duration = 0.05f)
    {
        Sequence fullFade = DOTween.Sequence();
        fullFade.Append(gameObject.transform.DOScaleX(1, duration));
        fullFade.Append(gameObject.transform.DOScaleX(0, duration));

        fullFade.Play();
    }

    public void Fade(GameObject gameObject, int value = 1, float duration = 0.05f)
    {
        gameObject.transform.DOScaleX(value, duration);
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.Threading.Tasks;

public class MiniGameController : MonoBehaviour
{
    private Circle circle;
    [SerializeField] private Circle[] circles;
    [SerializeField] private Image progressBar;
    public GameObject miniGameContainer;
    [SerializeField] private TextMeshProUGUI timerTMP;
    [SerializeField] private UnityEvent won;
    [SerializeField] private UnityEvent lost;
    [SerializeField] private Slider slider;

    private float gameTimer;
    private bool onCooldown;
    private bool win;

    private bool sliderProgressCooldown;

    // 🔥 NEW
    public bool gameOngoing = false;

    void Awake()
    {
        foreach (Circle circle in circles)
        {
            circle.gameObject.SetActive(false);
        }
        //miniGameContainer.SetActive(false);
    }

    public void StartGame(System.Action onComplete = null)
    {
        gameOngoing = true;
        onCooldown = false;
        sliderProgressCooldown = false;

        gameTimer = 30;
        win = false;

        progressBar.fillAmount = 0;
        //miniGameContainer.SetActive(true);
        miniGameContainer.transform.position = new Vector3(0, 0, 0);

        StartCircleGame();
        StartPushGame();

        // Subscribe to won/lost events to trigger callback
        UnityAction callbackAction = null;
        callbackAction = () =>
        {
            onComplete?.Invoke();
            won.RemoveListener(callbackAction);
            lost.RemoveListener(callbackAction);
        };

        won.AddListener(callbackAction);
        lost.AddListener(callbackAction);
    }

    public virtual void TaskDone(float value = 0.05F)
    {
        progressBar.fillAmount += value;
    }

    void Update()
    {
        // 🔥 Skip whole block if game is not ongoing
        if (!gameOngoing)
            return;

        // === TIMER ===
        if (gameTimer > 0)
        {
            gameTimer -= Time.deltaTime;
            timerTMP.text = Mathf.Floor(gameTimer).ToString();

            // --- SPACE pushes slider upward ---
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!onCooldown)
                    StartCoroutine(SlideUp());
            }

            // --- fill bar while slider > 0.7 ---
            if (slider.value > 0.7f && !sliderProgressCooldown)
            {
                StartCoroutine(FillProgressWhileSliderHigh());
            }

            // --- Win ---
            if (progressBar.fillAmount >= 1 && !win)
            {
                win = true;
                won.Invoke();
                End();
            }
        }
        else
        {
            lost.Invoke();
        }
    }

    void StartPushGame()
    {
        InvokeRepeating(nameof(SlideDown), 0.1f, 0.1f);
    }

    IEnumerator SlideUp()
    {
        onCooldown = true;
        slider.value += 0.05f;
        yield return new WaitForSeconds(0.05f);
        onCooldown = false;
    }

    IEnumerator FillProgressWhileSliderHigh()
    {
        sliderProgressCooldown = true;

        progressBar.fillAmount += 0.005f;

        yield return new WaitForSeconds(0.1f);
        sliderProgressCooldown = false;
    }

    void SlideDown()
    {
        slider.value -= 0.02f;
    }

    void StartCircleGame()
    {
        foreach (Circle circle in circles)
        {
            circle.gameObject.SetActive(true);
            circle.RotateCircle();
        }
    }

    public void End()
    {
        gameOngoing = false; // 🔥 Stop timer + input
        miniGameContainer.transform.position = new Vector3(50, 0, 0);
        //miniGameContainer.SetActive(false);

        foreach (Circle circle in circles)
        {
            circle.rb.angularVelocity = 0;
            circle.gameObject.SetActive(false);
        }

        CancelInvoke();
    }
}

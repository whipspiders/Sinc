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
    [SerializeField] private GameObject miniGameContainer;
    [SerializeField] private TextMeshProUGUI timerTMP;
    [SerializeField] private UnityEvent won;
    [SerializeField] private UnityEvent lost;
    [SerializeField] private Slider slider;
    private float gameTimer = 60;
    private bool onCooldown;

    private bool win;

    void Awake()
    {
        miniGameContainer.SetActive(false);
    }

    public void StartGame()
    {
        onCooldown = false;
        gameTimer = 60;
        win = false;
        progressBar.fillAmount = 0;
        miniGameContainer.SetActive(true);
        StartCircleGame();
        StartPushGame();
    }

    void Update()
    {
        if (gameTimer > 0)
        {

            gameTimer -= Time.deltaTime;
            timerTMP.text = Mathf.Floor(gameTimer).ToString();

            //____________slider game
                    if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!onCooldown)
            {
                StartCoroutine(SlideUp());
            }
            else
            {
                Debug.Log("Wait for cooldown to end");
            }
        }


            if (progressBar.fillAmount == 1)
            {
                won.Invoke();
                win = true;
            }
        }
        else { lost.Invoke(); }
    }

    void StartPushGame()
    {
        InvokeRepeating("SlideDown", 0.1f, 0.1f);
    }


    IEnumerator SlideUp()
    {
        onCooldown = true;
        slider.value += 0.05f;
        yield return new WaitForSeconds(0.05f);
        onCooldown = false;
    }
        void SlideDown()
    {
        slider.value -= 0.02f;
    }
    void StartCircleGame()
    {
        foreach (Circle circle in circles)
        {
            circle.RotateCircle();
        }
    }

    public virtual void TaskDone(float value = 0.05F)
    {
        progressBar.fillAmount += value;
    }

    public void End()
    {
        miniGameContainer.SetActive(false);
        foreach (Circle circle in circles)
        {
            circle.rb.angularVelocity = 0;
        }
        CancelInvoke();
    }
}

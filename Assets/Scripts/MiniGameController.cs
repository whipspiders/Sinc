using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class MiniGameController : MonoBehaviour
{
    private Circle circle;
    [SerializeField] private Circle[] circles;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject miniGameContainer;

    void Awake()
    {
        miniGameContainer.SetActive(false);
    }

    public void StartGame()
    {
        miniGameContainer.SetActive(true);
        StartCircleGame();
    }

    void StartCircleGame()
    {
        foreach (Circle circle in circles)
        {
            circle.RotateCircle();
        }
    }

    public void End()
    {
        miniGameContainer.SetActive(false);
        foreach (Circle circle in circles)
        {
            circle.rb.angularVelocity = 0;
        }
    }
}

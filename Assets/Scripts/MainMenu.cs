using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuText : MonoBehaviour
{
    [SerializeField] private GameObject menuBlackout;
    [SerializeField] private TextMeshProUGUI tmpAsset;

    void Start()
    {
        menuBlackout.transform.DOScaleX(0, 0.05f);
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Main");
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuText : MonoBehaviour
{
    private TextMeshProUGUI tmpAsset => gameObject.GetComponent<TextMeshProUGUI>();
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Main");
        }
    }
}

using UnityEngine;

public class UIWindow : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private bool isActive;

    void Start()
    {
        window.gameObject.SetActive(isActive);
    }

    public virtual void ToggleWindow(bool context)
    {
        window.gameObject.SetActive(context);
    }
}

using UnityEngine;

public class SpinningElement : MonoBehaviour
{
    [SerializeField] Circle circle;

    private void OnTriggerEnter2D(Collider2D other)
    {
        circle.InTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        circle.InTrigger = false;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{

    public Rigidbody2D rb;

    [SerializeField] private KeyCode bindKey;

    [SerializeField] private int timer;
    public bool InTrigger;
    void Update()
    {
        if (Input.GetKeyDown(bindKey))
    {
        Push();
    }
    }
    public void RotateCircle()
    {
        rb.angularVelocity = 3 *  100;
    }

    void Push()
    {
        if (InTrigger)
        {
            Debug.Log("Good timing");
            rb.angularVelocity = 0;
            StartCoroutine(RestartUponTimer(3));

        }
        else Debug.Log("Bad timing");
    }
    
    IEnumerator RestartUponTimer(int context)
    {
        yield return new WaitForSeconds(context);
        RotateCircle();
    }
}

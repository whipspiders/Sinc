using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Circle : MonoBehaviour
{
    [SerializeField] private UnityEvent done;
    public Rigidbody2D rb;

    [SerializeField] private KeyCode bindKey;

    [SerializeField] private int timer;
    [SerializeField] private Color lightSpinning;
    [SerializeField] private Color lightStopped;

    [SerializeField] private SpriteRenderer glowSprite;
    [SerializeField] private Sprite glowDef;
    [SerializeField] private Sprite glowGood;
    
    public bool InTrigger;
    private bool isMoving;

    void Awake()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(bindKey))
    {
        Push();
    }
    }
    public void RotateCircle()
    {
        glowSprite.sprite = glowDef;
        rb.angularVelocity = 3 *  100;
        isMoving = true;
    }

    void Push()
    {
        if (!isMoving) return;
        if (InTrigger)
        {
            done.Invoke();
            Debug.Log("Good timing");
            rb.angularVelocity = 0;
            isMoving = false;
            glowSprite.sprite = glowGood;
            StartCoroutine(RestartUponTimer(3));
        }
        else Debug.Log("Bad timing");
    }
    
    IEnumerator RestartUponTimer(int context)
    {
        yield return new WaitForSeconds(context);
        isMoving = true;
        RotateCircle();
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class sliderTest : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private bool onCooldown;
    void Start()
    {
        onCooldown = false;
        StartSliding();
    }

    void StartSliding()
    {
        InvokeRepeating("SlideDown", 0.1f, 0.1f);
    }

    void Update()
    {
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightsBlinkController : MonoBehaviour
{
    [SerializeField] private Image LightsImage;

    [SerializeField] private Sprite LightsOff;
    [SerializeField] private Sprite LightsOn;

    [SerializeField] private float flickerTime;

    private bool areLightsOn = false;

    private void Start()
    {
        StartCoroutine(LightsFlicker());
    }
    IEnumerator LightsFlicker()
    {
        yield return new WaitForSeconds(flickerTime);

        if (areLightsOn)
        {
            LightsImage.sprite = LightsOff;
            areLightsOn = false;
        }
        else
        {
            LightsImage.sprite = LightsOn;
            areLightsOn = true;
        }
        StartCoroutine(LightsFlicker());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllWinsMultiplied : MonoBehaviour
{
    [SerializeField] private GameObject AllWinsMultipliedIndicator;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("2xCoin!", 1) == 2)
            AllWinsMultipliedIndicator.SetActive(true);
    }

    public void ShowIndicator()
    {
        AllWinsMultipliedIndicator.SetActive(true);
    }

    public void HideIndicator()
    {
        AllWinsMultipliedIndicator.SetActive(false);
    }
}

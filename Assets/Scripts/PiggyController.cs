using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiggyController : MonoBehaviour
{
    [SerializeField] private Text PiggyText;

    private void Awake()
    {
        int coins = PlayerPrefs.GetInt("PiggyCoins", 0);
        
        if(coins <= 0)
            PiggyText.text = coins.ToString();
        else
            PiggyText.text = coins.ToString("0#,0");
    }
}

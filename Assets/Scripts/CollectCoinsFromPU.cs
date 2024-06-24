using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;
using System.Threading.Tasks;

public class CollectCoinsFromPU : MonoBehaviour
{
    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
    public Text coinsText;
    private int coins;
    [SerializeField] private Text vipMultiplierText;
    [SerializeField] private Text TotalCoinsText;

    [SerializeField] private GameObject Display;

    private void Awake()
    {
        coins = PlayerPrefs.GetInt("WonCoins", 0);
        //coinsText.text = coins.ToString("NO");
        coinsText.text = string.Format("{0:#,0}", coins);
        if(vipMultiplierText != null)
            vipMultiplierText.text = MPlayer.StatusMultiplier.ToString();

        if (TotalCoinsText != null)
        {
            coins = (int)(coins * MPlayer.StatusMultiplier);
            //TotalCoinsText.text = coins.ToString();
            TotalCoinsText.text = string.Format("{0:#,0}", coins);
            //PlayerPrefs.SetInt("WonCoins", coins);
        }
    }

    public void CollectCoins()
    {
        MPlayer.AddCoins(coins);
    }

    public void Collect2XCoins()
    {
        MPlayer.AddCoins(coins*2);
    }

    public async void CloseWindowAfterMiliseconds(int seconds)
    {
        //if(Display != null)
        //    Display.SetActive(false);

        await Task.Delay(seconds);

        GetComponent<PopUpsController>().CloseWindow();
    }

    public async void HideAndCloseWindowAfterMiliseconds(int seconds)
    {
        if (Display != null)
            Display.SetActive(false);

        await Task.Delay(seconds);

        GetComponent<PopUpsController>().CloseWindow();
    }


    public void CollectCoinsSpinWheel()
    {
        //DailySpinController.Instance.SetHaveSpin();
        MPlayer.AddCoins(coins);
        DailySpinController.Instance.CloseSpinGame();
    }
}

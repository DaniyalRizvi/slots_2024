using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Mkey;

public class LevelUpPopUpsController : MonoBehaviour
{
    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }

    [SerializeField] private Text MaxBet;
    [SerializeField] private Text Coins;

    [SerializeField] private PopUpsController MegaVoucherPU;
    [SerializeField] private PopUpsController LevelUnlockPU;

    [SerializeField] private GameObject Display;

    private int totalBet;
    private int[] levelLocks = { 0, 2, 4, 6, 8, 10, 12, 16, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120, 130, 145, 160, 175, 190, 205, 220, 235, 250, 265, 280 };

    private void Start()
    {
        SlotControls slotControls = FindObjectOfType<SlotControls>();
        slotControls.UpdateMaxMetLine();

        int maxLineBet = slotControls.GetMaxMetLine();

        //int betGoingOn = slotControls.TotalBet;
        //int singleLineBet = betGoingOn / slotControls.LineBet;
        //int totalBet = singleLineBet * maxLineBet;

        totalBet = slotControls.TotalBetCalculator(maxLineBet);

        MaxBet.text = string.Format("{0:#,0}", totalBet);
        Coins.text = string.Format("{0:#,0}", totalBet*2);

        slotControls.RefreshBetLines();
    }

    public void ShowLevelUnlockOrMegaVoucher()
    {
        bool showLevelUnlockPU = false;
        Display.SetActive(false);

        for (int i = 0; i < levelLocks.Length; i++)
        {
            if (MPlayer.Level == levelLocks[i])
            {
                showLevelUnlockPU = true;
                break;
            }
            else if (MPlayer.Level < levelLocks[i])
                break;
        }

        if(showLevelUnlockPU)
        {
            ShowLevelUnlockScreen();
        }
        else
        {
            ShowMegaVoucherScreen();
        }
    }

    private async void ShowLevelUnlockScreen()
    {
        await Task.Delay(2000);
        GetComponent<ShowGuiPopUp>().ShowPopUp(LevelUnlockPU);
        GetComponent<WarningMessController>().CloseWindow();
    }
    private async void ShowMegaVoucherScreen()
    {
        float rand = Random.Range(0f, 1f);

        if (rand > 0.4)
        {
            //print("MEGAAAA");
            await Task.Delay(2000);
            GetComponent<ShowGuiPopUp>().ShowPopUp(MegaVoucherPU);
        }
        else
        {
            //print("NO MEGAAAA");
        }
        GetComponent<WarningMessController>().CloseWindow();
    }

    public void CollectCoins()
    {
        MPlayer.AddCoins(totalBet * 2);
    }
}

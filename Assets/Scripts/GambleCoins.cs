using Mkey;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GambleCoins : MonoBehaviour
{
    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }

    [SerializeField] private Image cardImage;
    [SerializeField] private Text winningsText;
    private PopUpsController controller;
    [SerializeField] PopUpsController retryGamble;

    [SerializeField] private Sprite[] BlackCards;
    [SerializeField] private Sprite[] RedCards;

    [SerializeField] private Sprite Heart;
    [SerializeField] private Sprite Diamond;
    [SerializeField] private Sprite Clover;
    [SerializeField] private Sprite Spades;

    [SerializeField] private Sprite WhiteCard;
    [SerializeField] private AudioClip WrongAnswer;
    [SerializeField] private AudioClip GambleBGM;

    [SerializeField] private GameObject Display;

    [SerializeField] private GameObject x2Winning;
    [SerializeField] private GameObject x4Winning;

    private bool canSelect;
    private bool isColor;
    private string chosenOption;
    private string gameSelectedOption;

    private static int winnings;
    private static int initialWinnings;

    private static bool isNotRetryGamble;

    private int retries = 2;

    private void Awake()
    {
        controller = GetComponent<PopUpsController>();
        winningsText.text = MPlayer.WinCoins.ToString("0#,0");
        initialWinnings = MPlayer.WinCoins;
        canSelect = true;
        SoundMaster.Instance.SetMusicAndPlay(GambleBGM);
    }

    private void Start()
    {
        winnings = MPlayer.WinCoins;
    }

    private void Update()
    {
        if(isNotRetryGamble)
        {
            isNotRetryGamble = false;
            FindObjectOfType<SlotController>().HideGambleButton();
            ChangeMusic();
            controller.CloseWindow();
        }
    }

    private IEnumerator RandomOption(Action action)
    {
        int random;
        if(isColor)
        {
            random = UnityEngine.Random.Range(0, 2);

            if (random == 0)
            {
                gameSelectedOption = "red";
                cardImage.sprite = RedCards[UnityEngine.Random.Range(0, RedCards.Length)];
            }
            else
            {
                gameSelectedOption = "black";
                cardImage.sprite = BlackCards[UnityEngine.Random.Range(0, BlackCards.Length)];
            }
        }
        else
        {
            random = UnityEngine.Random.Range(0, 4);
            
            if(random == 0)
            {
                gameSelectedOption = "heart";
                cardImage.sprite = Heart;
            }
            else if(random == 1)
            {
                gameSelectedOption = "spades";
                cardImage.sprite = Spades;
            }
            else if (random == 2)
            {
                gameSelectedOption = "diamond";
                cardImage.sprite = Diamond;
            }
            else
            {
                gameSelectedOption = "clover";
                cardImage.sprite = Clover;
            }
        }

        if (gameSelectedOption != chosenOption)
            SoundMaster.Instance.PlayClip(0, WrongAnswer);
        //cardText.text = gameSelectedOption;
        yield return new WaitForSeconds(1f);
        action();
    }

    private void IsWin()
    {
        Debug.Log(MPlayer.WinCoins);
        if (chosenOption == gameSelectedOption)
        {
            GambleWon();
        }
        else
        {
            GambleLost();
        }
        winningsText.text = winnings.ToString("000,000");
    }

    public void SelectGambleOption(string state)
    {
        if(canSelect)
        {
            canSelect = false;
            ResetStates();

            if (winnings == 0)
            {
                winnings = MPlayer.WinCoins;
            }

            if (state == "black" || state == "red")
                isColor = true;
            else
                isColor = false;

            chosenOption = state;

            StartCoroutine(RandomOption(IsWin));
        }
    }

    private void GambleWon()
    {
        if (isColor)
        {
            winnings *= 2;  //if color
            Instantiate(x2Winning, transform);
        }
        else
        {
            winnings *= 4;          // if suits
            Instantiate(x4Winning, transform);
        }

        canSelect = true;
    }

    private void GambleLost()
    {
        if (retries > 0)
        {
            GetComponent<ShowGuiPopUp>().ShowPopUp(retryGamble);
            retries--;
        }
        else
        {
            RetryGambleNo();
        }
        
        canSelect = true;
    }

    public void RetryGambleNo()
    {
        MPlayer.AddCoins(-initialWinnings);

        winnings = 0;
        initialWinnings = 0;
        isNotRetryGamble = true;
    }

    public void RetryGambleYes()
    {
    }

    public async void CollectWinnings()
    {
        MPlayer.AddCoins(winnings - initialWinnings);
        FirebaseManager.Instance.UpdateTodayCoins(winnings - initialWinnings);
        winnings = 0;

        FindObjectOfType<SlotController>().HideGambleButton();
        ChangeMusic();

        Display.SetActive(false);
        await Task.Delay(2000);

        controller.CloseWindow();
    }
    private void ResetStates()
    {
        //cardText.text = "";
        cardImage.sprite = null;
        chosenOption = "";
        gameSelectedOption = "";
    }

    public void ChangeMusic()
    {
        SoundMaster.Instance.SetDefaultMusicAndPlay();
    }
}

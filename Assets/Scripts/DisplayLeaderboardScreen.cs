using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLeaderboardScreen : MonoBehaviour
{
    FirebaseManager data;

    [SerializeField] private GameObject Table;
    [SerializeField] private GameObject LeaderBoardRow;

    [SerializeField] private Text PlayerID;
    [SerializeField] private Text PlayerHighestLevelRankText;
    [SerializeField] private Text PlayerLevel;

    [SerializeField] private bool isHighestLevelTodayScreen;
    [SerializeField] private bool isHighestLevelYesterdayScreen;
    [SerializeField] private bool isHighestLevelOverallScreen;
    [SerializeField] private bool isMostCoinTodayScreen;
    [SerializeField] private bool isMostCoinYesterdayScreen;
    [SerializeField] private bool isMostCoinOverallScreen;
    [SerializeField] private bool isMostWinningTodayScreen;
    [SerializeField] private bool isMostWinningYesterdayScreen;

    void Start()
    {
        data = FindObjectOfType<FirebaseManager>();


        FetchingDataAccordingToScreen();

    }

    private void FetchingDataAccordingToScreen()
    {
        if (isHighestLevelTodayScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerHighestLevelTodayRank.ToString();
            PlayerLevel.text = data.GetPlayerLevel().ToString();

            InstantiateCoinsLeaderBoard(Table, data.levelTodayList);
        }
        else if(isHighestLevelOverallScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerHighestLevelRank.ToString();
            PlayerLevel.text = data.GetPlayerLevel().ToString();

            InstantiateCoinsLeaderBoard(Table, data.levelList);
        }
        else if (isHighestLevelYesterdayScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerHighestLevelYesterdayRank.ToString();
            PlayerLevel.text = data.GetPlayerLevelYesterday().ToString();

            InstantiateCoinsLeaderBoard(Table, data.levelYesterdayList);
        }
        else if(isMostCoinTodayScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerMostCoinsTodayRank.ToString();
            PlayerLevel.text = data.GetPlayerCoins().ToString();

            InstantiateCoinsLeaderBoard(Table, data.mostCoinsTodayList);
        }
        else if (isMostCoinYesterdayScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerMostCoinsYesterdayRank.ToString();
            PlayerLevel.text = data.GetPlayerMostCoinsYesterday().ToString();

            InstantiateCoinsLeaderBoard(Table, data.mostCoinsYesterdayList);
        }
        else if(isMostCoinOverallScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerMostCoinsRank.ToString();
            PlayerLevel.text = data.GetPlayerCoins().ToString();

            InstantiateCoinsLeaderBoard(Table, data.coinList);
        }
        else if(isMostWinningTodayScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerCoinsTodayRank.ToString();
            PlayerLevel.text = data.GetPlayerCoinsToday().ToString();

            InstantiateCoinsLeaderBoard(Table, data.coinsTodayList);
        }
        else if (isMostWinningYesterdayScreen)
        {
            PlayerID.text = data.playerUserId;
            PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerCoinsTodayYesterdayRank.ToString();
            PlayerLevel.text = data.GetPlayerCoinsYesterday().ToString();

            InstantiateCoinsLeaderBoard(Table, data.coinsTodayYesterdayList);
        }
    }

    private void InstantiateCoinsLeaderBoard(GameObject Table, List<Pair> pair)
    {
        int iterationCount = 1;

        foreach (Pair p in pair)
        {
            
            if (iterationCount > 100)
                break;

            GameObject column = Instantiate(LeaderBoardRow, Table.transform);
            Text[] texts = column.GetComponentsInChildren<Text>();

            if (iterationCount == 1)
            {
                column.GetComponent<LeaderboardRow>().ShowTrophy();
            }

            if (texts.Length >= 2)
            {
                if (p.id == data.playerUserId)
                {
                    //column.GetComponent<Image>().color = new Color(1f, 1f, 0f);
                }
                texts[0].text = p.id;
                texts[1].text = p.value;
                texts[2].text = iterationCount.ToString();
            }
            else
            {
                Debug.LogError("Not enough Text components found under the UI object.");
            }
            iterationCount++;
        }
    }
}

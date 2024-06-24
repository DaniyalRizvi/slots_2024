using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    FirebaseManager data;

    [SerializeField] private GameObject MostCoinsTop3;
    [SerializeField] private GameObject MostLevelsTop3;
    [SerializeField] private GameObject MostCoinsTodayTop3;

    [SerializeField] private GameObject LeaderBoardCoinColumn;
    [SerializeField] private GameObject LeaderBoardLevelColumn;
    [SerializeField] private GameObject LeaderBoardCoinsTodayColumn;

    [SerializeField] private Text PlayerMostCoinsRankText;
    [SerializeField] private Text PlayerHighestLevelRankText;
    [SerializeField] private Text PlayerMostCoinsTodayRankText;

    void Start()
    {
        data = FindObjectOfType<FirebaseManager>();

        PlayerMostCoinsRankText.text = "YOUR CURRENT RANK IS " + data.playerMostCoinsRank.ToString();
        InstantiateLeaderboardTop3(MostCoinsTop3, LeaderBoardCoinColumn, data.coinList);

        PlayerHighestLevelRankText.text = "YOUR CURRENT RANK IS " + data.playerHighestLevelRank.ToString();
        InstantiateLeaderboardTop3(MostLevelsTop3, LeaderBoardLevelColumn, data.levelList);

        PlayerMostCoinsTodayRankText.text = "YOUR CURRENT RANK IS " + data.playerCoinsTodayRank.ToString();
        InstantiateLeaderboardTop3(MostCoinsTodayTop3, LeaderBoardCoinsTodayColumn, data.coinsTodayList);
    }

    private void InstantiateLeaderboardTop3(GameObject Table, GameObject LeaderBoardColumn, List<Pair> pair)
    {
        int iterationCount = 0;
        foreach (Pair p in pair)
        {
            if (iterationCount > 2)
                break;

            GameObject column = Instantiate(LeaderBoardColumn, Table.transform);
            Text[] texts = column.GetComponentsInChildren<Text>();

            if (texts.Length >= 2)
            {
                if (p.id == data.playerUserId)
                {
                    //column.GetComponent<Image>().color = new Color(0.035f, 0f, 1f);
                }
                if(p.id.Length > 12)
                    texts[0].text = p.id.Substring(0, 6) + "..."; 
                else
                    texts[0].text = p.id;
                if (p.value.Length > 6)
                    texts[1].text = p.value.Substring(0, p.value.Length - 6) + "M";
                else
                    texts[1].text = p.value; 
            }
            else
            {
                Debug.LogError("Not enough Text components found under the UI object.");
            }
            iterationCount++;
        }
    }

    void Update()
    {
        
    }
}

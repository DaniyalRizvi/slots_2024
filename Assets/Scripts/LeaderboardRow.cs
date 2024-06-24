using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private GameObject Trophy;

    public void ShowTrophy()
    {
        Trophy.SetActive(true);
    }
}

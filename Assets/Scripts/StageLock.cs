using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class StageLock : MonoBehaviour
{
    private int[] levelLocks = { 0, 2, 4, 6, 8, 10, 12, 16, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120, 130, 145, 160, 175, 190, 205, 220, 235, 250, 265, 280 };
    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }

    [SerializeField] private int levelLock;
    [SerializeField] private GameObject levelLockUI;
    [SerializeField] private AudioClip LockedStageClickSound;
    [SerializeField] private Sprite GlowLock;

    private void Start()
    {
        if (MPlayer.Level >= levelLock)
        {
            GetComponent<Image>().color = new Color(1f, 1f, 1f);
            levelLockUI.gameObject.SetActive(false);
        }
        else if (MPlayer.Level < levelLock) 
        {
            GetComponent<ButtonClickSound>().SetSound(LockedStageClickSound);

            if (CheckForGlowLock())
            {
                levelLockUI.GetComponent<Image>().sprite = GlowLock;
                GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }
    }
    private void Awake()
    {
        levelLockUI.GetComponentInChildren<Text>().text = "Level " + levelLock;
    }

    private bool CheckForGlowLock()
    {
        int lockIndex = 0;
        for(int i = 1; i<levelLocks.Length;i++)
        {
            if (levelLocks[i] == levelLock)
            {
                lockIndex = i;
                break;
            }
        }
        if (MPlayer.Level >= levelLocks[lockIndex - 1])
            return true;
        else
            return false;
    }
    public int GetLevelLock()
    {
        return levelLock;
    }
}

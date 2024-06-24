using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class UnlockLevel : MonoBehaviour
{
    [SerializeField] private Image LevelImage;

    [SerializeField] private Sprite[] LevelBGs;

    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
    void Start()
    {
        int level = MPlayer.Level;

        SetLevelImage(level);
    }

    void SetLevelImage(int level)
    {
        switch(level)
        {
            case 2:
                LevelImage.sprite = LevelBGs[0];
                break;
            case 4:
                LevelImage.sprite = LevelBGs[1];
                break;
            case 6:
                LevelImage.sprite = LevelBGs[2];
                break;
            case 8:
                LevelImage.sprite = LevelBGs[3];
                break;
            case 10:
                LevelImage.sprite = LevelBGs[4];
                break;
            case 12:
                LevelImage.sprite = LevelBGs[5];
                break;
            case 16:
                LevelImage.sprite = LevelBGs[6];
                break;
            case 20:
                LevelImage.sprite = LevelBGs[7];
                break;
            case 25:
                LevelImage.sprite = LevelBGs[8];
                break;
            case 30:
                LevelImage.sprite = LevelBGs[9];
                break;
            case 35:
                LevelImage.sprite = LevelBGs[10];
                break;
            case 40:
                LevelImage.sprite = LevelBGs[11];
                break;
            case 45:
                LevelImage.sprite = LevelBGs[12];
                break;
            case 50:
                LevelImage.sprite = LevelBGs[13];
                break;
            case 55:
                LevelImage.sprite = LevelBGs[14];
                break;
            case 60:
                LevelImage.sprite = LevelBGs[15];
                break;
            case 65:
                LevelImage.sprite = LevelBGs[16];
                break;
            case 70:
                LevelImage.sprite = LevelBGs[17];
                break;
            case 75:
                LevelImage.sprite = LevelBGs[18];
                break;
            case 80:
                LevelImage.sprite = LevelBGs[19];
                break;
            case 85:
                LevelImage.sprite = LevelBGs[20];
                break;
            case 90:
                LevelImage.sprite = LevelBGs[21];
                break;
            case 95:
                LevelImage.sprite = LevelBGs[22];
                break;
            case 100:
                LevelImage.sprite = LevelBGs[23];
                break;
            case 110:
                LevelImage.sprite = LevelBGs[24];
                break;
            case 120:
                LevelImage.sprite = LevelBGs[25];
                break;
            case 130:
                LevelImage.sprite = LevelBGs[26];
                break;
            case 145:
                LevelImage.sprite = LevelBGs[27];
                break;
            case 160:
                LevelImage.sprite = LevelBGs[28];
                break;
            case 175:
                LevelImage.sprite = LevelBGs[29];
                break;
            case 190:
                LevelImage.sprite = LevelBGs[30];
                break;
            case 205:
                LevelImage.sprite = LevelBGs[31];
                break;
            case 220:
                LevelImage.sprite = LevelBGs[32];
                break;
            case 235:
                LevelImage.sprite = LevelBGs[33];
                break;
            case 250:
                LevelImage.sprite = LevelBGs[34];
                break;
            case 265:
                LevelImage.sprite = LevelBGs[35];
                break;
            case 280:
                LevelImage.sprite = LevelBGs[36];
                break;
        }
    }
}

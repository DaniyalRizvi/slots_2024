using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
   29.04.2021
*/
namespace Mkey
{
    public class DailyRewardLine : MonoBehaviour
    {
        [SerializeField]
        private int gameDayNumber = 0;
        [SerializeField]
        private int controlledDaysCount = 7;

        [SerializeField] private Image CoinImage;
        [SerializeField] private Image CoinPlate;
        [SerializeField] private Sprite OldCoin;
        [SerializeField] private Sprite OldCoinPlate;

        [SerializeField] private GameObject ClaimButton;
        [SerializeField] private GameObject ClaimCaption;

        public static DailyRewardLine CurrentLine => currentLine;

        #region events
        [SerializeField]
        private UnityEvent StartCurrentRewardDayEvent;
        [SerializeField]
        private UnityEvent StartOldRewardDayEvent;
        [SerializeField]
        private UnityEvent StartNextRewardDayEvent;
        [SerializeField]
        private UnityEvent ApplyRewardEvent;
        [SerializeField]
        private UnityEvent <string> UpdateDayNumberText;
        #endregion events

        #region temp vars
        private DailyRewardController DRC => DailyRewardController.Instance;
        private int rewDay = -1;
        private static DailyRewardLine currentLine;
        #endregion temp vars

        private void Start()
        {
            rewDay = DRC.RewardDay;
            int claimedDay = PlayerPrefs.GetInt("ClaimedDay", 0);
            if (claimedDay >= gameDayNumber && claimedDay != rewDay)
            {
                OldRewardApply();
            }

            if (rewDay < 0) return;
            int rewDayCl = DRC.RepeatingReward ? rewDay % controlledDaysCount : Mathf.Clamp(rewDay, 0, controlledDaysCount - 1);

            // raise events
            if (gameDayNumber == rewDayCl)
            {
                PlayerPrefs.SetInt("ClaimedDay", gameDayNumber);
                ClaimButton.SetActive(true);
                ClaimCaption.SetActive(true);
                currentLine = this;
                StartCurrentRewardDayEvent?.Invoke();
            }
            else if (gameDayNumber < rewDayCl)
            {
                StartOldRewardDayEvent?.Invoke();
            }
            else if (gameDayNumber > rewDayCl)
            {
                StartNextRewardDayEvent?.Invoke();
            }

            if (DRC.RepeatingReward) UpdateDayNumberText?.Invoke((rewDay + gameDayNumber - rewDayCl + 1).ToString());
        }

        public void Apply()
        {
            DRC.ApplyReward();
            ClaimButton.SetActive(false);
            ClaimCaption.SetActive(false);
            ApplyRewardEvent?.Invoke();
        }

        public void CurrentRewardApply()
        {
           if(CurrentLine) CurrentLine.Apply();
        }

        public void OldRewardApply()
        {
            CoinImage.sprite = OldCoin;
            CoinPlate.sprite = OldCoinPlate;
        }
    }
}

/*mkutils*/
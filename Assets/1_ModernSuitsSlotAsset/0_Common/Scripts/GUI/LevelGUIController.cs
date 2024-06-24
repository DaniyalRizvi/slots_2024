using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 26.05.2021
 */
namespace Mkey
{
    public class LevelGUIController : MonoBehaviour
    {
        [SerializeField]
        private Text LevelNumberText;
        [SerializeField]
        private ProgressSlider progressSlider;
        [SerializeField]
        private ProgressSlider progressTurboSlider;
        [SerializeField]
        private ProgressSlider progressNormalSlider;
        [SerializeField]
        private WarningMessController LevelUpCongratulationPrefab;
        [SerializeField]
        private Text levelProgressPercentage;

        [SerializeField]
        private string levelNumberPrefix;

        [SerializeField] private bool noShowLevelUpPopUp;

        #region temp vars
        private int levelTweenId;
        private float levelxp;
        private float oldLevelxp;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private GuiController MGui { get { return GuiController.Instance; } }
        private static bool gameStarted = false;
        private static int level;
        #endregion temp vars

        #region regular
        private void Start()
        {
            StartCoroutine(StartC());
        }

        private IEnumerator StartC()
        {
            while (!MPlayer)
            {
                yield return new WaitForEndOfFrame();
            }
            MPlayer.ChangeLevelProgressEvent += ChangeLevelProgressHandler;
            MPlayer.ChangeLevelEvent += ChangeLevelHandler;

            if (PlayerPrefs.GetInt("2xXP!", 1) == 1)
                ChangeToNormalSlider();
            else
                ChangeToTurboSlider();

                //RefreshLevel();

            if (!gameStarted)
            {
                level = MPlayer.Level;
                gameStarted = true;
            }
            else // in lobby behavior
            {
                if (level < MPlayer.Level && MPlayer.UseLevelUpReward && MPlayer.LevelUpReward > 0 && !noShowLevelUpPopUp)
                {
                    ShowLevelRewardPopUp(MPlayer.Level, MPlayer.LevelUpReward * (MPlayer.Level - level));
                }
                level = MPlayer.Level;
            }
        }

        private void OnDestroy()
        {
            if (MPlayer) MPlayer.ChangeLevelProgressEvent -= ChangeLevelProgressHandler;
            if (MPlayer) MPlayer.ChangeLevelEvent -= ChangeLevelHandler;
        }
        #endregion regular

        /// <summary>
        /// Refresh gui level
        /// </summary>
        private void RefreshLevel()
        {
            
            SimpleTween.Cancel(levelTweenId, false);
            if (MPlayer)
            {
                if (progressSlider)
                {
                    levelxp = MPlayer.LevelProgress;
                    levelProgressPercentage.text = ((int)levelxp).ToString() + "%";
                    if (levelxp > oldLevelxp)
                    {
                        levelTweenId = SimpleTween.Value(gameObject, oldLevelxp, levelxp, 0.3f).SetOnUpdate((float val) =>
                        {
                            oldLevelxp = val;
                            progressSlider.SetFillAmount(oldLevelxp / 100f);
                        }).ID;
                    }
                    else
                    {
                        progressSlider.SetFillAmount(levelxp / 100f);
                        oldLevelxp = levelxp;
                    }
                }
                if (LevelNumberText) LevelNumberText.text = levelNumberPrefix + MPlayer.Level.ToString();
            }
        }

        public void ChangeToTurboSlider()
        {
            progressSlider = progressTurboSlider;
            progressNormalSlider.gameObject.SetActive(false);
            progressTurboSlider.gameObject.SetActive(true);

            RefreshLevel();
        }
        public void ChangeToNormalSlider()
        {
            progressSlider = progressNormalSlider;
            progressTurboSlider.gameObject.SetActive(false);
            progressNormalSlider.gameObject.SetActive(true);

            RefreshLevel();
        }

        #region eventhandlers
        private void ChangeLevelHandler(int newLevel, long reward, bool useLevelReward)
        {
            if (this)
            {
                RefreshLevel();
                if (useLevelReward && reward > 0 && !noShowLevelUpPopUp) ShowLevelRewardPopUp(newLevel, reward);
            }
        }

        private void ChangeLevelProgressHandler(float newProgress)
        {
            if (this) RefreshLevel();
          //  Debug.Log("progress: " + newProgress);
        }
        #endregion eventhandlers

        private void ShowLevelRewardPopUp(int newLevel, long reward)
        {
            MGui.ShowMessageWithYesNoCloseButton(LevelUpCongratulationPrefab, reward.ToString(), newLevel.ToString(), () => { MPlayer.AddCoins((int)reward); }, null, null);
        }
    }
}

/*mkutils*/
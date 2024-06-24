using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/*
    30.10.2020 - first
    11.01.2021 - improve sound
 */

namespace Mkey
{
    public class ShowRewardedAd : MonoBehaviour
    {
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }

        [SerializeField]
        private string adName = "default";
        [SerializeField]
        private UnityEvent CompleteEvent;
        [SerializeField]
        private UnityEvent FailedEvent;

        #region temp vars
        private AdsControl Ads => AdsControl.Instance;
        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        #endregion temp vars

        public void Show()
        {
            if (Ads) 
            {
                Ads.ShowRewardedAd(adName, () => { MSound.ForceStopMusic() ; }, () => { MSound.PlayCurrentMusic(); }, // disable music and sounds while ads, restore sounds

                      (result, mess, amount) =>
                      {
                          if (result)
                          {
                              CompleteEvent?.Invoke();
                          }
                          else
                          {
                              FailedEvent?.Invoke();
                          }
                      });
            }
        }

        public void BuyCoins()
        {
            if (Ads)
            {
                Ads.ShowRewardedAd(adName, () => { MSound.ForceStopMusic(); }, () => { MSound.PlayCurrentMusic(); }, // disable music and sounds while ads, restore sounds

                      (result, mess, amount) =>
                      {
                          if (result)
                          {
                              CompleteEvent?.Invoke();
                          }
                          else
                          {
                              FailedEvent?.Invoke();
                          }
                      });
                MPlayer.AddCoins(20000* (MPlayer.Level+1));

            }
        }

        public void WonCoins(int coins)
        {
            PlayerPrefs.SetInt("WonCoins", coins * (MPlayer.Level + 1));
        }
    }
}
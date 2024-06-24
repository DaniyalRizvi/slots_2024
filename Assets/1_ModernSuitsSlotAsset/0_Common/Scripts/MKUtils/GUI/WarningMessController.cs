using UnityEngine.UI;
using UnityEngine;
using TMPro;
/*
 *changes
 * 02102019 Fix
 *      -if (yesButton) yesButton.gameObject.SetActive(yesButtonActive);
        -if (cancelButton) cancelButton.gameObject.SetActive(cancelButtonActive);
        -if (noButton) noButton.gameObject.SetActive(noButtonActive);
 */
namespace Mkey
{
    public enum MessageAnswer { None , Yes, Cancel, No }
    public class WarningMessController : PopUpsController
    {
        public Text caption;
        public TMPro.TMP_Text message;
        public Button yesButton;
        public Button noButton;
        public Button cancelButton;

        public MessageAnswer Answer
        {
            get; private set;
        }

        public void Cancel_Click()
        {
            Answer = MessageAnswer.Cancel;
            CloseWindow();
        }

        public void Yes_Click()
        {
            Answer = MessageAnswer.Yes;
            CloseWindow();
        }

        public void No_Click()
        {
            Answer = MessageAnswer.No;
            CloseWindow();
        }

        public void FreeSpinsYes_Click()
        {
            PlayerPrefs.SetInt("FreeSpins!", 1);
            FindObjectOfType<LobbyController>().FreeSpinSceneLoad(1);
            CloseWindow();
        }

        public void LevelXPBonusYes_Click()
        {
            PlayerPrefs.SetInt("2xXP!", 2);
            FindObjectOfType<LevelGUIController>().ChangeToTurboSlider();
            //FindObjectOfType<LobbyController>().FreeSpinSceneLoad(1);
        }

        public void CoinBonusYes_Click()
        {
            PlayerPrefs.SetInt("2xCoin!", 2);

            AllWinsMultiplied wins = FindObjectOfType<AllWinsMultiplied>();
            if (wins != null)
            {
                wins.ShowIndicator();
            }

            //FindObjectOfType<LobbyController>().FreeSpinSceneLoad(1);
        }

        public void SpinWheelAgainYes_Click()
        {
            DailySpinController.Instance.SetHaveSpin();
            CloseWindow();
        }

        public void SpinWheelAgainNo_Click()
        {
            DailySpinController.Instance.CloseSpinGame();
        }
        
        public void SpeedUpSpin()
        {
            DailySpinController.Instance.SpeedUpButtonPressed();
        }

        public void OpenSpinWheel()
        {
            FindObjectOfType<DailySpinController>().OpenSpinGame();
        }
        public void ShowLevelUnlockOrMegaVoucher()
        {

        }

        public void ShowMegaVoucherScreen()
        {
            float rand = Random.Range(0f, 1f);
            
            if (rand > 0.4)
            {
                //print("MEGAAAA");
                GetComponent<ShowRandomGuiPopUp>().ShowRandomPopUp();
            }
            else
            {
                //print("NO MEGAAAA");
            }
        }

        public string Caption
        {
            get { if (caption) return caption.text; else return string.Empty; }
            set { if (caption) caption.text = value; }
        }

        public string Message
        {
            get { if (message) return message.text; else return string.Empty; }
            set { if (message) message.text = value; }
        }

        internal void SetMessage(string caption, string message, bool yesButtonActive, bool cancelButtonActive, bool noButtonActive)
        {
            Caption = caption;
            Message = message;
            if (yesButton) yesButton.gameObject.SetActive(yesButtonActive);
            if (cancelButton) cancelButton.gameObject.SetActive(cancelButtonActive);
            if (noButton) noButton.gameObject.SetActive(noButtonActive);
        }
    }
}
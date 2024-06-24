using UnityEngine;

/*
    31.10.2020 
    18.04.2021 
        - add  failedMessagePrefab
 */
namespace Mkey
{
    public class PurchaseEvents : MonoBehaviour
    {
        [SerializeField]
        private WarningMessController goodMessagePrefab;
        [SerializeField]
        private WarningMessController failedMessagePrefab;

        [SerializeField]
        private bool showGoodMessage = true;
        [SerializeField]
        private bool showFailedMessage = true;

        private GuiController MGui => GuiController.Instance;

        private SlotPlayer MPlayer => SlotPlayer.Instance;
        private Purchaser MPurchaser => Purchaser.Instance;

        #region regular
        private void Start()
        {
            if (MPurchaser)
            {
               if(showGoodMessage) MPurchaser.GoodPurchaseEvent += GoodPurchaseMessage;
               if(showFailedMessage) MPurchaser.FailedPurchaseEvent += FailedPurchaseMessage;
            }
        }

        private void OnDestroy()
        {
            if (MPurchaser)
            {
                MPurchaser.GoodPurchaseEvent -= GoodPurchaseMessage;
                MPurchaser.FailedPurchaseEvent -= FailedPurchaseMessage;
            }
        }
        #endregion regular

        public void AddCoins(int count)
        {
            if (MPlayer != null)
            {
                MPlayer.AddCoins(count);
            }
        }

        public void AddGems(int count)
        {
            if (MPlayer != null)
            {
                MPlayer.AddGems(count);
            }
        }

        public void AddVIPPoints(int count)
        {
            if (MPlayer != null)
            {
                MPlayer.AddStatusPoints(count);
            }
        }

        public void AddPiggyCoins()
        {
            if (MPlayer != null)
            {
                int coins = PlayerPrefs.GetInt("PiggyCoins", 0);
                MPlayer.AddCoins(coins);
                PlayerPrefs.SetInt("PiggyCoins", 0);
            }
        }

        public void SetCoins(int coins)
        {
            PlayerPrefs.SetInt("WonCoins", coins);
        }

        internal void GoodPurchaseMessage(string prodId, string prodName)
        {
            if (MGui && goodMessagePrefab)
            {
                MGui.ShowMessageWithYesNoCloseButton(goodMessagePrefab, null, prodName + " purchased successful.", () => { }, null, null);
            }
            else if (MGui)
            {
                MGui.ShowMessageWithYesNoCloseButton("Succesfull!!!", prodName + " purchased successful.", () => { }, null, null);
            }
        }

        internal void FailedPurchaseMessage(string prodId, string prodName)
        {
            if (MGui && failedMessagePrefab)
            {
                MGui.ShowMessageWithYesNoCloseButton(failedMessagePrefab, null, prodName + " - purchase failed.", () => { }, null, null);
            }
            else if (MGui)
            {
                MGui.ShowMessageWithYesNoCloseButton("Sorry.", prodName + " - purchase failed.", () => { }, null, null);
            }
        }

        internal void GoodPurchaseMessage(string message)
        {
            if (MGui)
            {
                MGui.ShowMessage("Succesful!!!", message, 3, null);
            }
        }

        internal void FailedPurchaseMessage(string message)
        {
            if (MGui)
            {
                MGui.ShowMessage("Sorry.", message, 3, null);
            }
        }
    }
}
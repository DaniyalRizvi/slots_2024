using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class GemsGUIController : MonoBehaviour
{
    [SerializeField]
    private Text gemAmountText;

    #region temp vars
    private TweenIntValue gemsTween;
    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
    private GuiController MGui { get { return GuiController.Instance; } }
    private string gemsFormat = "0";
    #endregion temp vars

    #region regular
    private IEnumerator Start()
    {
        while (!MPlayer)
        {
            yield return new WaitForEndOfFrame();
        }

        // set player event handlers
        MPlayer.ChangeGemsEvent += ChangeGemsHandler;
        MPlayer.LoadGemsEvent += LoadGemsHandler;
        if (gemAmountText) gemsTween = new TweenIntValue(gemAmountText.gameObject, MPlayer.Gems, 1, 3, true, (b) => { if (this && gemAmountText) gemAmountText.text = (b > 0) ? b.ToString(gemsFormat) : "0"; });
        Refresh();
    }

    private void OnDestroy()
    {
        if (MPlayer)
        {
            // remove player event handlers
            MPlayer.ChangeGemsEvent -= ChangeGemsHandler;
            MPlayer.LoadGemsEvent -= LoadGemsHandler;
        }
    }
    #endregion regular

    /// <summary>
    /// Refresh gui balance
    /// </summary>
    private void Refresh()
    {
        if (gemAmountText && MPlayer) gemAmountText.text = (MPlayer.Gems > 0) ? MPlayer.Gems.ToString(gemsFormat) : "0";
    }

    #region eventhandlers
    private void ChangeGemsHandler(int newBalance)
    {
        if (gemsTween != null) gemsTween.Tween(newBalance, 100);
        else
        {
            if (gemAmountText) gemAmountText.text = (newBalance > 0) ? newBalance.ToString(gemsFormat) : "0";
        }
    }

    private void LoadGemsHandler(int newBalance)
    {
        if (gemAmountText) gemAmountText.text = (newBalance > 0) ? newBalance.ToString(gemsFormat) : "0";
    }
    #endregion eventhandlers
}

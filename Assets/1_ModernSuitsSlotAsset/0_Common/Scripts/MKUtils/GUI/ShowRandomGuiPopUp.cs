using UnityEngine;
using System.Collections.Generic;

/*
    01.02.2021 
 */

namespace Mkey
{
	public class ShowRandomGuiPopUp : MonoBehaviour
	{
        [SerializeField]
        private List<PopUpsController> popUps;

        private int popUpsIndex = 0;

        #region temp vars
        protected static GuiController mGui;
        #endregion temp vars

        public void ShowRandomPopUp()
        {
            if (mGui == null) mGui = FindObjectOfType<GuiController>();
            if (mGui && popUps != null && popUps.Count > 0)
            {
                PopUpsController rP = popUps.GetRandomPos();
                if(rP) mGui.ShowPopUp(rP);
            }
        }

        public void ShowPopUpsInOrder()
        {
            if (mGui == null) mGui = FindObjectOfType<GuiController>();
            if (mGui && popUps != null && popUps.Count > 0)
            {
                if (popUpsIndex >= popUps.Count)
                    popUpsIndex = 0;

                PopUpsController rP = popUps[popUpsIndex];
                if (rP)
                {
                    if (rP.GetComponent<EventType>().eventType == Event.Event2xXP && PlayerPrefs.GetInt("2xXP!", 1) == 2)
                    {
                        popUpsIndex++;
                        ShowPopUpsInOrder();
                    }
                    else  if (rP.GetComponent<EventType>().eventType == Event.Event2xCoins && PlayerPrefs.GetInt("2xCoin!", 1) == 2)
                    {
                        popUpsIndex++;
                        ShowPopUpsInOrder();
                    }
                    else
                    {
                        mGui.ShowPopUp(rP);
                        popUpsIndex++;
                    }
                }
            }
        }
    }
}

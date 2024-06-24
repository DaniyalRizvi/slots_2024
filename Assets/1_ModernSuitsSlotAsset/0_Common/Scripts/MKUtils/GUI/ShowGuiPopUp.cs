using System.Collections;
using UnityEngine;

/*
    24.10.2019 - first
    30.06.2020 remove reverence to =GuiController
 */

namespace Mkey
{
	public class ShowGuiPopUp : MonoBehaviour
	{
        #region temp vars
        protected static GuiController mGui;
        #endregion temp vars

        public void ShowPopUp(PopUpsController popUpsController)
        {
            if (!mGui) mGui = FindObjectOfType<GuiController>();
            if (mGui) mGui.ShowPopUp(popUpsController);
        }

        public void ShowPopUpAfterDelay(PopUpsController popUpsController)
        {
            StartCoroutine(Show(popUpsController));
        }

        public IEnumerator Show(PopUpsController popUpsController)
        {
            yield return new WaitForSeconds(0.5f);
            if (!mGui) mGui = FindObjectOfType<GuiController>();
            if (mGui) mGui.ShowPopUp(popUpsController);
        }
    }
}

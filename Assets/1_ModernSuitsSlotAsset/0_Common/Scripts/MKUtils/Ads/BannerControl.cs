using UnityEngine;
using System.Collections;

/*
   15.04.2020- first
   04.01.2021 -
        -avoid duplicate banner
 */

namespace Mkey
{
    public enum BannerState {Show, Hide }
    public class BannerControl : MonoBehaviour
    {
        [SerializeField]
        private BannerState banner = BannerState.Show;
        private SlotMenuController slotMenuController;

        private Vector3 withoutBannerFooterPosition = Vector3.zero;
        private Vector3 withBannerFooterPosition = new Vector3(0f, 129f, 0f); 
        [SerializeField] private float footerUITransitionSpeed = 1f;

        [SerializeField] private Vector3 WithBannerSlotsPosition;

#if ADDGADS
        #region temp vars
        private AdsControl GADS { get { return AdsControl.Instance; } }
        #endregion temp vars

        private IEnumerator Start()
        {
            while (!GADS)
            {
                yield return new WaitForEndOfFrame();
            }


            yield return new WaitForEndOfFrame();

            if (banner == BannerState.Show)
            {
                GADS.HideBanner();
                GADS.ShowBanner();
                //ShowBannerActions();
            }
            else
            {
                GADS.HideBanner();
            }
            //if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "1_Lobby")
            //{
            //    gameObject.SetActive(false);
            //}
        }

        public void ShowBannerActions()
        {
            slotMenuController = FindObjectOfType<SlotMenuController>();
            if (slotMenuController)
            {
                StartCoroutine(slotMenuController.MoveFooterUI(withBannerFooterPosition, footerUITransitionSpeed));
                StartCoroutine(slotMenuController.MoveSlots(WithBannerSlotsPosition, footerUITransitionSpeed));
            }
        }

        public void HideBannerActions()
        {
            slotMenuController = FindObjectOfType<SlotMenuController>();
            if(slotMenuController)
             StartCoroutine(slotMenuController.MoveFooterUI(withoutBannerFooterPosition, footerUITransitionSpeed));
        }

        public BannerState GetBannerState()
        {
            return banner;
        }

#endif
    }
}
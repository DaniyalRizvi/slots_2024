using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class SlotMenuController : MonoBehaviour
    {
        [Space(16, order = 0)]
        [SerializeField]
        private SlotController slot;

        [SerializeField] private RectTransform Footer;

        [SerializeField] private Transform Slots;
        #region temp vars
        private Button[] buttons;
        #endregion temp vars

        #region regular
        void Start()
        {
            buttons = GetComponentsInChildren<Button>(true);
        }
        #endregion regular

        /// <summary>
        /// Set all buttons interactble = activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            if (buttons == null) return;
            foreach (Button b in buttons)
            {
              if(b)  b.interactable = activity;
            }
        }

        private string GetMoneyName(int count)
        {
            if (count > 1) return "coins";
            else return "coin";
        }

        public IEnumerator MoveFooterUI(Vector3 to, float speed)
        {
            var t = 0f;

            while (t < 1f)
            {
                t += speed * Time.deltaTime;
                Footer.anchoredPosition = Vector3.Lerp(Footer.anchoredPosition, to, t);
                yield return null;
            }
        }

        public IEnumerator MoveSlots(Vector3 to, float speed)
        {
            var t = 0f;

            while (t < 1f)
            {
                t += speed * Time.deltaTime;
                Slots.localPosition = Vector3.Lerp(Slots.localPosition, to, t);
                yield return null;
            }
        }
    }
}
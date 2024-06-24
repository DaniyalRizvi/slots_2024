using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Mkey {
    public class LobbyController : MonoBehaviour {

        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }

        #region temp vars
        private Button[] buttons;
        #endregion temp vars

        private void Awake()
        {
        }

        #region regular

        void Start()
        {
            buttons = GetComponentsInChildren<Button>();
        }
        #endregion regular

        public void GetLevelLock()
        {

        }
        public void SceneLoad(int scene)
        {
            GameObject selectedStage = EventSystem.current.currentSelectedGameObject;
            if (selectedStage.GetComponent<StageLock>().GetLevelLock() <= MPlayer.Level)
            {
                SceneLoader.Instance.LoadScene(scene);
            }
        }

        public void FreeSpinSceneLoad(int scene)
        {
            SceneLoader.Instance.LoadScene(scene);
        }

        /// <summary>
        /// Set all buttons interactble = activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            if (buttons == null) return;
            foreach (Button b in buttons)
            {
                if (b) b.interactable = activity;
            }
        }
    }
}
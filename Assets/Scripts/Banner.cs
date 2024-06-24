using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class Banner : MonoBehaviour
{
    Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 99;
        canvas.planeDistance = 20;

        //GameObject slotMenu = FindObjectOfType<SlotMenuController>().gameObject;
        //GameObject slot = FindObjectOfType<SlotController>().gameObject;

        //if (GetComponentInChildren<Image>().sprite != null)
        //    slotMenu.SetActive(false);
        //else
        //    slot.SetActive(false);
    }

    //private void OnDisable()
    //{
    //    BannerControl bannerControl = FindObjectOfType<BannerControl>();
    //    if (bannerControl.GetBannerState() != BannerState.Hide && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "1_Lobby")
    //        bannerControl.HideBannerActions();
    //}

    //private void OnDestroy()
    //{
    //    BannerControl bannerControl = FindObjectOfType<BannerControl>();
    //    if (bannerControl.GetBannerState() != BannerState.Hide)
    //        bannerControl.HideBannerActions();
    //}
}
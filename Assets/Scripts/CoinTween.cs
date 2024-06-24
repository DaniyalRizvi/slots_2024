using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;

public class CoinTween : MonoBehaviour
{
    [SerializeField] private Transform coinTarget;
    [SerializeField] private ParticleSystem CoinEffect;
    public bool showEffect = false;

    [SerializeField] private bool isDiamond;
    void Start()
    {
        if(!isDiamond)
            coinTarget = FindObjectOfType<BalanceGUIController>().coinTarget.transform;
        else
            coinTarget = FindObjectOfType<LevelGUIController>().transform;

        SimpleTween.Move(gameObject, transform.position, coinTarget.position, 1f);
    }

    private void Update()
    {
        if (transform.position == coinTarget.position)
        {
            if (showEffect)
                Instantiate(CoinEffect,transform.position,Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

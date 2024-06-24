using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Mkey;

public class CoinCollectHelper : MonoBehaviour
{
    [SerializeField] private GameObject CoinPrefab;
    private Transform Parent;

    private void Start()
    {
        Parent = FindObjectOfType<GuiController>().transform;
    }
    public async void SpawnCoins(int numberOfCoins)
    {
        for (int i = 0; i < numberOfCoins; i++) 
        {
            GameObject coin = Instantiate(CoinPrefab, Parent);

            if (i == numberOfCoins - 1)
                coin.GetComponent<CoinTween>().showEffect = true;

            await Task.Delay(80);

        }
    }

    public async void SpawnCoins(Vector3 Position,int numberOfCoins)
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            GameObject coin = Instantiate(CoinPrefab, Parent);
            coin.transform.position = Position;

            if (i == numberOfCoins - 1)
                coin.GetComponent<CoinTween>().showEffect = true;

            await Task.Delay(80);

        }
    }
}

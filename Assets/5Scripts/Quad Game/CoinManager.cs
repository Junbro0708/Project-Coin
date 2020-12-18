using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    float currentTime;

    float minTime = 1;
    float maxTime = 5;

    public float createTime;

    public GameObject coinB;
    public GameObject coinS;
    public GameObject coinG;

    void Start()
    {
        createTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        BronzeCoin();
        SilverCoin();
        GoldCoin();
    }

    void BronzeCoin()
    {
        int xPos = Random.Range(-85, 85);
        int zPos = Random.Range(-45, 45);

        currentTime += Time.deltaTime;

        if (currentTime > createTime)
        {
            Instantiate(coinB, new Vector3(xPos, 3, zPos), Quaternion.identity);

            currentTime = 0;

            createTime = Random.Range(minTime, maxTime);
        }
    }
    void SilverCoin()
    {
        int xPos = Random.Range(-85, 85);
        int zPos = Random.Range(-45, 45);

        currentTime += Time.deltaTime;

        if (currentTime > createTime)
        {
            Instantiate(coinS, new Vector3(xPos, 3, zPos), Quaternion.identity);

            currentTime = 0;

            createTime = Random.Range(minTime, maxTime);
        }
    }
    void GoldCoin()
    {
        int xPos = Random.Range(-85, 85);
        int zPos = Random.Range(-45, 45);

        currentTime += Time.deltaTime;

        if (currentTime > createTime)
        {
            Instantiate(coinG, new Vector3(xPos, 3, zPos), Quaternion.identity);

            currentTime = 0;

            createTime = Random.Range(minTime, maxTime);
        }
    }
}
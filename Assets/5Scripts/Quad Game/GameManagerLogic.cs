using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerLogic : MonoBehaviour
{
    public Text mycoin;
    public Player player;
    public GameObject menuPanel;
    public GameObject gamePlayer;
    public GameObject rivalPlayer;
    public GameObject coinImage;
    public GameObject myScore;
    public GameObject rivalScore;

    public void Update()
    {
        mycoin.text = player.coin.ToString();
    }

    public void GameStart()
    {
        menuPanel.SetActive(false);
        gamePlayer.SetActive(true);
        rivalPlayer.SetActive(true);
        coinImage.SetActive(true);
        myScore.SetActive(true);
        rivalScore.SetActive(true);
    }
}

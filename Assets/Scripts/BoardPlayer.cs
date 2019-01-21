using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardPlayer : MonoBehaviour {

    public PlayerColor playerColor;
    public Image playerImage;
    public TMP_Text playerName;
    public TMP_Text playerCount;

    public Light spot;

    private void OnEnable()
    {
        playerName.text = GameController.GetPlayerData(playerColor).playerName;
        UpdateCount();
    }

    public void UpdateCount()
    {
        playerCount.text = GameController.GetPlayerData(playerColor).numPigs.ToString();
    }

    public void OnActivePlayer()
    {
        spot.transform.DOMove(transform.position, 0.3f);
    }
}

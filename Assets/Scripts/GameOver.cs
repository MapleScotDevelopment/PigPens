using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : Dialog
{
    public Image playerImage;
    public TMP_Text playerName;

    public Sprite pinkPig;
    public Sprite yellowPig;
    public Sprite bluePig;
    public Sprite greenPig;
    public Sprite borgPig;

    private CanvasGroup cg;
    private float showY = 0f;
    private float hiddenY = -25f;

    public Canvas TitleCanvas;
    private TitleZoom titleZoom;

    // Use this for initialization
    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        DialogManager.RegisterDialog("GameOver", this);
        titleZoom = TitleCanvas.GetComponent<TitleZoom>();
    }

    private void SetPlayer()
    {
        playerName.text = GameController.GetPlayerData(GameController.GetCurrentPlayer()).playerName;
        if (playerImage.transform.localRotation.eulerAngles.y != 0)
            playerImage.transform.Rotate(0, -180, 0);
        switch (GameController.GetCurrentPlayer())
        {
            case PlayerColor.BLANK:
                break;
            case PlayerColor.PINK:
                playerImage.sprite = pinkPig;
                break;
            case PlayerColor.YELLOW:
                playerImage.sprite = yellowPig;
                break;
            case PlayerColor.BLUE:
                playerImage.sprite = bluePig;
                break;
            case PlayerColor.GREEN:
                playerImage.sprite = greenPig;
                break;
            case PlayerColor.COMPUTER1:
                playerImage.sprite = borgPig;
                break;
            case PlayerColor.COMPUTER2:
                playerImage.sprite = borgPig;
                playerImage.transform.Rotate(0, 180, 0);
                break;
        }

        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
    }

    public override void Show()
    {
        this.transform.SetAsLastSibling();
        cg.DOKill();
        cg.blocksRaycasts = true;
        cg.interactable = true;
        //cg.DOFade(1f, 0.3f);
        Tweener tweener = transform.DOMoveY(showY, 0.3f);
        tweener.SetEase(Ease.InBounce);

        titleZoom.ZoomIn();
        SetPlayer();
    }

    public override void Hide()
    {
        cg.DOKill();
        cg.interactable = false;
        //cg.DOFade(0f, 0.3f).OnComplete(() => cg.blocksRaycasts = false);
        Tweener tweener = transform.DOMoveY(hiddenY, 0.3f);
        tweener.SetEase(Ease.OutBounce);

        titleZoom.ZoomOut();
    }

    public override void HandleKeyPress(KeyCode k)
    {
        if (cg.interactable)
            OnMenu();
    }

    public void OnMenu()
    {
        GameController.StopGame();
    }
}

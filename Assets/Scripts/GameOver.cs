using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOver : Dialog, IThemedObject
{
    public Image playerImage;
    public TMP_Text playerName;

    public string pinkPig;
    public string yellowPig;
    public string bluePig;
    public string greenPig;
    public string borgPig;

    private Sprite pinkPigSprite;
    private Sprite yellowPigSprite;
    private Sprite bluePigSprite;
    private Sprite greenPigSprite;
    private Sprite borgPigSprite;

    private CanvasGroup cg;
    private float showY = 0f;
    private float hiddenY = -25f;

    public Canvas TitleCanvas;
    private TitleZoom titleZoom;

    private ThemeController themeController;

    // Use this for initialization
    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        DialogManager.RegisterDialog("GameOver", this);
        titleZoom = TitleCanvas.GetComponent<TitleZoom>();
    }

    private void Start()
    {
        // register with the theme controller
        themeController = ThemeController.Instance;
        themeController.RegisterThemedObjectHandler(this);
    }

    public void ThemeChanged()
    {
        pinkPigSprite = themeController.GetThemedObject<Sprite>(pinkPig);
        yellowPigSprite = themeController.GetThemedObject<Sprite>(yellowPig);
        bluePigSprite = themeController.GetThemedObject<Sprite>(bluePig);
        greenPigSprite = themeController.GetThemedObject<Sprite>(greenPig);
        borgPigSprite = themeController.GetThemedObject<Sprite>(borgPig);
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
                playerImage.sprite = pinkPigSprite;
                break;
            case PlayerColor.YELLOW:
                playerImage.sprite = yellowPigSprite;
                break;
            case PlayerColor.BLUE:
                playerImage.sprite = bluePigSprite;
                break;
            case PlayerColor.GREEN:
                playerImage.sprite = greenPigSprite;
                break;
            case PlayerColor.COMPUTER1:
                playerImage.sprite = borgPigSprite;
                break;
            case PlayerColor.COMPUTER2:
                playerImage.sprite = borgPigSprite;
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
        EventSystem.current.SetSelectedGameObject(null);
        GameController.StopGame();
    }
}

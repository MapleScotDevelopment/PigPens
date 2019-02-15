using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PennedPig : MonoBehaviour, IThemedObject {

    PlayerColor current = PlayerColor.BLANK;

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

    public AudioSource pigGrunting;

    ParticleSystem particles;
    Image image;

    private ThemeController themeController;

    // Use this for initialization
    void Awake () {
        particles = GetComponentInParent<ParticleSystem>();
        image = GetComponentInParent<Image>();
        pigGrunting = GetComponentInParent<AudioSource>();
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
        SetPigColor(current, false);
    }

    public void SetPigColor(PlayerColor c, bool inGame)
    {
        current = c;
        if (c == PlayerColor.BLANK)
        {
            image.enabled = false;
            return;
        }
        var main = particles.main;
        main.startColor = ColorValues.GetColorForPlayer(c);

        if (image.transform.localRotation.eulerAngles.y != 0)
            image.transform.Rotate(0, -180, 0);
        switch (c)
        {
            case PlayerColor.PINK:
                image.enabled = true;
                image.sprite = pinkPigSprite;
                break;
            case PlayerColor.YELLOW:
                image.enabled = true;
                image.sprite = yellowPigSprite;
                break;
            case PlayerColor.BLUE:
                image.enabled = true;
                image.sprite = bluePigSprite;
                break;
            case PlayerColor.GREEN:
                image.enabled = true;
                image.sprite = greenPigSprite;
                break;
            case PlayerColor.COMPUTER1:
                image.enabled = true;
                image.sprite = borgPigSprite;
                break;
            case PlayerColor.COMPUTER2:
                image.enabled = true;
                image.sprite = borgPigSprite;
                image.transform.Rotate(0, 180, 0);
                break;
        }

        if (inGame)
        {
            particles.Play();
            pigGrunting.Play();
        }
    }
}

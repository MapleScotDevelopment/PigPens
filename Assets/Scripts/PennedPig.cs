using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PennedPig : MonoBehaviour {

    public Sprite pinkPig;
    public Sprite yellowPig;
    public Sprite bluePig;
    public Sprite greenPig;
    public Sprite borgPig;

    public AudioSource pigGrunting;

    ParticleSystem particles;
    Image image;

	// Use this for initialization
	void Awake () {
        particles = GetComponentInParent<ParticleSystem>();
        image = GetComponentInParent<Image>();
        pigGrunting = GetComponentInParent<AudioSource>();
	}

    public void SetPigColor(PlayerColor c)
    {
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
                image.sprite = pinkPig;
                break;
            case PlayerColor.YELLOW:
                image.enabled = true;
                image.sprite = yellowPig;
                break;
            case PlayerColor.BLUE:
                image.enabled = true;
                image.sprite = bluePig;
                break;
            case PlayerColor.GREEN:
                image.enabled = true;
                image.sprite = greenPig;
                break;
            case PlayerColor.COMPUTER1:
                image.enabled = true;
                image.sprite = borgPig;
                break;
            case PlayerColor.COMPUTER2:
                image.enabled = true;
                image.sprite = borgPig;
                image.transform.Rotate(0, 180, 0);
                break;
        }

        particles.Play();
        pigGrunting.Play();
    }
}

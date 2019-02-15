using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageThemedObjectHandler : ThemedObjectHandler
{
    private Image image;

    override protected void Awake()
    {
        image = GetComponentInParent<Image>();
        base.Awake();
    }

    private void OnEnable()
    {
        ThemeChanged();
    }

    override public void ThemeChanged()
    {
        // theme has changed so get new object name
        themedObject = themeController.GetThemedObject<Sprite>(id);
        if (themedObject != null && themedObject is Sprite)
            image.sprite = (Sprite)themedObject;
    }
}

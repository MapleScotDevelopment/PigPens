using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteThemedObjectHandler : ThemedObjectHandler
{
    private SpriteRenderer spriteRenderer;

    override protected void Awake()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
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
            spriteRenderer.sprite = (Sprite)themedObject;
    }
}

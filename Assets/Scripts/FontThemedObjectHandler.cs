using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class FontThemedObjectHandler : MonoBehaviour, IThemedObject
{
    private TMP_Text tmpPro;
    private ThemeController themeController;

    private void Awake()
    {
        tmpPro = GetComponentInParent<TMP_Text>();
        // register with the theme controller
        themeController = ThemeController.Instance;
        themeController.RegisterThemedObjectHandler(this);
    }

    private void OnEnable()
    {
        ThemeChanged();
    }

    public void ThemeChanged()
    {
        // theme has changed so get new object name
        var font = themeController.GetThemedObject<TMP_FontAsset>("font");
        if (font != null && font is TMP_FontAsset)
            tmpPro.font = font;
    }
}

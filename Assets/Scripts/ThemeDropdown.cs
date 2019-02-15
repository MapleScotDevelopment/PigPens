using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThemeDropdown : MonoBehaviour {

    private TMP_Dropdown dropdown;
    private ThemeController themeController;

    private void Awake()
    {
        // register with the theme controller
        themeController = ThemeController.Instance;
        dropdown = GetComponent<TMP_Dropdown>();
    }

    // Use this for initialization
    void Start ()
    {
        dropdown.captionText.text = I18n.GetText("Themes", "Themes");
        dropdown.AddOptions(themeController.GetThemeNames());
        dropdown.value = themeController.CurrentTheme;
    }

    public void OnThemeSelected()
    {
        themeController.SwitchTheme(dropdown.captionText.text);
    }
}

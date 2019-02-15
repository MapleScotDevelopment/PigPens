using System;
using UnityEngine;

public class ThemedObjectHandler : MonoBehaviour, IThemedObject
{
    public string id;
    protected UnityEngine.Object themedObject;

    protected ThemeController themeController;

    virtual protected void Awake()
    {
        // register with the theme controller
        themeController = ThemeController.Instance;
        themeController.RegisterThemedObjectHandler(this);
    }

    virtual public void ThemeChanged()
    {
    }
}

public interface IThemedObject
{
    void ThemeChanged();
}
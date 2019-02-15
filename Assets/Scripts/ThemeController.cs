using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class ThemeController : MonoBehaviour
{
    public static ThemeController Instance { get; private set; }

    public int CurrentTheme { get; private set; }

    public string CurrentThemeName { get; private set; }

    private const string ORIGINAL_THEME="Original";
    private ThemeData themeData;

    private Theme current=null;
    private Dictionary<string, UnityEngine.Object> cache = new Dictionary<string, UnityEngine.Object>();

    private List<IThemedObject> themedObjectHandlers = new List<IThemedObject>();

    // Use this for initialization
    void Awake ()
    {
        Instance = this;
        CurrentThemeName = ORIGINAL_THEME;
        CurrentTheme = GameController.GetCurrentTheme();
        // load themes
        var themes = Resources.Load<TextAsset>(@"Themes");
        if (themes != null)
        {
            try
            {
                themeData = JsonUtility.FromJson<ThemeData>(themes.text);
                CurrentThemeName = themeData.names[CurrentTheme];
            }
            catch (Exception e)
            {
                // Nothing to do
                return;
            }
            if (HasTheme(CurrentThemeName))
            {
                current = themeData.themes[CurrentTheme];
                CacheTheme();
            }
        }
	}

    public List<String> GetThemeNames()
    {
        List<string> names = new List<string>();
        foreach (string s in themeData.names)
            names.Add(s);
        return names;
    }

    private int GetTheme(string themeName)
    {
        for(int i = 0; i < themeData.themes.Length; i++)
        {
            if (themeData.names[i] == themeName)
                return i;
        }
        return -1;
    }

    private bool HasTheme(string themeName)
    {
        foreach (string s in themeData.names)
            if (s == themeName)
                return true;
        return false;
    }

    public void RegisterThemedObjectHandler(IThemedObject themedObjectHandler)
    {
        themedObjectHandlers.Add(themedObjectHandler);
    }

    public bool ThemesAvailable()
    {
        return current != null;
    }

    public void SwitchTheme(string themeName)
    {
        if (!ThemesAvailable() || !HasTheme(themeName))
            return;

        CurrentTheme = GetTheme(themeName);
        CurrentThemeName = themeName;
        current = themeData.themes[CurrentTheme];
        GameController.SetCurrentTheme(CurrentTheme);

        CacheTheme();

        foreach(IThemedObject handler in themedObjectHandlers)
        {
            handler.ThemeChanged();
        }
    }

    private void CacheTheme()
    {
        cache.Clear();
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>(@"" + CurrentThemeName + "/" + CurrentThemeName);
        foreach (ThemeEntry e in current.entries)
        {
            switch (e.type)
            {
                case "sprite":
                    Sprite o = atlas.GetSprite(e.objectId);
                    if (o != null)
                        cache.Add(e.objectId, o);
                    break;
                case "font":
                    var fontObject = Resources.Load<TMP_FontAsset>(@"" + CurrentThemeName + "/" + e.objectId);
                    if (fontObject != null)
                        cache.Add(e.objectId, fontObject);
                    break;
                case "anim":
                    var animObject = Resources.Load<AnimationClip>(@"" + CurrentThemeName + "/" + e.objectId);
                    if (animObject != null)
                        cache.Add(e.objectId, animObject);
                    break;
            }
        }
    }

    private List<string> GetThemeIds(Theme current)
    {
        List<string> result = new List<string>();
        foreach (ThemeEntry e in current.entries)
        {
            result.Add(e.id);
        }
        return result;
    }

    private List<string> GetThemeObjectIds(Theme current)
    {
        List<string> result = new List<string>();
        foreach (ThemeEntry e in current.entries)
        {
            result.Add(e.objectId);
        }
        return result;
    }

    public T GetThemedObject<T>(string id) where T : UnityEngine.Object
    {
        T result = null;
        if (ThemesAvailable() && ThemeHasId(current, id))
        {
            string objectId = GetObjectId(current, id);
            if (cache[objectId] is T)
                result = cache[objectId] as T;
        }

        return result as T;
    }

    private string GetObjectId(Theme current, string id)
    {
        foreach (ThemeEntry e in current.entries)
        {
            if (e.id == id)
                return e.objectId;
        }
        return null;
    }

    private bool ThemeHasId(Theme current, string id)
    {
        List<string> ids = GetThemeIds(current);
        foreach (string s in ids)
        {
            if (s == id)
                return true;
        }
        return false;
    }
}

using System;
using System.Collections.Generic;

[Serializable]
public class ThemeData
{
    public string[] names;
    public Theme[] themes;
}

[Serializable]
public class Theme
{
    public ThemeEntry[] entries;
}

[Serializable]
public class ThemeEntry
{
    public string id;
    public string type;
    public string objectId;
}
using UnityEngine;

public class ColorValues
{
    private static ColorValues _instance;
    private Color[] playerColors;

    public ColorValues()
    {
        playerColors = new Color[7];
        playerColors[(int)PlayerColor.BLANK] = new Color(0,0,0);
        playerColors[(int)PlayerColor.PINK] = new Color(253,173,218);
        playerColors[(int)PlayerColor.YELLOW] = new Color(255,255,0);
        playerColors[(int)PlayerColor.BLUE] = new Color(170,238,255);
        playerColors[(int)PlayerColor.GREEN] = new Color(229,255,128);
        playerColors[(int)PlayerColor.COMPUTER1] = new Color(32,32,32);
        playerColors[(int)PlayerColor.COMPUTER2] = new Color(45,45,45);
    }

    public static Color GetColorForPlayer(PlayerColor p)
    {
        if (_instance == null)
        {
            _instance = new ColorValues();
        }

        return _instance.playerColors[(int)p];
    }
}
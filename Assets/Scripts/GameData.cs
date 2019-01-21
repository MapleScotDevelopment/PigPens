using System;

[Serializable]
public partial class GameData
{
    public PlayerData[] playerData;
    public BoardData boardData;
    public GameState playState;
    public PlayerColor current;
}

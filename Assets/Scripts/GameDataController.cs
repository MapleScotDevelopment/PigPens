using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class GameDataController
{
    public GameData gameData { get; private set; }
    private const string GameDataSaveFileName = "GameData.json";

    public GameDataController()
    {
        gameData = new GameData();
    }

    public GameState PlayState
    {
        get { return gameData.playState; }
        set { gameData.playState = value;  }
    }

    public PlayerColor CurrentPlayer
    {
        get { return gameData.current;  }
        set { gameData.current = value; }
    }

    public void LoadGameData()
    {
        bool needReset = false;
        InitialisePlayerData();
        try
        {
            string saveFile = Path.Combine(Application.persistentDataPath, GameDataSaveFileName);
            string json = File.ReadAllText(saveFile);
            gameData = JsonUtility.FromJson<GameData>(json);
            if (gameData.playState == GameState.OVER || gameData.playState == GameState.MENU)
                needReset = true;
            else
                gameData.playState = GameState.START;

            if (gameData.playerData.Length == 4)
            {
                PlayerData[] data = new PlayerData[6];
                foreach (PlayerData p in gameData.playerData)
                {
                    p.isBorg = false;
                    data[(int)p.playerColor - 1] = p;
                }
                gameData.playerData = data;
                InitialiseBorgPlayers();
            }
        }
        catch (Exception)
        {
            needReset = true;
        }

        if (needReset)
            ResetGameData();
    }

    public void ResetGameData()
    {
        gameData.playState = GameState.MENU;
        InitialiseBoardData();
    }

    private void InitialiseBoardData()
    {
        gameData.boardData = new BoardData();

        // 4 rows of 5 horizontal fences
        gameData.boardData.horiz = new bool[BoardData.HORIZ_ROWS, BoardData.HORIZ_COLS] {
            { false, false, false, false, false },
            { false, false, false, false, false },
            { false, false, false, false, false },
            { false, false, false, false, false }
        }; ;

        // 3 rows of 6 vertical fences
        gameData.boardData.verts = new bool[BoardData.VERT_ROWS, BoardData.VERT_COLS] {
            { false, false, false, false, false, false },
            { false, false, false, false, false, false },
            { false, false, false, false, false, false }
        };

        // 3 rows of 5 pens
        gameData.boardData.pens = new PlayerColor[BoardData.VERT_ROWS, BoardData.HORIZ_COLS] {
            { PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK },
            { PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK },
            { PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK, PlayerColor.BLANK }
        };
    }

    private void InitialisePlayerData()
    {
        // ignore the exception and initialise playerData
        gameData.playerData = new PlayerData[6];
        gameData.playerData[0] = new PlayerData();
        gameData.playerData[1] = new PlayerData();
        gameData.playerData[2] = new PlayerData();
        gameData.playerData[3] = new PlayerData();

        gameData.playerData[0].playerColor = PlayerColor.PINK;
        gameData.playerData[0].playerEnabled = false;
        gameData.playerData[0].playerName = "";
        gameData.playerData[0].isBorg = false;

        gameData.playerData[1].playerColor = PlayerColor.YELLOW;
        gameData.playerData[1].playerEnabled = false;
        gameData.playerData[1].playerName = "";
        gameData.playerData[1].isBorg = false;

        gameData.playerData[2].playerColor = PlayerColor.BLUE;
        gameData.playerData[2].playerEnabled = false;
        gameData.playerData[2].playerName = "";
        gameData.playerData[2].isBorg = false;

        gameData.playerData[3].playerColor = PlayerColor.GREEN;
        gameData.playerData[3].playerEnabled = false;
        gameData.playerData[3].playerName = "";
        gameData.playerData[3].isBorg = false;

        InitialiseBorgPlayers();
    }

    private void InitialiseBorgPlayers()
    {
        gameData.playerData[4] = new PlayerData();
        gameData.playerData[5] = new PlayerData();
        gameData.playerData[4].playerColor = PlayerColor.COMPUTER1;
        gameData.playerData[4].playerEnabled = false;
        gameData.playerData[4].playerName = "One of Two";
        gameData.playerData[4].isBorg = true;

        gameData.playerData[5].playerColor = PlayerColor.COMPUTER2;
        gameData.playerData[5].playerEnabled = false;
        gameData.playerData[5].playerName = "Two of Two";
        gameData.playerData[5].isBorg = true;
    }

    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData);
        string saveFile = Path.Combine(Application.persistentDataPath, GameDataSaveFileName);
        File.WriteAllText(saveFile, json);
    }

    public bool IsPennedIn(int row, int col)
    {
        return
            //above
            IsHorizFenceSet(row, col) &&
            IsHorizFenceSet(row + 1, col) &&
            IsVertFenceSet(row, col) &&
            IsVertFenceSet(row, col + 1);
    }

    private bool IsVertFenceSet(int row, int col)
    {
        return gameData.boardData.verts[row, col];
    }

    private bool IsHorizFenceSet(int row, int col)
    {
        return gameData.boardData.horiz[row, col];
    }

    public void SetPen(int row, int col, PlayerColor player)
    {
        if (gameData.boardData.pens[row, col] == PlayerColor.BLANK)
            gameData.boardData.pens[row, col] = player;
    }

    public void DecPigCount(PlayerColor p)
    {
        gameData.playerData[(int)p - 1].numPigs--;
    }

}
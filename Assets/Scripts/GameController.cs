using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance=null;

    private GameDataController gameDataController;
    Player[] players = new Player[4];
    GameState oldGameState = GameState.MENU;

    public Canvas mainMenu;
    public Canvas spinner;
    public Canvas gameOver;
    public Canvas gameCanvas;
    private GameBoard gameBoardScript;

    private static GameController Instance {
        get {
            if (_instance == null)
                _instance = new GameController();
            return _instance;   
        }
    }

    public void Awake()
    {
        if (_instance == null)
            _instance = this;

        gameDataController = new GameDataController();
        gameDataController.LoadGameData();
    }

    public void Start()
    {
        Instance.gameBoardScript = gameCanvas.GetComponent<GameBoard>();

        // Switch to our initially loaded state
        if (oldGameState == GameState.MENU && gameDataController.PlayState == GameState.MENU)
            DialogManager.Show("MainMenu");
        DoStateChange(gameDataController.PlayState);
    }

    private void SetNumPigs(int pigCount)
    {
        // Set the pig count for each player at the start of the game
        SetNumPigs(GetPlayerData(PlayerColor.PINK), pigCount);
        SetNumPigs(GetPlayerData(PlayerColor.YELLOW), pigCount);
        SetNumPigs(GetPlayerData(PlayerColor.BLUE), pigCount);
        SetNumPigs(GetPlayerData(PlayerColor.GREEN), pigCount);
        SetNumPigs(GetPlayerData(PlayerColor.COMPUTER1), pigCount);
        SetNumPigs(GetPlayerData(PlayerColor.COMPUTER2), pigCount);
    }

    private void SetNumPigs(PlayerData playerData, int pigCount)
    {
        if (playerData.playerEnabled)
            playerData.numPigs = pigCount;
    }

    public void DoStateChange(GameState newState)
    {
        if (oldGameState == newState)
            return;

        gameDataController.PlayState = newState;

        // hide the main menu canvas
        if (oldGameState == GameState.MENU)
        {
            DialogManager.Hide("MainMenu");
        }

        if (oldGameState == GameState.SPIN)
        {
            //hide the spinner
            DialogManager.Hide("Spinner");
        }

        if (oldGameState == GameState.OVER)
        {
            // hide the game over menu
            DialogManager.Hide("GameOver");
        }

        if (newState == GameState.SPIN)
        {
            if (oldGameState == GameState.BAD_PIGGY || oldGameState == GameState.FENCE)
            {
                // switch to next player
                GameController.Instance.NextPlayer();
            }

            //bring up spinner
            DialogManager.Show("Spinner");
        }

        if (newState == GameState.OVER)
        {
            DialogManager.Show("GameOver");
        }

        if (newState == GameState.MENU)
        {
            DialogManager.Show("MainMenu");
        }

        gameBoardScript.ReceiveStateChange(newState);

        oldGameState = newState;
        gameDataController.SaveGameData();
    }

    private void NextPlayer()
    {
        do
        {
            if (gameDataController.CurrentPlayer == PlayerColor.COMPUTER2)
                gameDataController.CurrentPlayer = PlayerColor.PINK;
            else
                gameDataController.CurrentPlayer = (PlayerColor)(int)gameDataController.CurrentPlayer + 1;
        } while (!GetPlayerData(gameDataController.CurrentPlayer).playerEnabled);
    }

    static public T FindComponentByName<T>(T[] components, string name) where T : Component
    {
        foreach (T c in components)
            if (name == c.name)
                return c;
        return (T)null;
    }

    static public void StartGame()
    {
        Instance.gameDataController.ResetGameData();
        int playerCount = 0;
        foreach (Player p in Instance.players)
        {
            PlayerData data = GetPlayerData(p.playerColor);
            data.playerEnabled = p.toggle.isOn;
            if (data.playerEnabled) playerCount++;
            data.playerName = p.inputField.text;
            MonoBehaviour.print("Player " + p.playerColor.ToString() + " is named " + data.playerName + " and is " + (data.playerEnabled ? "enabled" : "disabled"));
        }

        Instance.gameDataController.gameData.playerData[(int)PlayerColor.COMPUTER1 - 1].playerEnabled = false;
        Instance.gameDataController.gameData.playerData[(int)PlayerColor.COMPUTER2 - 1].playerEnabled = false;

        switch (playerCount)
        {
            case 2:
                Instance.SetNumPigs(6);
                break;
            case 3:
                Instance.SetNumPigs(5);
                break;
            case 4:
                Instance.SetNumPigs(4);
                break;
            case 1:
                Instance.gameDataController.gameData.playerData[(int)PlayerColor.COMPUTER1 - 1].playerEnabled = true;
                Instance.SetNumPigs(6);
                break;
            case 0:
                Instance.gameDataController.gameData.playerData[(int)PlayerColor.COMPUTER1 - 1].playerEnabled = true;
                Instance.gameDataController.gameData.playerData[(int)PlayerColor.COMPUTER2 - 1].playerEnabled = true;
                Instance.SetNumPigs(6);
                break;
            default:
                GenericDialog dialog = GenericDialog.Instance();
                dialog.Title("Need More Players")
                    .Message("You need to have at least 2 players")
                    .OnAccept("OK", () => {
                        DialogManager.Hide("generic");
                    })
                    .NoDecline();

                DialogManager.Show("generic");
                return;
        }

        Instance.gameDataController.CurrentPlayer = PlayerColor.COMPUTER2;
        Instance.NextPlayer();
        Instance.gameDataController.SaveGameData();

        StateChange(GameState.START);
    }

    static public void StopGame()
    {
        Instance.DoStateChange(GameState.MENU);
    }

    static public void addPlayer(Player player)
    {
        Instance.players[(int)player.playerColor - 1] = player;

        PlayerData playerData = GetPlayerData(player.playerColor);
        player.toggle.isOn = playerData.playerEnabled;
        player.inputField.text = playerData.playerName;
    }

    static public PlayerData GetPlayerData(PlayerColor player)
    {
        return Instance.gameDataController.gameData.playerData[(int)player - 1];
    }

    static public PlayerColor GetCurrentPlayer()
    {
        return Instance.gameDataController.CurrentPlayer;
    }

    static public BoardData GetBoardData()
    {
        return Instance.gameDataController.gameData.boardData;
    }

    static public KeyValuePair<int, int>[] AddVertFence(int row, int col)
    {
        Instance.gameDataController.gameData.boardData.verts[row,col] = true;

        KeyValuePair<int, int>[] result = new KeyValuePair<int, int>[2];
        result[0] = new KeyValuePair<int, int>(-1, -1);
        result[1] = new KeyValuePair<int, int>(-1, -1);
        if (col > 0)
        {
            if (IsPennedIn(row, col - 1))
            {
                Instance.gameDataController.SetPen(row, col - 1, GetCurrentPlayer());
                result[0] = new KeyValuePair<int, int>(row, col - 1);
                Instance.gameDataController.DecPigCount(GetCurrentPlayer());
            }
        }
        if (col < BoardData.HORIZ_COLS)
        {
            if (IsPennedIn(row, col))
            {
                Instance.gameDataController.SetPen(row, col, GetCurrentPlayer());
                result[1] = new KeyValuePair<int, int>(row, col);
                Instance.gameDataController.DecPigCount(GetCurrentPlayer());
            }
        }

        return result;
    }

    static public KeyValuePair<int, int>[] AddHorizFence(int row, int col)
    {
        Instance.gameDataController.gameData.boardData.horiz[row,col] = true;

        KeyValuePair<int, int>[] result = new KeyValuePair<int, int>[2];
        result[0] = new KeyValuePair<int, int>(-1, -1);
        result[1] = new KeyValuePair<int, int>(-1, -1);
        if (row > 0)
        {
            if (IsPennedIn(row - 1, col))
            {
                Instance.gameDataController.SetPen(row - 1, col, GetCurrentPlayer());
                result[0] = new KeyValuePair<int, int>(row - 1, col);
                Instance.gameDataController.DecPigCount(GetCurrentPlayer());
            }
        }

        if (row < BoardData.VERT_ROWS)
        {
            if (IsPennedIn(row, col))
            {
                Instance.gameDataController.SetPen(row, col, GetCurrentPlayer());
                result[1] = new KeyValuePair<int, int>(row, col);
                Instance.gameDataController.DecPigCount(GetCurrentPlayer());
            }
        }

        return result;
    }

    public static bool IsPennedIn(int row, int col)
    {
        return Instance.gameDataController.IsPennedIn(row, col);
    }

    static public void StateChange(GameState newState)
    {
        Instance.DoStateChange(newState);
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour {

    private const string SINGLE_FENCE_PROMPT = "Choose where to put your fence";
    private const string SECOND_FENCE_PROMPT = "Choose where to put your second fence";
    private const string FENCE2_PROMPT = "Choose where to put your first fence";
    private const string BAD_PIGGY_PROMPT = "The BAD PIGGY runs away - miss this turn";

    private const float START_DELAY = 0.3f;
    private const float PENNED_PIG_DELAY = 2f;

    public Component PinkPlayer;
    public Component YellowPlayer;
    public Component BluePlayer;
    public Component GreenPlayer;
    public Component ComputerPlayer1;
    public Component ComputerPlayer2;

    public Light playerSpot;

    public Image fence1;
    public Image fence2;

    public TMP_Text promptText;

    public Component badPiggyAnim;
    private BadPiggyController badPiggyController;

    Button[,] horiz = new Button[4,5];
    Button[,] verts = new Button[3,6];
    PennedPig[,] pens = new PennedPig[3,5];
    private GameState currentState;
    private int numFences;
    private PlayerColor activePlayer=PlayerColor.BLANK;

    private BoardPlayer[] boardPlayers = new BoardPlayer[6];

    private void Awake()
    {
        // Map each Fence to an array entry
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i=0; i < BoardData.HORIZ_ROWS; i++)
        {
            for(int j=0; j < BoardData.HORIZ_COLS; j++)
            {
                horiz[i, j] = GameController.FindComponentByName<Button>(buttons, "FenceH" + i.ToString() + j.ToString());
                horiz[i, j].onClick.AddListener(this.OnFence);
            }
        }

        for (int i = 0; i < BoardData.VERT_ROWS; i++)
        {
            for (int j = 0; j < BoardData.VERT_COLS; j++)
            {
                verts[i, j] = GameController.FindComponentByName<Button>(buttons, "FenceV" + i.ToString() + j.ToString());
                verts[i, j].onClick.AddListener(this.OnFence);
            }
        }

        PennedPig[] scripts = GetComponentsInChildren<PennedPig>();
        for (int i = 0; i < BoardData.VERT_ROWS; i++)
        {
            for (int j = 0; j < BoardData.HORIZ_COLS; j++)
            {
                pens[i, j] = GameController.FindComponentByName<PennedPig>(scripts, "Pen" + i.ToString() + j.ToString());
            }
        }

        boardPlayers[(int)PlayerColor.PINK - 1] = PinkPlayer.GetComponent<BoardPlayer>();
        boardPlayers[(int)PlayerColor.YELLOW - 1] = YellowPlayer.GetComponent<BoardPlayer>();
        boardPlayers[(int)PlayerColor.BLUE - 1] = BluePlayer.GetComponent<BoardPlayer>();
        boardPlayers[(int)PlayerColor.GREEN - 1] = GreenPlayer.GetComponent<BoardPlayer>();
        boardPlayers[(int)PlayerColor.COMPUTER1 - 1] = ComputerPlayer1.GetComponent<BoardPlayer>();
        boardPlayers[(int)PlayerColor.COMPUTER2 - 1] = ComputerPlayer2.GetComponent<BoardPlayer>();

        BorgPlayer.gameBoard = this;
    }

    private void StartGame()
    {
        PinkPlayer.gameObject.SetActive(false);
        YellowPlayer.gameObject.SetActive(false);
        BluePlayer.gameObject.SetActive(false);
        GreenPlayer.gameObject.SetActive(false);
        ComputerPlayer1.gameObject.SetActive(false);
        ComputerPlayer2.gameObject.SetActive(false);
        promptText.text = "";
        InitialiseBoard(GameController.GetBoardData());
        PinkPlayer.gameObject.SetActive(GameController.GetPlayerData(PlayerColor.PINK).playerEnabled);
        YellowPlayer.gameObject.SetActive(GameController.GetPlayerData(PlayerColor.YELLOW).playerEnabled);
        BluePlayer.gameObject.SetActive(GameController.GetPlayerData(PlayerColor.BLUE).playerEnabled);
        GreenPlayer.gameObject.SetActive(GameController.GetPlayerData(PlayerColor.GREEN).playerEnabled);
        ComputerPlayer1.gameObject.SetActive(GameController.GetPlayerData(PlayerColor.COMPUTER1).playerEnabled);
        ComputerPlayer2.gameObject.SetActive(GameController.GetPlayerData(PlayerColor.COMPUTER2).playerEnabled);
        activePlayer = GameController.GetCurrentPlayer();
        Vector3 location;
        switch((PlayerColor)activePlayer)
        {
            case PlayerColor.PINK:
                location = PinkPlayer.transform.position;
                break;
            case PlayerColor.YELLOW:
                location = YellowPlayer.transform.position;
                break;
            case PlayerColor.BLUE:
                location = BluePlayer.transform.position;
                break;
            case PlayerColor.GREEN:
                location = GreenPlayer.transform.position;
                break;
            case PlayerColor.COMPUTER1:
                location = ComputerPlayer1.transform.position;
                break;
            case PlayerColor.COMPUTER2:
                location = ComputerPlayer2.transform.position;
                break;
            default:
                location = playerSpot.transform.position;
                break;
        }
        // initialise spot location
        playerSpot.transform.DOMove(location, 0f);
        playerSpot.gameObject.SetActive(true);

        badPiggyController = badPiggyAnim.GetComponent<BadPiggyController>();
    }

    public void InitialiseBoard(BoardData boardData)
    {
        for (int i=0; i < BoardData.HORIZ_ROWS; i++)
        {
            for (int j = 0; j < BoardData.HORIZ_COLS; j++)
                horiz[i, j].interactable = !boardData.horiz[i,j];
        }

        for (int i = 0; i < BoardData.VERT_ROWS; i++)
        {
            for (int j = 0; j < BoardData.VERT_COLS; j++)
                verts[i, j].interactable = !boardData.verts[i,j];
        }

        for (int i = 0; i < BoardData.VERT_ROWS; i++)
        {
            for(int j=0; j < BoardData.HORIZ_COLS; j++)
                pens[i,j].SetPigColor(boardData.pens[i,j]);
        }
    }

    public void SetPigCount(PlayerColor c)
    {
        if (c == PlayerColor.BLANK)
            return;

        boardPlayers[(int)c - 1].UpdateCount();
    }

    private void HighlightPlayer()
    {
        if (activePlayer == PlayerColor.BLANK)
            return;

        boardPlayers[(int)activePlayer - 1].OnActivePlayer();
    }

    public void ReceiveStateChange(GameState newState)
    {
        if (newState == GameState.START)
        {
            StartGame();
            DOTween.Sequence().AppendInterval(START_DELAY).AppendCallback(ToSpin);
            return;
        }

        if (newState == GameState.SPIN)
        {
            promptText.text = "";
            activePlayer = GameController.GetCurrentPlayer();
            HighlightPlayer();
            fence1.gameObject.SetActive(false);
            fence2.gameObject.SetActive(false);
            BorgPlayer.DoSpin(activePlayer);
        }

        if (newState == GameState.OVER)
        {
            promptText.text = "";
            fence1.gameObject.SetActive(false);
            fence2.gameObject.SetActive(false);
        }

        if (newState == GameState.FENCE2)
        {
            promptText.text = I18n.GetText("FENCE2_PROMPT", FENCE2_PROMPT);
            numFences = 2;
            fence1.gameObject.SetActive(true);
            fence2.gameObject.SetActive(true);
            BorgPlayer.DoFence(activePlayer, 2);
        }

        if (newState == GameState.BAD_PIGGY)
        {
            numFences = 0;
            promptText.text = I18n.GetText("BAD_PIGGY_PROMPT", BAD_PIGGY_PROMPT);
            DOTween.Sequence().AppendCallback(badPiggyController.Trigger).AppendInterval(BadPiggyController.BAD_PIGGY_DELAY).AppendCallback(ToSpin);
        }

        if (newState == GameState.FENCE)
        {
            numFences = 1;
            if (currentState == GameState.FENCE2)
            {
                promptText.text = I18n.GetText("SECOND_FENCE_PROMPT", SECOND_FENCE_PROMPT);
                fence1.gameObject.SetActive(false);
            }
            else
            {
                promptText.text = I18n.GetText("SINGLE_FENCE_PROMPT", SINGLE_FENCE_PROMPT);
                fence1.gameObject.SetActive(true);
            }
            BorgPlayer.DoFence(activePlayer, 1);
        }
        currentState = newState;
    }

    public void ToSpin()
    {
        GameController.StateChange(GameState.SPIN);
    }

    public void ToOver()
    {
        GameController.StateChange(GameState.OVER);
    }

    public void OnFence()
    {
        FenceLocation fence = ExtractFenceLocation(EventSystem.current.currentSelectedGameObject.name);
        OnFence(fence);
    }

    public void OnFence(FenceLocation fenceLocation)
    {
        if (numFences > 0)
        {
            Button fence;
            KeyValuePair<int, int>[] affectedPens;
            if (fenceLocation.fenceType == FenceLocation.Type.Horizontal)
            {
                fence = horiz[fenceLocation.row, fenceLocation.col];
                affectedPens = GameController.AddHorizFence(fenceLocation.row, fenceLocation.col);
            }
            else
            {
                fence = verts[fenceLocation.row, fenceLocation.col];
                affectedPens = GameController.AddVertFence(fenceLocation.row, fenceLocation.col);
            }
            fence.interactable = false;

            bool isPigPenned = false;
            foreach (KeyValuePair<int, int> k in affectedPens)
                if (k.Key != -1)
                {
                    isPigPenned = true;
                    pens[k.Key,k.Value].SetPigColor(activePlayer);
                    SetPigCount(activePlayer);
                    if (GameController.GetPlayerData(activePlayer).numPigs<=0)
                    {
                        DOTween.Sequence().AppendInterval(PENNED_PIG_DELAY).AppendCallback(ToOver);
                        numFences=0;
                        return;
                    }
                }

            GameState nextState;

            if (currentState == GameState.FENCE2)
                nextState = GameState.FENCE;
            else
                nextState = GameState.SPIN;

            if (isPigPenned && nextState == GameState.SPIN)
                DOTween.Sequence().AppendInterval(PENNED_PIG_DELAY).AppendCallback(ToSpin);
            else
                GameController.StateChange(nextState);
        }
    }

    private FenceLocation ExtractFenceLocation(string name)
    {
        int row = int.Parse(name.ToCharArray()[6].ToString());
        int col = int.Parse(name.ToCharArray()[7].ToString());
        switch(name.ToCharArray()[5])
        {
            case 'H':
                return new FenceLocation(FenceLocation.Type.Horizontal, row, col);
            case 'V':
                return new FenceLocation(FenceLocation.Type.Vertical, row, col);
        }
        return null;
    }
}

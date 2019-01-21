using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

class BorgPlayer
{
    private static readonly float BORG_DELAY=0.3f;
    public static Spinner spinner;
    public static GameBoard gameBoard;
    private static FenceLocation firedFenceLocation;

    private static List<FenceLocation> singleFenceLocations = new List<FenceLocation>();
    private static List<FenceLocation> doubleFenceLocations = new List<FenceLocation>();
    private static List<FenceLocation> safeSingleFenceLocations = new List<FenceLocation>();
    private static List<FenceLocation> safeDoubleFenceLocations = new List<FenceLocation>();
    private static List<FenceLocation> unsafeFenceLocations = new List<FenceLocation>();

    public static void DoSpin(PlayerColor activePlayer)
    {
        PlayerData p = GameController.GetPlayerData(activePlayer);
        if (!p.isBorg)
            return;

        DOTween.Sequence().AppendInterval(BORG_DELAY).AppendCallback(spinner.OnSpin);
    }

    public static void DoFence(PlayerColor activePlayer, int numFences)
    {
        PlayerData p = GameController.GetPlayerData(activePlayer);
        if (!p.isBorg)
            return;

        CheckPens();

        // 1 fence gives us a pen
        if (singleFenceLocations.Count > 0)
        {
            int pick = UnityEngine.Random.Range(0, singleFenceLocations.Count);
            PlaceFence(singleFenceLocations[pick]);
            return;
        }

        // 2 fences gives us a pen
        if (numFences == 2 && doubleFenceLocations.Count > 0)
        {
            int pick = UnityEngine.Random.Range(0, doubleFenceLocations.Count);
            PlaceFence(doubleFenceLocations[pick]);
            return;
        }

        // placing 1 fence is safe
        if (safeSingleFenceLocations.Count > 0)
        {
            int pick = UnityEngine.Random.Range(0, safeSingleFenceLocations.Count);
            PlaceFence(safeSingleFenceLocations[pick]);
            return;
        }

        // placing 1 fence will create a 2 fence pen
        if (safeDoubleFenceLocations.Count > 0)
        {
            int pick = UnityEngine.Random.Range(0, safeDoubleFenceLocations.Count);
            PlaceFence(safeDoubleFenceLocations[pick]);
            return;
        }

        // placing 1 fence will create a 3 fence pen
        if (unsafeFenceLocations.Count > 0)
        {
            int pick = UnityEngine.Random.Range(0, unsafeFenceLocations.Count);
            PlaceFence(unsafeFenceLocations[pick]);
            return;
        }
    }

    private static void CheckPens()
    {
        singleFenceLocations.Clear();
        doubleFenceLocations.Clear();
        safeSingleFenceLocations.Clear();
        safeDoubleFenceLocations.Clear();
        unsafeFenceLocations.Clear();

        // iterate over horizontal fences and test the above and below pens
        for (int row = 0; row < BoardData.HORIZ_ROWS; row++)
        {
            for (int col = 0; col < BoardData.HORIZ_COLS; col++)
            {
                if (GameController.GetBoardData().horiz[row, col])
                    continue;
                int aboveCount = 0;
                if (row > 0)
                    aboveCount = CountFences(row - 1, col);
                int belowCount = 0;
                if (row < BoardData.VERT_ROWS)
                    belowCount = CountFences(row, col);

                AddFenceToLocations(FenceLocation.Type.Horizontal, row, col, Math.Max(aboveCount, belowCount));
            }
        }

        // iterate over vertical fences and test the left and right pens
        for (int row = 0; row < BoardData.VERT_ROWS; row++)
        {
            for (int col = 0; col < BoardData.VERT_COLS; col++)
            {
                if (GameController.GetBoardData().verts[row, col])
                    continue;
                int leftCount = 0;
                if (col > 0)
                    leftCount = CountFences(row, col - 1);
                int rightCount = 0;
                if (col < BoardData.HORIZ_COLS)
                    rightCount = CountFences(row, col);

                AddFenceToLocations(FenceLocation.Type.Vertical, row, col, Math.Max(leftCount, rightCount));
            }
        }
    }

    private static void AddFenceToLocations(FenceLocation.Type fenceType, int row, int col, int count)
    {
        switch (count)
        {
            case 3:
                // find the unset fence and add it to single
                singleFenceLocations.Add(new FenceLocation(fenceType, row, col));
                return;
            case 2:
                FenceLocation newFence = new FenceLocation(fenceType, row, col);
                doubleFenceLocations.Add(newFence);
                unsafeFenceLocations.Add(newFence);
                return;
            case 1:
                safeDoubleFenceLocations.Add(new FenceLocation(fenceType, row, col));
                return;
            case 0:
                safeSingleFenceLocations.Add(new FenceLocation(fenceType, row, col));
                return;
        }
    }

    private static int CountFences(int row, int col)
    {
        int count = 0;
        // count how many fences already placed
        bool above, below, left, right = false;
        GetFenceStatus(row, col, out above, out below, out left, out right);

        if (above)
            count++;
        if (below)
            count++;
        if (left)
            count++;
        if (right)
            count++;
        return count;
    }

    private static void GetFenceStatus(int row, int col, out bool above, out bool below, out bool left, out bool right)
    {
        above = GameController.GetBoardData().horiz[row, col];
        below = GameController.GetBoardData().horiz[row + 1, col];
        left = GameController.GetBoardData().verts[row, col];
        right = GameController.GetBoardData().verts[row, col + 1];
    }

    private static void PlaceFence(FenceLocation fence)
    {
        firedFenceLocation = fence;
        DOTween.Sequence().AppendInterval(BORG_DELAY).AppendCallback(BorgPlayer.FireButton);
    }

    private static void FireButton()
    {
        gameBoard.OnFence(firedFenceLocation);
    }
}
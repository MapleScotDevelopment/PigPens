using System;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

[Serializable]
public class BoardData : ISerializationCallbackReceiver
{
    public const int HORIZ_ROWS = 4;
    public const int HORIZ_COLS = 5;
    public const int VERT_ROWS = 3;
    public const int VERT_COLS = 6;

    // Unity Json serialisation can't handle multi-dimensional arrays
    public bool[] _horiz0;
    public bool[] _horiz1;
    public bool[] _horiz2;
    public bool[] _horiz3;

    public bool[] _verts0;
    public bool[] _verts1;
    public bool[] _verts2;

    public PlayerColor[] _pens0;
    public PlayerColor[] _pens1;
    public PlayerColor[] _pens2;

    public bool[,] horiz;
    public bool[,] verts;
    public PlayerColor[,] pens;

    public void OnAfterDeserialize()
    {
        // Copy contens of 1D arrays to 2D arrays
        horiz = new bool[HORIZ_ROWS, HORIZ_COLS];

        for(int i=0; i < _horiz0.Length; i++)
            horiz[0,i] = _horiz0[i];
        for(int i=0; i < _horiz1.Length; i++)
            horiz[1,i] = _horiz1[i];
        for(int i=0; i < _horiz2.Length; i++)
            horiz[2,i] = _horiz2[i];
        for (int i = 0; i < _horiz3.Length; i++)
            horiz[VERT_ROWS, i] = _horiz3[i];

        verts = new bool[VERT_ROWS, VERT_COLS];

        for (int i = 0; i < _verts0.Length; i++)
            verts[0,i] = _verts0[i];
        for (int i = 0; i < _verts1.Length; i++)
            verts[1,i] = _verts1[i];
        for (int i = 0; i < _verts2.Length; i++)
            verts[2,i] = _verts2[i];

        pens = new PlayerColor[VERT_ROWS, HORIZ_COLS];

        for (int i = 0; i < _pens0.Length; i++)
            pens[0,i] = _pens0[i];
        for (int i = 0; i < _pens1.Length; i++)
            pens[1,i] = _pens1[i];
        for (int i = 0; i < _pens0.Length; i++)
            pens[2,i] = _pens2[i];
    }

    public void OnBeforeSerialize()
    {
        if (horiz == null)
            return;
        // Copy contents of 2d arrays to 1d arrays
        _horiz0 = new bool[HORIZ_COLS];
        _horiz1 = new bool[HORIZ_COLS];
        _horiz2 = new bool[HORIZ_COLS];
        _horiz3 = new bool[HORIZ_COLS];

        for(int i=0; i < HORIZ_ROWS; i++)
        {
            for (int j=0; j < HORIZ_COLS; j++)
            {
                switch (i)
                {
                    case 0:
                        _horiz0[j] = horiz[i,j];
                        break;
                    case 1:
                        _horiz1[j] = horiz[i,j];
                        break;
                    case 2:
                        _horiz2[j] = horiz[i,j];
                        break;
                    case VERT_ROWS:
                        _horiz3[j] = horiz[i,j];
                        break;
                }
            }
        }

        _verts0 = new bool[VERT_COLS];
        _verts1 = new bool[VERT_COLS];
        _verts2 = new bool[VERT_COLS];

        for (int i=0; i < VERT_ROWS; i++)
        {
            for(int j=0; j < VERT_COLS; j++)
            {
                switch (i)
                {
                    case 0:
                        _verts0[j] = verts[i,j];
                        break;
                    case 1:
                        _verts1[j] = verts[i,j];
                        break;
                    case 2:
                        _verts2[j] = verts[i,j];
                        break;
                }
            }
        }

        _pens0 = new PlayerColor[HORIZ_COLS];
        _pens1 = new PlayerColor[HORIZ_COLS];
        _pens2 = new PlayerColor[HORIZ_COLS];

        for (int i=0; i < VERT_ROWS; i++)
        {
            for(int j=0; j < HORIZ_COLS; j++)
            {
                switch (i)
                {
                    case 0:
                        _pens0[j] = pens[i,j];
                        break;
                    case 1:
                        _pens1[j] = pens[i,j];
                        break;
                    case 2:
                        _pens2[j] = pens[i,j];
                        break;
                }
            }
        }
    }
}
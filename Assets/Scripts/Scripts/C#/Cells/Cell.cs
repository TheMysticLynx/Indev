using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public static List<Cell> Cells = new List<Cell>();

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Direction { get; private set; }
    public int Texture { get; private set; }

    public Cell(int x, int y, int direction, int textureIndex)
    {
        X = x;
        Y = y;
        Direction = direction;
        Texture = textureIndex;
        Cells.Add(this);
    }

    public void Rotate(bool clockwise = true, int steps = 1) {
        if (clockwise) {
            Direction = (Direction + steps) % 4;
        } else {
            Direction = (Direction - steps) % 4;
            if (Direction < 0) {
                Direction += 4;
            }
        }
    }
}

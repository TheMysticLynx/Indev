using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    Vector2Int gridSize;
    Cell[,] grid;

    public CellGrid(Vector2Int gridSize)
    {
        this.gridSize = gridSize;
        grid = new Cell[gridSize.x, gridSize.y];
    }
}

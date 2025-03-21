using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject<T>
{
    private GridSystem<GridObject<T>> _grid;
    public int _xPosition;
    public int _yPosition;
    private T _cellObject;


    public void SetupGridObject(GridSystem<GridObject<T>> grid, int xPosition, int yPosition)
    {
        _grid = grid;
        _xPosition = xPosition;
        _yPosition = yPosition;
    }

    public void SetCellObject(T cellObject)
    {
        _cellObject = cellObject;
    }

    public T GetCellObject()
    {
        return _cellObject;
    }
}

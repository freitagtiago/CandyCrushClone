using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalConverter : CoordinateConverter
{
    public override Vector3 _forward => Vector3.up;

    public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 startingPosition)
    {
        return new Vector3(x, 0, y) * cellSize + startingPosition;
    }

    public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 startingPosition)
    {
        return new Vector3(x * cellSize + cellSize * 0.5f
                            , 0
                            , y * cellSize + cellSize * 0.5f) + startingPosition;
    }

    public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 startingPosition)
    {
        Vector3 gridPosition = (worldPosition - startingPosition) / cellSize;
        int x = Mathf.FloorToInt(gridPosition.x);
        int y = Mathf.FloorToInt(gridPosition.z);
        return new Vector2Int(x, y);
    }
}

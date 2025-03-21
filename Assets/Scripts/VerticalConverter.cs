using System.Net.NetworkInformation;
using UnityEngine;

public class VerticalConverter : CoordinateConverter
{
    public override Vector3 _forward => Vector3.forward;

    public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 startingPosition)
    {
        return new Vector3(x, y, 0) * cellSize + startingPosition;
    }

    public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 startingPosition)
    {
        return new Vector3(x * cellSize + cellSize * 0.5f
                            , y * cellSize + cellSize * 0.5f
                            , 0) + startingPosition;
    }

    public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 startingPosition)
    {
        Vector3 gridPosition = (worldPosition - startingPosition) / cellSize;
        int x = Mathf.FloorToInt(gridPosition.x);
        int y = Mathf.FloorToInt(gridPosition.y);
        return new Vector2Int(x, y);
    }
}

using UnityEngine;

public abstract class CoordinateConverter
{
    public abstract Vector3 GridToWorld(int x
                                        , int y
                                        , float cellSize
                                        , Vector3 startingPosition);

    public abstract Vector3 GridToWorldCenter(int x
                                            , int y
                                            , float cellSize
                                            , Vector3 startingPosition);
    public abstract Vector2Int WorldToGrid(Vector3 worldPosition
                                           , float cellSize
                                           , Vector3 startingPosition);

    public abstract Vector3 _forward { get; }

}

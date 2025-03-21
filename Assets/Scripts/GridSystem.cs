using System;
using TMPro;
using UnityEngine;

public class GridSystem<T>
{
    private int _width;
    private int _height;
    private float _cellSize;

    private Vector3 _startPosition;
    private T[,] _gridArray;
    private CoordinateConverter _coordinateConverter;

    public event Action<int, int, T> OnValueChanged;

    public static GridSystem<T> VerticalGrid(int width
                                            , int height
                                            , float cellSize
                                            , Vector3 origin)
    {
        return new GridSystem<T>(width, height, cellSize, origin, new VerticalConverter());
    }

    public static GridSystem<T> HorizontalGrid(int width
        , int height
        , float cellSize
        , Vector3 origin)
    {
        return new GridSystem<T>(width, height, cellSize, origin, new HorizontalConverter());
    }


    public GridSystem(int width
                     , int height
                     , float cellSize
                     , Vector3 startPosition
                     , CoordinateConverter coordinateConverter)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _startPosition = startPosition;
        _gridArray = new T[width, height];

        _coordinateConverter = coordinateConverter == null ? new VerticalConverter() : coordinateConverter;

#if UNITY_EDITOR
        DrawGridLines();
#endif
    }

    public void SetCoordinateByWorldPosition(Vector3 worldPosition, T value)
    {
        Vector2Int position = _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _startPosition);
        SetCellValue(position.x, position.y, value);
    }

    public void SetCellValue(int x, int y, T value)
    {
        if (IsValidCoordinate(x, y))
        {
            _gridArray[x, y] = value;
            OnValueChanged?.Invoke(x, y, value);
        }
    }

    public T GetValueByWorldPosition(Vector3 worldPosition)
    {
        Vector2Int position = GetXYCoordinate(worldPosition);
        return GetCellValue(position.x, position.y);
    }

    public T GetCellValue(int x, int y)
    {
        if (IsValidCoordinate(x, y))
        {
            return _gridArray[x, y];
        }
        return default;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return _coordinateConverter.GridToWorld(x, y, _cellSize, _startPosition);
    }
    public Vector3 GetWorldPositionCenter(int x, int y)
    {
        return _coordinateConverter.GridToWorldCenter(x, y, _cellSize, _startPosition);
    }

    public Vector2Int GetXYCoordinate(Vector3 worldPosition)
    {
        return _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _startPosition);
    }

    private bool IsValidCoordinate(int x, int y)
    {
        return (x >= 0 && x < _width) && (y >= 0 && y < _height);
    }

    #region DEBUG_METHODS

    private void DrawGridLines()
    {
        GameObject debugContainer = new GameObject("DebugContainer");
        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                GenerateCoordinateLabel(debugContainer, $"{x},{y}", GetWorldPositionCenter(x, y), _coordinateConverter._forward);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 100f);
    }

    private TextMeshPro GenerateCoordinateLabel(GameObject parent
        , string message
        , Vector3 position
        , Vector3 direction)
    {
        GameObject gameObject = new GameObject($"Coordinate_{message}", typeof(TextMeshPro));
        TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();

        gameObject.transform.SetParent(parent.transform);
        gameObject.transform.position = position;
        gameObject.transform.forward = direction;

        textMeshPro.text = message;
        textMeshPro.fontSize = 2;
        textMeshPro.color = Color.white;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.GetComponent<MeshRenderer>().sortingOrder = 0;

        return textMeshPro;
    }
    #endregion
}

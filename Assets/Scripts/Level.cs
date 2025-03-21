using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Level : MonoBehaviour
{
    [SerializeField] private LevelConfigSO _levelConfig;
    [SerializeField] private CellObject _cellObjectPrefab;
    [SerializeField] private Ease ease = Ease.InQuad;

    private GridSystem<GridObject<CellObject>> _grid;
    private InputHandler _inputHandler;
    private Vector2Int _selectedCellPosition = Vector2Int.one * -1;
    

    private void Awake()
    {
        _inputHandler = FindObjectOfType<InputHandler>();
    }

    private void Start()
    {
        SetupGrid();
        _inputHandler.Fire += OnSelectCell;
    }

    private void OnDisable()
    {
        _inputHandler.Fire -= OnSelectCell;
    }

    private void SetupGrid()
    {
        _grid = GridSystem<GridObject<CellObject>>.VerticalGrid(_levelConfig._width
                                                                , _levelConfig._heigth
                                                                , _levelConfig._cellSize
                                                                , _levelConfig._startPosition);

        for(int x = 0; x < _levelConfig._width; x++)
        {
            for(int y = 0; y < _levelConfig._heigth; y++)
            {
                FillWithCellObject(x,y);
            }
        }
    }

    private void FillWithCellObject(int x, int y)
    {
        CellObject cellObject = Instantiate(_cellObjectPrefab, _grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        cellObject.Setup(_levelConfig.GetRandomObject());
        GridObject<CellObject> gridObject = new GridObject<CellObject>();
        gridObject.SetupGridObject(_grid, x, y);
        gridObject.SetCellObject(cellObject);
        _grid.SetCellValue(x, y, gridObject);
    }

    private void OnSelectCell()
    {
        Vector2Int clickPosition = _grid.GetXYCoordinate(Camera.main.ScreenToWorldPoint(_inputHandler.GetSelectedPosition()));

        if(!IsValidPosition(clickPosition)
            || IsEmptyPosition(clickPosition))
        {
            return;
        }

        if(_selectedCellPosition == clickPosition)
        {
            DeselectCell();
        }
        else if (_selectedCellPosition == Vector2Int.one * -1)
        {
            SelectCell(clickPosition);
        }
        else
        {
            StartCoroutine(RunGameLoop(_selectedCellPosition, clickPosition));
        }

    }

    private bool IsValidPosition(Vector2Int clickPosition)
    {
        return (clickPosition.x >= 0
                && clickPosition.x < _levelConfig._width)
                && (clickPosition.y >= 0
                && clickPosition.y < _levelConfig._heigth);
    }

    private bool IsEmptyPosition(Vector2Int clickPosition)
    {
        return _grid.GetCellValue(clickPosition.x, clickPosition.y) == null;
    }

    private void DeselectCell()
    {
        _selectedCellPosition = new Vector2Int(-1, -1);
    }

    private void SelectCell(Vector2Int selectedCellPosition)
    {
        _selectedCellPosition = selectedCellPosition;
    }

    private IEnumerator RunGameLoop(Vector2Int selectedCellPosition, Vector2Int newSelectionPosition)
    {
        yield return StartCoroutine(SwapCell(selectedCellPosition, newSelectionPosition));
        DeselectCell();
        yield return null;
    }

    private IEnumerator SwapCell(Vector2Int selectedCellPosition, Vector2Int newSelectionPosition)
    {
        GridObject<CellObject> selectedCell = _grid.GetCellValue(selectedCellPosition.x, selectedCellPosition.y);
        GridObject<CellObject> newSelection = _grid.GetCellValue(newSelectionPosition.x, newSelectionPosition.y);

        selectedCell.GetCellObject().transform
                .DOLocalMove(_grid.GetWorldPositionCenter(newSelection._xPosition, newSelection._yPosition), 0.5f)
                .SetEase(ease);
        newSelection.GetCellObject().transform
            .DOLocalMove(_grid.GetWorldPositionCenter(selectedCell._xPosition, selectedCell._yPosition), 0.5f)
            .SetEase(ease);

        _grid.SetCellValue(selectedCell._xPosition, selectedCell._yPosition, newSelection);
        _grid.SetCellValue(newSelection._xPosition, newSelection._yPosition, selectedCell);

        yield return new WaitForSeconds(0.5f);
    }
}

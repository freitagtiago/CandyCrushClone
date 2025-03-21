using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    [SerializeField] private LevelConfigSO _levelConfig;
    [SerializeField] private CellObject _cellObjectPrefab;
    [SerializeField] private Ease ease = Ease.InQuad;
    [SerializeField] private SpriteRenderer _backgroundRenderer;
    [SerializeField] private List<GameObject> _matchVFXPrefabList = new List<GameObject>();

    private GridSystem<GridObject<CellObject>> _grid;
    private InputHandler _inputHandler;
    private AudioPlayer _audioPlayer;
    private Vector2Int _selectedCellPosition = Vector2Int.one * -1;

    private bool _canSelect = true;
    
    private void Awake()
    {
        _inputHandler = FindObjectOfType<InputHandler>();
        _audioPlayer = FindObjectOfType<AudioPlayer>(); ;
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
        _backgroundRenderer.sprite = _levelConfig._background;
        for (int x = 0; x < _levelConfig._width; x++)
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
        if (!_canSelect)
        {
            return;
        }

        Vector2Int clickPosition = _grid.GetXYCoordinate(Camera.main.ScreenToWorldPoint(_inputHandler.GetSelectedPosition()));

        if (!IsValidPosition(clickPosition))
        {
            DeselectCell();
            return;
        }
        if (IsEmptyPosition(clickPosition))
        {
            DeselectCell();
            return;
        }

        if (_selectedCellPosition == clickPosition)
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
        _audioPlayer.PlaySelectSFX();
    }

    private IEnumerator RunGameLoop(Vector2Int selectedCellPosition, Vector2Int newSelectionPosition)
    {
        _canSelect = false;
        yield return StartCoroutine(SwapCell(selectedCellPosition, newSelectionPosition));
        List<Vector2Int> matches = FindCellMatches();
        yield return StartCoroutine(HandleMatches(matches));
        yield return StartCoroutine(GenerateCells());
        yield return StartCoroutine(FillEmptySpots());
        DeselectCell();
        _canSelect = true;
    }

    private IEnumerator FillEmptySpots()
    {
        for (int x = 0; x < _levelConfig._width; x++)
        {
            for (int y = 0; y < _levelConfig._heigth; y++)
            {
                if (_grid.GetCellValue(x, y) == null)
                {
                    FillWithCellObject(x, y);
                    _audioPlayer.PlayGenerateCellSFX();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        List<Vector2Int> newMatches = FindCellMatches();
        if (newMatches.Count > 0)
        {
            yield return StartCoroutine(HandleMatches(newMatches));
            yield return StartCoroutine(GenerateCells());
            yield return StartCoroutine(FillEmptySpots());
        }
    }

    private IEnumerator GenerateCells()
    {
        for (int x = 0; x < _levelConfig._width; x++)
        {
            for (int y = 0; y < _levelConfig._heigth; y++)
            {
                if (_grid.GetCellValue(x, y) == null)
                {
                    for (int i = y + 1; i < _levelConfig._heigth; i++)
                    {
                        GridObject<CellObject> cellBelow = _grid.GetCellValue(x, i);
                        if (cellBelow != null)
                        {
                            _grid.SetCellValue(x, y, cellBelow);
                            _grid.SetCellValue(x, i, null);

                            CellObject cellObject = cellBelow.GetCellObject();
                            Vector3 targetPosition = _grid.GetWorldPositionCenter(x, y);
                            cellObject.transform.DOLocalMove(targetPosition, 0.5f).SetEase(ease);
                            _audioPlayer.PlayCellFallSFX();
                            yield return new WaitForSeconds(0.1f);
                            break;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator HandleMatches(List<Vector2Int> matches)
    {
        foreach(Vector2Int match in matches)
        {
            CellObject cell = _grid.GetCellValue(match.x, match.y).GetCellObject();
            _grid.SetCellValue(match.x, match.y, null);
            ApplyMatchVFX(match);

            cell.transform.DOPunchScale(Vector3.one * 0.1f
                                        ,0.1f
                                        , 1
                                        , 0.5f);

            yield return new WaitForSeconds(0.1f);
            cell.Destroy();
        }
        _audioPlayer.PlayDestroyCellSFX();
    }

    private void ApplyMatchVFX(Vector2Int position)
    {
        //POLLING
        GameObject vfx = Instantiate(_matchVFXPrefabList[Random.Range(0, _matchVFXPrefabList.Count)], transform);
        vfx.transform.position = _grid.GetWorldPositionCenter(position.x, position.y);
        Destroy(vfx, 1f);
    }

    private List<Vector2Int> FindCellMatches()
    {
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

        for (int y = 0; y < _levelConfig._heigth; y++)
        {
            for (int x = 0; x < _levelConfig._width - 2; x++)
            {
                GridObject<CellObject> cellA = _grid.GetCellValue(x, y);
                GridObject<CellObject> cellB = _grid.GetCellValue(x + 1, y);
                GridObject<CellObject> cellC = _grid.GetCellValue(x + 2, y);

                if (cellA == null || cellB == null || cellC == null)
                {
                    continue;
                }

                if (cellA.GetCellObject().GetCellObjectSO() == cellB.GetCellObject().GetCellObjectSO()
                    && cellB.GetCellObject().GetCellObjectSO() == cellC.GetCellObject().GetCellObjectSO())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x + 1, y));
                    matches.Add(new Vector2Int(x + 2, y));
                }
            }
        }

        for (int x = 0; x < _levelConfig._width; x++)
        {
            for (int y = 0; y < _levelConfig._heigth - 2; y++)
            {
                GridObject<CellObject> cellA = _grid.GetCellValue(x, y);
                GridObject<CellObject> cellB = _grid.GetCellValue(x, y + 1);
                GridObject<CellObject> cellC = _grid.GetCellValue(x, y + 2);

                if (cellA == null || cellB == null || cellC == null)
                {
                    continue;
                }

                if (cellA.GetCellObject().GetCellObjectSO() == cellB.GetCellObject().GetCellObjectSO()
                    && cellB.GetCellObject().GetCellObjectSO() == cellC.GetCellObject().GetCellObjectSO())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x, y + 1));
                    matches.Add(new Vector2Int(x, y + 2));
                }
            }
        }

        return new List<Vector2Int>(matches);
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

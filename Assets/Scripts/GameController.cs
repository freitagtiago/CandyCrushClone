using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private LevelConfigSO _levelConfig;
    [SerializeField] private Ease ease = Ease.InQuad;
    [SerializeField] private SpriteRenderer _backgroundRenderer;
    [SerializeField] private UIHandler _uiHandler;
    [SerializeField] private Transform _gridObjectTransform;

    [Header("Cell")]
    [SerializeField] private CellObject _cellObjectPrefab;
    private Queue<CellObject> _cellObjectPool = new Queue<CellObject>();
    [SerializeField] private int _cellPoolSize = 20;

    [Header("VFX")]
    [SerializeField] private List<GameObject> _matchVFXPrefabList = new List<GameObject>();
    private Queue<GameObject> _vfxPool = new Queue<GameObject>();
    [SerializeField] private int _vfxPoolSize = 10;

    private GridSystem<GridObject<CellObject>> _grid;
    private InputHandler _inputHandler;
    private Vector2Int _selectedCellPosition = Vector2Int.one * -1;

    private int _currentScore = 0;
    private bool _canSelect = false;
    
    private void Awake()
    {
        _inputHandler = FindObjectOfType<InputHandler>();
        _levelConfig = LevelLoader.Instance.GetCurrentLevel();
    }

    private void OnDisable()
    {
        if(_currentScore > PlayerPrefs.GetInt($"{_levelConfig._levelName}_max_score", 0))
        {
            PlayerPrefs.SetInt($"{_levelConfig._levelName}_max_score", _currentScore);
        }
        
        _inputHandler.Fire -= OnSelectCell;
    }

    private void Start()
    {
        InitializeCellObjectPool();
        InitializeVFXPool();

        SetupGrid();
        _inputHandler.Fire += OnSelectCell;
        StartCoroutine(StartLevel());
    }

    private void Update()
    {
        CheckWinCondition();
    }

    private IEnumerator StartLevel()
    {
        _uiHandler.EnableWaitMessage();
        yield return new WaitForSeconds(2f);
        StartCoroutine(_uiHandler.EnableStartMessage());
        _uiHandler.RunTimer(_levelConfig._levelTime);
        _canSelect = true;
    }

    private void CheckWinCondition()
    {
        if(_currentScore > _levelConfig._scoreToWin)
        {
            _canSelect = false;
            _uiHandler.StopTimer();
            _uiHandler.EnableGameOverPanel(true);
        }else if (_uiHandler._elapsedTime > _levelConfig._levelTime)
        {
            _canSelect = false;
            _uiHandler.StopTimer();
            _uiHandler.EnableGameOverPanel(false);
        }
    }

    #region POOL Methods
    private void InitializeCellObjectPool()
    {
        for (int i = 0; i < _cellPoolSize; i++)
        {
            CellObject cellObject = Instantiate(_cellObjectPrefab, _gridObjectTransform);
            cellObject.gameObject.SetActive(false);
            _cellObjectPool.Enqueue(cellObject);
        }
    }

    private void InitializeVFXPool()
    {
        for (int i = 0; i < _vfxPoolSize; i++)
        {
            GameObject vfx = Instantiate(_matchVFXPrefabList[Random.Range(0, _matchVFXPrefabList.Count)], _gridObjectTransform);
            vfx.SetActive(false);
            _vfxPool.Enqueue(vfx);
        }
    }

    private void ReturnCellObjectToPool(CellObject cellObject)
    {
        cellObject.gameObject.SetActive(false);
        _cellObjectPool.Enqueue(cellObject);
    }

    private CellObject GetCellObjectFromPool()
    {
        if (_cellObjectPool.Count > 0)
        {
            return _cellObjectPool.Dequeue();
        }
        else
        {
            CellObject cellObject = Instantiate(_cellObjectPrefab, _gridObjectTransform);
            cellObject.gameObject.SetActive(false);
            return cellObject;
        }
    }

    private GameObject GetVFXFromPool()
    {
        if (_vfxPool.Count > 0)
        {
            return _vfxPool.Dequeue();
        }
        else
        {
            GameObject vfx = Instantiate(_matchVFXPrefabList[Random.Range(0, _matchVFXPrefabList.Count)], _gridObjectTransform);
            vfx.SetActive(false);
            return vfx;
        }
    }

    private IEnumerator ReturnVFXToPoolAfterDelay(GameObject vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        vfx.SetActive(false);
        _vfxPool.Enqueue(vfx);
    }

    #endregion

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
        CellObject cellObject = GetCellObjectFromPool();
        if (cellObject != null)
        {
            cellObject.transform.position = _grid.GetWorldPositionCenter(x, y);
            cellObject.gameObject.SetActive(true);
            cellObject.Setup(_levelConfig.GetRandomObject());

            GridObject<CellObject> gridObject = new GridObject<CellObject>();
            gridObject.SetupGridObject(_grid, x, y);
            gridObject.SetCellObject(cellObject);
            _grid.SetCellValue(x, y, gridObject);
        }
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
        AudioPlayer.Instance.PlaySelectSFX();
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
                    AudioPlayer.Instance.PlayGenerateCellSFX();
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
                            AudioPlayer.Instance.PlayCellFallSFX();
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
        foreach (Vector2Int match in matches)
        {
            CellObject cell = _grid.GetCellValue(match.x, match.y).GetCellObject();
            int pointsToGet = cell.GetCellObjectSO()._points;
            _currentScore += pointsToGet;
            _uiHandler.UpdateScore(pointsToGet);
            _grid.SetCellValue(match.x, match.y, null);

            cell.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);

            yield return new WaitForSeconds(0.1f);

            AudioPlayer.Instance.PlayDestroyCellSFX();
            ApplyMatchVFX(match);
            ReturnCellObjectToPool(cell);
        }
        AudioPlayer.Instance.PlayDestroyCellSFX();
    }

    private void ApplyMatchVFX(Vector2Int position)
    {
        GameObject vfx = GetVFXFromPool();
        if (vfx != null)
        {
            vfx.transform.position = _grid.GetWorldPositionCenter(position.x, position.y);
            vfx.SetActive(true);
            StartCoroutine(ReturnVFXToPoolAfterDelay(vfx, 1f));
        }
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

        _grid.SetCellValue(selectedCellPosition.x, selectedCellPosition.y, newSelection);
        _grid.SetCellValue(newSelectionPosition.x, newSelectionPosition.y, selectedCell);

        selectedCell.GetCellObject().transform
                .DOLocalMove(_grid.GetWorldPositionCenter(newSelectionPosition.x, newSelectionPosition.y), 0.5f)
                .SetEase(ease);
        newSelection.GetCellObject().transform
            .DOLocalMove(_grid.GetWorldPositionCenter(selectedCellPosition.x, selectedCellPosition.y), 0.5f)
            .SetEase(ease);

        yield return new WaitForSeconds(0.5f);
    }
}

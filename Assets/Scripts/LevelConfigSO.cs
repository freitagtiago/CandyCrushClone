using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 0)]
public class LevelConfigSO : ScriptableObject
{
    public string _levelName;
    public Sprite _background;
    public int _width;
    public int _heigth;
    public float _cellSize = 1f;
    public Vector3 _startPosition = Vector3.zero;
    public List<CellObjectSO> _availableCellObjects = new List<CellObjectSO>();

    public CellObjectSO GetRandomObject()
    {
        int index = Random.Range(0, _availableCellObjects.Count);

        return _availableCellObjects[index];
    }
}

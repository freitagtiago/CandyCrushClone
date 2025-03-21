using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
    [SerializeField] private CellObjectSO _cellObject;

    public void Setup(CellObjectSO cellObject)
    {
        _cellObject = cellObject;
        GetComponent<SpriteRenderer>().sprite = cellObject._sprite;
    }

    public CellObjectSO GetCellObjectSO()
    {
        return _cellObject;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}

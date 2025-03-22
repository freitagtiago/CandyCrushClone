using UnityEngine;

[CreateAssetMenu(fileName = "CellObject", menuName = "ScriptableObjects/CellObject", order = 0)]
public class CellObjectSO : ScriptableObject
{
    public string _cellObjectName;
    public Sprite _sprite;
    public int _points = 5;
}

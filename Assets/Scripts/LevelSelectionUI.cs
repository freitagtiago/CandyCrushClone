using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionUI : MonoBehaviour
{
    [SerializeField] private LevelCard _levelCardPrefab;
    [SerializeField] private Transform _levelCardScrollView;

    private void Start()
    {
        LoadLevelCards();
    }

    private void LoadLevelCards()
    {
        LevelConfigSO[] levelConfigs = Resources.LoadAll<LevelConfigSO>("LevelConfig");

        foreach(LevelConfigSO levelConfig in levelConfigs)
        {
            LevelCard levelCard = Instantiate(_levelCardPrefab, _levelCardScrollView);
            levelCard.Setup(levelConfig);
        }
    }
}

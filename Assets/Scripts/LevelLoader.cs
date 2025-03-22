using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    private LevelConfigSO _currentLevel;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }


    public void SetCurrentLevel(LevelConfigSO currentLevel)
    {
        _currentLevel = currentLevel;
    }

    public LevelConfigSO GetCurrentLevel()
    {
        return _currentLevel;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCard : MonoBehaviour
{
    [SerializeField] private Image _levelImage;
    [SerializeField] private TextMeshProUGUI _levelName;
    [SerializeField] private TextMeshProUGUI _levelScore;
    private LevelConfigSO _levelConfig;
    public void Setup(LevelConfigSO levelConfig)
    {
        _levelConfig = levelConfig;
        GetComponent<Button>().onClick.AddListener(LoadLevel);
        _levelImage.sprite = levelConfig._background;
        _levelName.text = levelConfig._levelName;

        int maxScore = PlayerPrefs.GetInt($"{levelConfig._levelName}_max_score", 0);
        if(maxScore > 0)
        {
            _levelScore.text = $"MAX:{maxScore}";
            _levelScore.gameObject.SetActive(true);
        }
    }

    public void LoadLevel()
    {
        LevelLoader.Instance.SetCurrentLevel(_levelConfig);
        SceneManager.LoadScene(1);
    }
}

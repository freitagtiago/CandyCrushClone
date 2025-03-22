using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private string _waitMessage = "Prepare-se...";
    [SerializeField] private string _startMessage = "Começou!";
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private string _timerPattern = "Tempo {0}";
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private string _scorePattern = "{0} Pontos";

    [Header("EndGame Panel")]
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private TextMeshProUGUI _finalResult;
    [SerializeField] private string _finalResultPattern = "Você {0} {1} pontos coletados.";
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _returnToLobby;

    private int _score = 0;
    private float _currentLevelDuration;
    public float _elapsedTime { get; private set; }

    private void Awake()
    {
        _retryButton.onClick.AddListener(RetryButton);
        _returnToLobby.onClick.AddListener(ReturnToLobbyButton);
    }

    public void RunTimer(float levelDuration)
    {
        _currentLevelDuration = levelDuration;
        StartCoroutine(RunTimerRoutine());
    }

    public void StopTimer()
    {
        _currentLevelDuration = 0;
    }

    public void UpdateScore(int valueToAdd)
    {
        _score += valueToAdd;
        _scoreText.text = string.Format(_scorePattern, _score);
    }

    public IEnumerator EnableStartMessage()
    {
        _messageText.gameObject.SetActive(true);
        _messageText.text = _startMessage;
        yield return new WaitForSeconds(2f);
        _messageText.text = string.Empty;
        _messageText.gameObject.SetActive(false);
    }

    public void EnableWaitMessage()
    {
        _messageText.gameObject.SetActive(true);
        _messageText.text = _waitMessage;
    }

    private IEnumerator RunTimerRoutine()
    {
        float remainingTime = _currentLevelDuration;
        float timeSinceLastUpdate = 0f;

        while (_elapsedTime < _currentLevelDuration)
        {
            _elapsedTime += Time.deltaTime;
            timeSinceLastUpdate += Time.deltaTime;
            remainingTime = Mathf.Clamp(_currentLevelDuration - _elapsedTime, 0, _currentLevelDuration);

            if (timeSinceLastUpdate >= 1f)
            {
                _timerText.text = string.Format(_timerPattern, remainingTime.ToString("F0"));
                timeSinceLastUpdate = 0f;
            }

            yield return null;
        }

        _timerText.text = string.Format(_timerPattern, 0);
    }

    public void EnableGameOverPanel(bool wonGame)
    {
        if (wonGame)
        {
            _finalResult.text = String.Format(_finalResultPattern, "ganhou!", _score);
        }
        else
        {
            _finalResult.text = String.Format(_finalResultPattern, "perdeu.", _score);
        }

        _endPanel.SetActive(true);
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToLobbyButton()
    {
        SceneManager.LoadScene(0);
    }
}

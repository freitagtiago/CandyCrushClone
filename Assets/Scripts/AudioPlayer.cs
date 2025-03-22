using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance;

    [SerializeField] private AudioClip _music;
    [SerializeField] private AudioClip _selectCell;
    [SerializeField] private AudioClip _matchFound;
    [SerializeField] private AudioClip _noMatchFound;
    [SerializeField] private AudioClip _generateCell;
    [SerializeField] private AudioClip _cellFalling;
    [SerializeField] private AudioClip _destroyCell;

    [SerializeField] private AudioSource _fxAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;

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

    public void PlaySelectSFX()
    {
        _fxAudioSource.PlayOneShot(_selectCell);
    }

    public void PlayMatchSFX()
    {
        _fxAudioSource.PlayOneShot(_matchFound);
    }

    public void PlayNoMatchSFX()
    {
        _fxAudioSource.PlayOneShot(_noMatchFound);
    }

    public void PlayGenerateCellSFX()
    {
        _fxAudioSource.PlayOneShot(_generateCell);
    }

    public void PlayDestroyCellSFX()
    {
        _fxAudioSource.PlayOneShot(_destroyCell);
    }

    public void PlayCellFallSFX()
    {
        _fxAudioSource.PlayOneShot(_cellFalling);
    }
}

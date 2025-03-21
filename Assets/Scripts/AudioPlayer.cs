using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _selectCell;
    [SerializeField] private AudioClip _matchFound;
    [SerializeField] private AudioClip _noMatchFound;
    [SerializeField] private AudioClip _generateCell;
    [SerializeField] private AudioClip _cellFalling;
    [SerializeField] private AudioClip _destroyCell;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySelectSFX()
    {
        _audioSource.PlayOneShot(_selectCell);
    }

    public void PlayMatchSFX()
    {
        _audioSource.PlayOneShot(_matchFound);
    }

    public void PlayNoMatchSFX()
    {
        _audioSource.PlayOneShot(_noMatchFound);
    }

    public void PlayGenerateCellSFX()
    {
        _audioSource.PlayOneShot(_generateCell);
    }

    public void PlayDestroyCellSFX()
    {
        _audioSource.PlayOneShot(_destroyCell);
    }

    public void PlayCellFallSFX()
    {
        _audioSource.PlayOneShot(_cellFalling);
    }
}

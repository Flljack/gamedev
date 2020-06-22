using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Sprite soundOn;
    public Sprite soundOff;
    public GameObject[] soundButtons;
    public GameObject mainCamera;

    private bool _soundStatus;
    private void Start()
    {
        loadSoundStatus();
    }

    public void toggleSoundStatus()
    {
        _soundStatus = !_soundStatus;
        foreach (var soundButton in soundButtons)
        {
            soundButton.GetComponent<Image>().sprite = _soundStatus ? soundOn : soundOff;
        }
        changeAudioSource();
        saveSoundStatus();
    }

    private void changeAudioSource()
    {
        mainCamera.GetComponent<AudioSource>().mute = !_soundStatus;
    }
    
    private void loadSoundStatus()
    {
        _soundStatus = PlayerPrefs.GetInt("soundStatus") == 1 ? true : false;
        foreach (var soundButton in soundButtons)
        {
            soundButton.GetComponent<Image>().sprite = _soundStatus ? soundOn : soundOff;
        }
        changeAudioSource();
    }

    private void saveSoundStatus()
    {
        PlayerPrefs.SetInt("soundStatus", _soundStatus ? 1 : 0);
    }
}

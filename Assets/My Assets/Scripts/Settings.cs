using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private Color[] toggleColors; //0 for on color, 1 for off color
    [SerializeField]
    private Image musicToggleImg, sfxToggleImg;
    [SerializeField]
    private Sprite on, off;
    private bool musicON, sfxON;


    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteAll();
        //UIManager.instance.UpdateStarsText();
        UIManager.instance.UpdateCoinsText();
    }

    //Toggle music button
    public void ToggleMusic(Image button)
    {
        if (musicON) //If it's on, turn it off
        {
            MusicOff();
        } else {
            MusicOn();
        }
        print("Music: " + musicON);
    }

    //Toggle sounds effects button
    public void ToggleSFX()
    {
        if (sfxON) //If it's on, turn it off
        {
            SFXOff();
        } else
        {
            SFXOn();
        }
    }

    void MusicOn()
    {
        AudioManager.instance.SetMixerVolume(AudioManager.instance.musicMixer, 0);
        PlayerPrefs.SetInt("Music", 1);
        musicToggleImg.sprite = on;
        musicON = true;
        // print("Music on");
    }

    void MusicOff()
    {
        AudioManager.instance.SetMixerVolume(AudioManager.instance.musicMixer, -80);
        PlayerPrefs.SetInt("Music", 0);
        musicToggleImg.sprite = off;
        musicON = false;
        // print("Music off ");
    }

    void SFXOn()
    {
        AudioManager.instance.SetMixerVolume(AudioManager.instance.SFXMixer, 0);
        PlayerPrefs.SetInt("SFX", 1);
        sfxToggleImg.sprite = on;
        sfxON = true;
        // print("SFX on");
    }

    void SFXOff()
    {
        AudioManager.instance.SetMixerVolume(AudioManager.instance.SFXMixer, -80);
        PlayerPrefs.SetInt("SFX", 0);
        sfxToggleImg.sprite = off;
        sfxON = false;
        // print("SFX off ");
    }

    public void CheckSavedSettings()
    {
        if (musicON = PlayerPrefs.GetInt("Music", 1) == 1)
        {
            MusicOn();
        } else
        {
            MusicOff();
        }

        if (sfxON = PlayerPrefs.GetInt("SFX", 1) == 1)
        {
            SFXOn();
        } else
        {
            SFXOff();
        }
    }

    void OnDisable()
    {
        CheckSavedSettings();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Menu : MonoBehaviour {

    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public Toggle fullscreenToggle;
    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public int[] screenWidths;

    int activeIndex;

    void Start()
    {
        activeIndex = PlayerPrefs.GetInt("Screen res Index");
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.soundEffectsVolumePercent;

        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeIndex;
        }

        fullscreenToggle.isOn = isFullscreen;
    }

    public void play()
    {
        SceneManager.LoadScene("Game");
    }

    public void quit()
    {
        Application.Quit();
    }

    public void optionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void mainMenu()
    {
        optionsMenuHolder.SetActive(false);
        mainMenuHolder.SetActive(true);
    }

    public void setResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeIndex = i;
            float aspectRatio = 16f / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);

            PlayerPrefs.SetInt("Screen res Index", activeIndex);
            PlayerPrefs.Save();
        }
    }

    public void setMaster(float volume)
    {
        AudioManager.instance.setVolume(volume, AudioManager.AudioChannel.master);
    }
    public void setMusic(float volume)
    {
        AudioManager.instance.setVolume(volume, AudioManager.AudioChannel.music);
    }
    public void setSfx(float volume)
    {
        AudioManager.instance.setVolume(volume, AudioManager.AudioChannel.sfx);
    }

    public void setFullscreen(bool isFullscreen)
    {
        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if (isFullscreen)
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution maxRes = resolutions[resolutions.Length - 1];
            Screen.SetResolution(maxRes.width, maxRes.height, true);
        }
        else
        {
            setResolution(activeIndex);
        }

        PlayerPrefs.SetInt("Fullscreen", (isFullscreen ? 1 : 0));
        PlayerPrefs.Save();
    }
}

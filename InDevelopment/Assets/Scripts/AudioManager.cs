using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public enum AudioChannel { master, sfx, music};
    AudioSource soundEffect2DSource;
    public float masterVolumePercent { get; private set; }
    public float soundEffectsVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }
    AudioSource[] musicSources;
    int activeMusicSourceIndex;
    Transform audioListener;
    Transform playerT;
    SoundLibrary library;

    public static AudioManager instance;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            library = GetComponent<SoundLibrary>();
            audioListener = FindObjectOfType<AudioListener>().transform;
            if(FindObjectOfType<Player>() != null)
            {
                playerT = FindObjectOfType<Player>().transform;
            }
            musicSources = new AudioSource[2];
            for (int i = 0; i < musicSources.Length; i++)
            {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            GameObject newSoundEffect2DSource = new GameObject("2D Sound Effect Source");
            soundEffect2DSource = newSoundEffect2DSource.AddComponent<AudioSource>();
            newSoundEffect2DSource.transform.parent = transform;

            masterVolumePercent = PlayerPrefs.GetFloat("master volume", 0.5f);
            soundEffectsVolumePercent = PlayerPrefs.GetFloat("sfx volume", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("music volume", 1);
        }
    }

    void Update()
    {
        if(playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void playMusic(AudioClip clip, float fadeDurration)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(musicFade(fadeDurration));
    }

    IEnumerator musicFade(float durration)
    {
        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * (1 / durration);
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }

    public void playSound(AudioClip clip, Vector3 position)
    {
        if (clip != null) { 
            AudioSource.PlayClipAtPoint(clip, position, soundEffectsVolumePercent * masterVolumePercent);
        }
    }

    public void playSound(string soundName, Vector3 position)
    {
        playSound(library.getClipFromName(soundName), position);
    }

    public void setVolume(float percent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.master:
                masterVolumePercent = percent;
                break;
            case AudioChannel.sfx:
                soundEffectsVolumePercent = percent;
                break;
            case AudioChannel.music:
                musicVolumePercent = percent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master volume", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx volume", soundEffectsVolumePercent);
        PlayerPrefs.SetFloat("music volume", musicVolumePercent);
        PlayerPrefs.Save();
    }

    public void playSound2D(string name)
    {
        soundEffect2DSource.PlayOneShot(library.getClipFromName(name), soundEffectsVolumePercent * masterVolumePercent);
    }
}

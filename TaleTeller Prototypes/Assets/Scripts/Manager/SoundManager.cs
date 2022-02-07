using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Linq;
using NaughtyAttributes;

public class SoundManager : Singleton<SoundManager>
{
    public void Awake()
    {
        CreateSingleton(true);            
    }

    public SoundBank soundBank;
    public SoundBank musicBank;
    public Dictionary<string, AudioClip> soundsDictionary;
    public Dictionary<string, AudioClip> musicDictionary;

    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;

    [Header("Music Scenes")]
    [Scene] public int menuScene;
    [Scene] public int gameScene;

    public void Start()
    {
        soundsDictionary = InitDictionary(soundBank);
        musicDictionary = InitDictionary(musicBank);
        InitMusicQueue();
        LaunchMusic(SceneManager.GetActiveScene().buildIndex);
    }

    public Dictionary<string, AudioClip> InitDictionary(SoundBank bank)
    {
        Dictionary<string, AudioClip> result = new Dictionary<string, AudioClip>();
        for (int i = 0; i < bank.clips.Count; i++)
        {
            result.Add(bank.clips[i].id,bank.clips[i].clip);
        }
        return result;
    }

    public AudioClip GetSoundClip(string id)
    {
        if (soundsDictionary[id] == null)
            Debug.LogWarning("Wrong ID, no matching sound");

        return soundsDictionary[id];
    }

    public AudioClip GetMusicClip(string id)
    {
        if (musicDictionary[id] == null)
            Debug.LogWarning("Wrong ID, no matching sound");

        return musicDictionary[id];
    }

    public void FadeSound(AudioSource source, float targetVolume, float time)
    {
        float originalVolume = source.volume;
        LeanTween.value(source.gameObject, originalVolume, targetVolume, time).setOnUpdate((float value) =>
        {
            source.volume = value;
        }).setEaseOutSine();
    }

    public void FadeSound(AudioSource source, float targetVolume, float time, Action onCompleteAction)
    {
        float originalVolume = source.volume;
        LeanTween.value(source.gameObject, originalVolume, targetVolume, time).setOnUpdate((float value) =>
        {
            source.volume = value;
        }).setEaseOutSine().setOnComplete(()=> onCompleteAction.Invoke());
    }

    /// <summary>
    /// Allows crossfade between two audio sources.
    /// </summary>
    /// <param name="sourceA">the source fading out and stopping.</param>
    /// <param name="sourceB">the source starting and fading in.</param>
    /// <param name="time">Crossfade duration.</param>
    /// <param name="onCompleteAction">Action invoked on crossfade complete</param>
    public void CrossFade(AudioSource sourceA, AudioSource sourceB, float time, Action onCompleteAction = null)
    {
        float sourceAOriginVolume = sourceA.volume;
        sourceB.volume = 0;
        sourceB.Play();
        LeanTween.value(sourceB.gameObject, 0, sourceAOriginVolume, time).setOnUpdate((float value)=>
        {
            sourceB.volume = value;
        }).setEaseInSine();

        LeanTween.value(sourceB.gameObject, sourceAOriginVolume, 0, time).setOnUpdate((float value) =>
        {
            sourceA.volume = value;
        }).setEaseOutSine().setOnComplete(()=> 
        {
            sourceA.Stop();
            sourceA.volume = sourceAOriginVolume;
            if (onCompleteAction != null) onCompleteAction.Invoke();
        });
    }

    /// <summary>
    /// Allows crossfade between two audio sources.
    /// </summary>
    /// <param name="sourceA">the source fading out and stopping.</param>
    /// <param name="sourceB">the source starting and fading in.</param>
    /// <param name="sourceBClip">the clip to play on sourceB.</param>
    /// <param name="time">Crossfade duration.</param>
    /// <param name="onCompleteAction">Action invoked on crossfade complete</param>
    public void CrossFade(AudioSource sourceA, AudioSource sourceB, AudioClip sourceBClip, float time, Action onCompleteAction = null)
    {
        float sourceAOriginVolume = sourceA.volume;
        sourceB.clip = sourceBClip;
        sourceB.volume = 0;
        sourceB.Play();
        LeanTween.value(sourceB.gameObject, 0, sourceAOriginVolume, time).setOnUpdate((float value) =>
        {
            sourceB.volume = value;
        }).setEaseInSine();

        LeanTween.value(sourceB.gameObject, sourceAOriginVolume, 0, time).setOnUpdate((float value) =>
        {
            sourceA.volume = value;
        }).setEaseOutSine().setOnComplete(() =>
        {
            sourceA.Stop();
            sourceA.volume = sourceAOriginVolume;
            if (onCompleteAction != null) onCompleteAction.Invoke();
        });
    }

    public AudioSource GenerateAudioSource(GameObject go)
    {
        AudioSource source =  go.AddComponent<AudioSource>();
        return source;
    }

    #region Music

    AudioSource musicSourceOne;
    AudioSource musicSourceTwo;
    List<AudioSource> allMusicSources
    {
        get { return new List<AudioSource>() { musicSourceOne, musicSourceTwo }; }
    }

    List<string> playlist = new List<string>();

    Coroutine currentPlaying;

    public void InitMusicQueue()
    {
        if (musicSourceOne == null) musicSourceOne = SoundManager.Instance.GenerateAudioSource(gameObject);
        if (musicSourceTwo == null) musicSourceTwo = SoundManager.Instance.GenerateAudioSource(gameObject);

        for(int i = 3; i<8; i++)
            playlist.Add("MUS_THEME0" + i.ToString());

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        LaunchMusic(arg0.buildIndex);
    }

    void LaunchMusic(int sceneIndex)
    {
        var possibleSources = allMusicSources.Where(s => !s.isPlaying);
        AudioSource availableSource = null;

        if (possibleSources.Count() <= 0)
        {
            availableSource = allMusicSources.Where(s => s.time == allMusicSources.Max(source => source.time)).First();
            availableSource.Stop();
        }
        else
            availableSource = allMusicSources.Where(s => !s.isPlaying).First();


        AudioSource busySource = allMusicSources.Where(s => s.isPlaying).FirstOrDefault();
        Sound soundToPlay = null;

        //Menu Music
        if (sceneIndex == menuScene)
        {
            if (currentPlaying != null) StopCoroutine(currentPlaying);
            soundToPlay = new Sound(availableSource, "MUS_TITLE", SoundType.MUSIC, true, false);
        }
        //Game Music
        else if(sceneIndex == gameScene)
        {
            soundToPlay = new Sound(availableSource, playlist[UnityEngine.Random.Range((int)0, (int)playlist.Count)], SoundType.MUSIC, false, false);
            currentPlaying = StartCoroutine(CurrentPlayingMusic(soundToPlay.currentSource.clip.length - 5));
        }
         

        if (busySource != null)
        {
            CrossFade(busySource, availableSource, 5f);
        }
        else
        {
            availableSource.volume = 0;
            soundToPlay.Play();
            FadeSound(musicSourceOne, 1f, 10f);
        }
    }

    IEnumerator CurrentPlayingMusic(float musicLength)
    {
        float timeElapsed = 0;
        while (timeElapsed < musicLength)
        {
            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }

        AudioSource availableSource = allMusicSources.Where(s => !s.isPlaying).First();
        AudioSource busySource = allMusicSources.Where(s => s.isPlaying).FirstOrDefault();

        Sound soundToPlay = new Sound(availableSource, playlist[UnityEngine.Random.Range((int)0, (int)playlist.Count)], SoundType.MUSIC, false, false);
        CrossFade(busySource, availableSource, 5f);
        currentPlaying = StartCoroutine(CurrentPlayingMusic(soundToPlay.currentSource.clip.length - 5));
    }

    #endregion
}

public enum SoundType
{
    MUSIC, 
    SFX
}
public class Sound
{
    public AudioSource currentSource;
    public string id;
    public SoundType type;

    public Sound(AudioSource targetSource, string id, SoundType type, bool loop, bool autoPlay)
    {
        currentSource = targetSource;
        this.id = id;
        this.type = type;

        AudioClip clip;
        switch (type)
        {
            case SoundType.MUSIC:
                clip = SoundManager.Instance.GetMusicClip(id);
                currentSource.outputAudioMixerGroup = SoundManager.Instance.musicGroup;
                break;
            case SoundType.SFX:
                clip = SoundManager.Instance.GetSoundClip(id);
                currentSource.outputAudioMixerGroup = SoundManager.Instance.sfxGroup;
                break;
            default:
                clip = null;
                break;
        }

        targetSource.clip = clip;
        targetSource.loop = loop;

        if (autoPlay)
            targetSource.Play();
    }

    public void Play()
    {
        currentSource.Play();
    }

    public void Stop()
    {
        currentSource.Stop();
    }

    public void Fade(float targetVolume, float time, Action onCompleteAction = null)
    {
        if (onCompleteAction == null) SoundManager.Instance.FadeSound(currentSource, targetVolume, time);
        else SoundManager.Instance.FadeSound(currentSource, targetVolume, time, onCompleteAction);
    }

    public void CrossFade(float time, Sound otherSound, Action onCompleteAction = null)
    {
        SoundManager.Instance.CrossFade(currentSource, otherSound.currentSource, time, onCompleteAction);
    }
}

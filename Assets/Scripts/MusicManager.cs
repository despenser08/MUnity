using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioManager audioManager;
    public UIManager uiManager;
    public AddMusic addMusic;
    public List<AudioClip> musics;
    public List<string> musicPathList = new List<string>();
    public int musicIndex = 0;

    public bool pause = true;
    [Range(0, 100)] public float volume = 100f;
    public bool loop = true;
    public float pitch = 1f;

    public float interval = 2f;
    public float pressInterval = 0.2f;
    
    public float volumePressed = 0.5f;
    public float durationPressed = 0.1f;

    private float muteVolume;
    private static string saveFolder;
    private const string SaveFile = "/status.json";

    private bool firstPressed = true;

    private void Start()
    {
        saveFolder = Application.dataPath;
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        if (File.Exists(saveFolder + SaveFile)) StartCoroutine(LoadCoroutine());
        else File.Create(saveFolder + SaveFile);

        if (musics.Count > 0)
            audioManager.Play(musics[musicIndex]);
    }

    private void Update()
    {
        audioManager.Pause(pause);
        audioManager.Volume(volume / 100f);
        audioManager.Pitch(pitch);

        if (musics.Count > 0 && audioManager.audioSource.time >= musics[musicIndex].length && !pause)
        {
            pause = true;
            Invoke(nameof(PlayNext), interval);
        }

        Save();

        KeyControl();
    }

    private void KeyControl()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (firstPressed)
                VolumeUp(10);
            Invoke(nameof(VolumeUp), pressInterval);
            firstPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            CancelInvoke(nameof(VolumeUp));
            firstPressed = true;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (firstPressed)
                VolumeDown(10);
            Invoke(nameof(VolumeDown), pressInterval);
            firstPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            CancelInvoke(nameof(VolumeDown));
            firstPressed = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (firstPressed)
                DurationUp(5);
            Invoke(nameof(DurationUp), pressInterval);
            firstPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            CancelInvoke(nameof(DurationUp));
            firstPressed = true;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (firstPressed)
                DurationDown(5);
            Invoke(nameof(DurationDown), pressInterval);
            firstPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            CancelInvoke(nameof(DurationDown));
            firstPressed = true;
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            if (firstPressed)
                Pause();
            Invoke(nameof(Pause), pressInterval);
            firstPressed = false;
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CancelInvoke(nameof(Pause));
            firstPressed = true;
        }
    }

    private void VolumeUp(int volumeAdded)
    {
        if (volume <= 100 - volumeAdded)
            volume += volumeAdded;
        else volume = 100;
        uiManager.ShowVolumeControl();
    }

    private void VolumeUp()
    {
        if (volume <= 100 - volumePressed)
            volume += volumePressed;
        else volume = 100;
        uiManager.ShowVolumeControl();
    }

    private void VolumeDown(int volumeSubtracted)
    {
        if (volume >= volumeSubtracted)
            volume -= volumeSubtracted;
        else volume = 0;
        uiManager.ShowVolumeControl();
    }

    private void VolumeDown()
    {
        if (volume >= volumePressed)
            volume -= volumePressed;
        else volume = 0;
        uiManager.ShowVolumeControl();
    }

    private void DurationUp(int durationAdded)
    {
        if (audioManager.audioSource.clip.length > audioManager.audioSource.time + durationAdded)
            audioManager.audioSource.time += durationAdded;
        else audioManager.audioSource.time = audioManager.audioSource.clip.length;
    }
    
    private void DurationUp()
    {
        if (audioManager.audioSource.clip.length > audioManager.audioSource.time + durationPressed)
            audioManager.audioSource.time += durationPressed;
        else audioManager.audioSource.time = audioManager.audioSource.clip.length;
    }

    private void DurationDown(int durationSubtracted)
    {
        if (0 < audioManager.audioSource.time - durationSubtracted)
            audioManager.audioSource.time -= durationSubtracted;
        else audioManager.audioSource.time = 0;
    }
    
    private void DurationDown()
    {
        if (0 < audioManager.audioSource.time - durationPressed)
            audioManager.audioSource.time -= durationPressed;
        else audioManager.audioSource.time = 0;
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void PlayQueued(AudioClip clip)
    {
        var index = musics.IndexOf(clip);

        if (index <= -1) return;
        if (musicIndex == index) return;

        musicIndex = index;
        audioManager.Play(musics[musicIndex]);
    }

    public void PlayIndex(int index)
    {
        if (index >= musics.Count || index <= -1) return;

        if (musicIndex == index) return;

        musicIndex = index;
        audioManager.Play(musics[musicIndex]);
    }

    public void PlayNext()
    {
        if (musics.Count <= 0) return;

        if (++musicIndex < musics.Count)
            audioManager.Play(musics[musicIndex]);
        else if (loop)
        {
            musicIndex = 0;
            audioManager.Play(musics[musicIndex]);
        }

        pause = false;
    }

    public void SkipPrevious()
    {
        if (audioManager.audioSource.time <= 10) PlayPrevious();
        else audioManager.audioSource.time = 0;
    }

    public void PlayPrevious()
    {
        if (musics.Count <= 0) return;

        if (--musicIndex < 0) musicIndex = musics.Count - 1;
        if (musicIndex < musics.Count)
            audioManager.Play(musics[musicIndex]);
        else if (loop)
        {
            musicIndex = 0;
            audioManager.Play(musics[musicIndex]);
        }

        pause = false;
    }

    public void Pause()
    {
        if (audioManager.audioSource.clip)
            pause = !pause;
    }

    public void AddVolume(float volumeAdded)
    {
        if (volume + volumeAdded >= 0 && volume + volumeAdded <= 100)
            volume += volumeAdded;
        else if (volume + volumeAdded < 0) volume = 0;
        else if (volume + volumeAdded > 100) volume = 100;
    }

    public void Mute()
    {
        if (volume != 0)
        {
            muteVolume = volume;
            volume = 0;
        }
        else volume = muteVolume;
    }

    private void Save()
    {
        var save = new Status
        {
            musicIndex = musicIndex,
            duration = audioManager.audioSource.time,
            pause = pause,
            volume = volume,
            loop = loop,
            pitch = pitch,
            interval = interval,
            pressInterval = pressInterval,
            volumePressed = volumePressed,
            durationPressed = durationPressed,
            musicPaths = musicPathList
        };

        var load = File.ReadAllText(saveFolder + SaveFile);
        var saveStr = JsonUtility.ToJson(save);

        if (saveStr != load)
            File.WriteAllText(saveFolder + SaveFile, saveStr);
    }

    private IEnumerator LoadCoroutine()
    {
        var save = File.ReadAllText(saveFolder + SaveFile);
        var parsed = JsonUtility.FromJson<Status>(save);

        if (parsed == null) yield break;

        uiManager.LoadingScreen(true);
        yield return new WaitUntil(() => uiManager.loadingScreen.activeSelf);

        volume = parsed.volume;
        yield return new WaitUntil(() => volume == parsed.volume);

        foreach (var audioPath in parsed.musicPaths)
        {
            var info = new FileInfo(audioPath);
            if (info.Exists)
                addMusic.LoadSong(info);
            else parsed.musicPaths.Remove(audioPath);
        }

        yield return new WaitUntil(() => parsed.musicPaths.Count <= musics.Count);

        pause = parsed.pause;
        pitch = parsed.pitch;
        loop = parsed.loop;
        interval = parsed.interval;

        pressInterval = parsed.pressInterval;
        volumePressed = parsed.volumePressed;
        durationPressed = parsed.durationPressed;

        PlayIndex(parsed.musicIndex);

        audioManager.audioSource.time = parsed.duration;
        yield return new WaitUntil(() => audioManager.audioSource.time == parsed.duration);

        uiManager.LoadingScreen(false);
    }

    private class Status
    {
        public int musicIndex;
        public float duration;
        public bool pause;
        public float volume;
        public bool loop;
        public float pitch;
        public float interval;
        public float pressInterval;
        public float volumePressed;
        public float durationPressed;
        public List<string> musicPaths;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioManager audioManager;
    public UIManager UIManager;
    public AddMusic addMusic;
    public List<AudioClip> musics;
    public List<string> musicPathList = new List<string>();
    public int musicIndex = 0;

    public bool pause = true;
    [Range(0, 100)]
    public float volume = 100f;
    public bool loop = true;
    public float pitch = 1f;

    public float interval = 2f;

    private static string SAVE_FOLDER;
    private static readonly string SAVE_FILE = "/status.json";

    void Start()
    {
        SAVE_FOLDER = Application.dataPath;
        if (!Directory.Exists(SAVE_FOLDER)) Directory.CreateDirectory(SAVE_FOLDER);

        if (File.Exists(SAVE_FOLDER + SAVE_FILE)) StartCoroutine(LoadCoroutine());

        if (musics.Count > 0)
            audioManager.Play(musics[musicIndex]);
    }

    void Update()
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
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (volume <= 90) volume += 10;
            else volume = 100;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (volume >= 10) volume -= 10;
            else volume = 0;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) audioManager.audioSource.time += 5;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) audioManager.audioSource.time -= 5;
        else if (Input.GetKeyDown(KeyCode.Space)) Pause();
    }

    void OnApplicationQuit()
    {
        Save();
    }

    public void PlayQueued(AudioClip clip)
    {
        int index = musics.IndexOf(clip);
        if (index > -1)
        {
            if (musicIndex != index)
            {
                musicIndex = index;
                audioManager.Play(musics[musicIndex]);
            }
        }
    }

    public void PlayIndex(int index)
    {
        if (index < musics.Count && index > -1)
        {
            if (musicIndex != index)
            {
                musicIndex = index;
                audioManager.Play(musics[musicIndex]);
            }
        }
    }

    public void PlayNext()
    {
        if (musics.Count > 0)
        {
            if (++musicIndex < musics.Count)
                audioManager.Play(musics[musicIndex]);
            else if (loop)
            {
                musicIndex = 0;
                audioManager.Play(musics[musicIndex]);
            }

            pause = false;
        }
    }

    public void SkipPrevious()
    {
        if (audioManager.audioSource.time <= 10) PlayPrevious();
        else audioManager.audioSource.time = 0;
    }

    public void PlayPrevious()
    {
        if (musics.Count > 0)
        {
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
    }

    public void Pause()
    {
        if (audioManager.audioSource.clip)
            pause = !pause;
    }

    void Save()
    {
        Status save = new Status
        {
            musicIndex = musicIndex,
            duration = audioManager.audioSource.time,
            pause = pause,
            volume = volume,
            loop = loop,
            pitch = pitch,
            interval = interval,
            musicPaths = musicPathList
        };

        File.WriteAllText(SAVE_FOLDER + SAVE_FILE, JsonUtility.ToJson(save));
    }

    IEnumerator LoadCoroutine()
    {
        string save = File.ReadAllText(SAVE_FOLDER + SAVE_FILE);
        Status parsed = JsonUtility.FromJson<Status>(save);

        if (parsed != null)
        {
            UIManager.LoadingScreen(true);
            yield return new WaitUntil(() => UIManager.loadingScreen.activeSelf);

            volume = parsed.volume;
            yield return new WaitUntil(() => volume == parsed.volume);

            foreach (string audioPath in parsed.musicPaths)
            {
                FileInfo info = new FileInfo(audioPath);
                if (info.Exists)
                    addMusic.LoadSong(info);
                else parsed.musicPaths.Remove(audioPath);
            }
            yield return new WaitUntil(() => parsed.musicPaths.Count <= musics.Count);

            pause = parsed.pause;
            pitch = parsed.pitch;
            loop = parsed.loop;
            interval = parsed.interval;

            PlayIndex(parsed.musicIndex);

            audioManager.audioSource.time = parsed.duration;
            yield return new WaitUntil(() => audioManager.audioSource.time == parsed.duration);

            UIManager.LoadingScreen(false);
        }

    }

    class Status
    {
        public int musicIndex;
        public float duration;
        public bool pause;
        public float volume;
        public bool loop;
        public float pitch;
        public float interval;
        public List<string> musicPaths;
    }

}

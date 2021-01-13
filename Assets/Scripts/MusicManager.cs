using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioManager audioManager;
    public List<AudioClip> musics;
    public int musicIndex = 0;

    public bool pause = false;
    [Range(0, 100)]
    public float volume = 100f;
    public bool loop = true;
    public float pitch = 1f;

    public float interval = 2f;

    void Start()
    {
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

}

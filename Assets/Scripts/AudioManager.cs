using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
    }

    public void Pause(bool pause)
    {
        if (pause) audioSource.Pause();
        else audioSource.UnPause();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public bool Loop(bool loop)
    {
        return audioSource.loop = loop;
    }

    public bool Loop()
    {
        return audioSource.loop;
    }

    public float Volume(float volume)
    {
        return audioSource.volume = volume;
    }

    public float Volume()
    {
        return audioSource.volume;
    }

    public float Pitch(float pitch)
    {
        return audioSource.pitch = pitch;
    }

    public float Pitch()
    {
        return audioSource.pitch;
    }
}

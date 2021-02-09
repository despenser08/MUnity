using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public DiscordController discordController;

    public void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
        discordController.UpdateActivity("Listening " + clip.name);
    }

    public void Pause(bool pause)
    {
        if (pause) audioSource.Pause();
        else audioSource.UnPause();
    }

    public void Stop() => audioSource.Stop();

    public bool Loop(bool loop) => audioSource.loop = loop;

    public bool Loop() => audioSource.loop;

    public float Volume(float volume) => audioSource.volume = volume;

    public float Volume() => audioSource.volume;

    public float Pitch(float pitch) => audioSource.pitch = pitch;

    public float Pitch() => audioSource.pitch;
}
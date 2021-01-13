using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public MusicManager musicManager;
    public Text title;
    public Text duration;
    public Slider durationBar;
    public Image playPauseImage;
    public Sprite[] playPause;
    public GameObject playlistContent;
    public GameObject playlistMusicPrefab;
    public Camera mainCamera;
    public bool rainbow = false;

    private float nowDuration = 0f;
    private float fullDuration = 0f;

    private Dictionary<AudioClip, GameObject> playlistMusicDict = new Dictionary<AudioClip, GameObject>();
    private float playListMusicY = 125f;

    void Update()
    {
        if (musicManager.audioManager.audioSource.clip)
        {
            nowDuration = musicManager.audioManager.audioSource.time;
            fullDuration = musicManager.audioManager.audioSource.clip.length;

            title.text = musicManager.audioManager.audioSource.clip.name;
            durationBar.maxValue = fullDuration;
            durationBar.value = nowDuration;

            duration.text = FloatToTime(nowDuration) + " / " + FloatToTime(fullDuration);

            if (musicManager.pause) playPauseImage.sprite = playPause[0];
            else playPauseImage.sprite = playPause[1];
        } else
        {
            title.text = "Nothing to play";
            duration.text = "0:00 / 0:00";
        }

        if (rainbow && !musicManager.pause)
            mainCamera.backgroundColor = Color.HSVToRGB(nowDuration / fullDuration, 1f, 1f);
        else mainCamera.backgroundColor = Color.black;
    }

    string FloatToTime(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        return string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
    }

    public void SetDuration(Slider slider)
    {
        if (musicManager.audioManager.audioSource.clip)
            if (musicManager.audioManager.audioSource.clip.length > slider.value)
                musicManager.audioManager.audioSource.time = slider.value;
    }

    public void AddMusic(AudioClip clip)
    {
        GameObject playlistMusic = Instantiate(playlistMusicPrefab, playlistContent.transform);

        Text[] texts = playlistMusic.GetComponentsInChildren<Text>();
        texts[0].text = clip.name;
        texts[1].text = FloatToTime(clip.length);

        playlistMusic.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveMusic(clip));

        playlistMusic.transform.localPosition = new Vector3(91.5f, playListMusicY - 150);
        playlistMusic.transform.localScale = new Vector3(1, 1, 1);
        playListMusicY -= 40;

        playlistMusicDict.Add(clip, playlistMusic);
    }

    void RemoveMusic(AudioClip clip)
    {
        int index = musicManager.musics.IndexOf(clip);
        MovePlaylistMusic(index);
        Destroy(playlistMusicDict[clip]);
        musicManager.musics.Remove(clip);
        playlistMusicDict.Remove(clip);
        if (index <= musicManager.musicIndex) musicManager.musicIndex--;
        if (musicManager.audioManager.audioSource.clip == clip)
        {
            musicManager.audioManager.audioSource.clip = null;
            musicManager.PlayNext();
        }
    }

    void MovePlaylistMusic(int index)
    {
        playListMusicY += 40;
        foreach (AudioClip clip in musicManager.musics.GetRange(index, musicManager.musics.Count - 1))
            playlistMusicDict[clip].transform.position += new Vector3(0, 55);
    }

}
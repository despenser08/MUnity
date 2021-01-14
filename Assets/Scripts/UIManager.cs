using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public MusicManager musicManager;
    public Text title;
    public Text duration;
    public GameObject loadingScreen;
    public Slider durationBar;
    public Image playPauseImage;
    public Sprite[] playPause;
    public GameObject playlistContent;
    public GameObject playlistMusicPrefab;
    public Camera mainCamera;
    public bool rainbow;

    private float nowDuration;
    private float fullDuration;

    private readonly Dictionary<AudioClip, GameObject> playlistMusicDict = new Dictionary<AudioClip, GameObject>();

    private void Update()
    {
        if (musicManager.audioManager.audioSource.clip)
        {
            nowDuration = musicManager.audioManager.audioSource.time;
            var playingClip = musicManager.audioManager.audioSource.clip;
            fullDuration = playingClip.length;

            title.text = playingClip.name;
            durationBar.maxValue = fullDuration;
            durationBar.value = nowDuration;

            duration.text = FloatToTime(nowDuration) + " / " + FloatToTime(fullDuration);

            playPauseImage.sprite = musicManager.pause ? playPause[0] : playPause[1];

            foreach (var clip in musicManager.musics)
                playlistMusicDict[clip].GetComponentInChildren<Image>().enabled =
                    clip == musicManager.audioManager.audioSource.clip;
        }
        else
        {
            title.text = "Nothing to play";
            duration.text = "0:00 / 0:00";
            durationBar.value = 0;
        }

        if (rainbow && !musicManager.pause)
            mainCamera.backgroundColor = Color.HSVToRGB(nowDuration / fullDuration, 1f, 1f);
        else mainCamera.backgroundColor = Color.black;
    }

    private static string FloatToTime(float time)
    {
        var t = TimeSpan.FromSeconds(time);
        if (t.Hours == 0)
            return $"{t.Minutes:D1}:{t.Seconds:D2}";

        return t.Days == 0
            ? $"{t.Hours:D1}:{t.Minutes:D1}:{t.Seconds:D2}"
            : $"{t.Days:D1}:{t.Hours:D1}:{t.Minutes:D1}:{t.Seconds:D2}";
    }

    public void SetDuration(Slider slider)
    {
        if (!musicManager.audioManager.audioSource.clip) return;

        if (musicManager.audioManager.audioSource.clip.length > slider.value)
            musicManager.audioManager.audioSource.time = slider.value;
    }

    public void AddMusic(AudioClip clip)
    {
        var playlistMusic = Instantiate(playlistMusicPrefab, playlistContent.transform);

        if (playlistMusicDict.Count > 4)
            playlistContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 40);

        var texts = playlistMusic.GetComponentsInChildren<Text>();
        texts[0].text = clip.name;
        texts[1].text = FloatToTime(clip.length);

        var buttons = playlistMusic.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => musicManager.PlayQueued(clip));
        buttons[1].onClick.AddListener(() => RemoveMusic(clip));

        playlistMusic.transform.localPosition -= new Vector3(0, 40 * playlistMusicDict.Count);
        playlistMusic.transform.localScale = new Vector3(1, 1, 1);

        playlistMusicDict.Add(clip, playlistMusic);
    }

    private void RemoveMusic(AudioClip clip)
    {
        var index = musicManager.musics.IndexOf(clip);

        MovePlaylistMusic(index);
        Destroy(playlistMusicDict[clip]);

        musicManager.musics.Remove(clip);
        musicManager.musicPathList.RemoveAt(index);
        playlistMusicDict.Remove(clip);

        if (index <= musicManager.musicIndex) musicManager.musicIndex--;

        if (musicManager.audioManager.audioSource.clip != clip) return;

        musicManager.audioManager.audioSource.clip = null;
        musicManager.PlayNext();
    }

    private void MovePlaylistMusic(int index)
    {
        if (playlistMusicDict.Count > 4)
            playlistContent.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 40);

        foreach (var clip in musicManager.musics.GetRange(index, musicManager.musics.Count - index)
            .Where(clip => playlistMusicDict.ContainsKey(clip)))
            playlistMusicDict[clip].transform.localPosition += new Vector3(0, 40);
    }

    public void LoadingScreen(bool enable) => loadingScreen.SetActive(enable);
}
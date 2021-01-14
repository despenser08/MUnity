using System;
using System.Collections.Generic;
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
    public bool rainbow = false;

    private float nowDuration = 0f;
    private float fullDuration = 0f;

    private Dictionary<AudioClip, GameObject> playlistMusicDict = new Dictionary<AudioClip, GameObject>();

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

            foreach (AudioClip clip in musicManager.musics)
            {
                if (clip == musicManager.audioManager.audioSource.clip)
                    playlistMusicDict[clip].GetComponentInChildren<Image>().enabled = true;
                else playlistMusicDict[clip].GetComponentInChildren<Image>().enabled = false;
            }
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

    string FloatToTime(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        if (t.Hours == 0)
            return string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
        else if (t.Days == 0) return string.Format("{0:D1}:{0:D2}:{1:D3}", t.Hours, t.Minutes, t.Seconds);
        else return string.Format("{0:D1}:{0:D2}:{0:D3}:{1:D4}", t.Days, t.Hours, t.Minutes, t.Seconds);
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

        if (playlistMusicDict.Count > 4)
            playlistContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 40);

        Text[] texts = playlistMusic.GetComponentsInChildren<Text>();
        texts[0].text = clip.name;
        texts[1].text = FloatToTime(clip.length);

        Button[] buttons = playlistMusic.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => musicManager.PlayQueued(clip));
        buttons[1].onClick.AddListener(() => RemoveMusic(clip));

        playlistMusic.transform.localPosition -= new Vector3(0, 40 * playlistMusicDict.Count);
        playlistMusic.transform.localScale = new Vector3(1, 1, 1);

        playlistMusicDict.Add(clip, playlistMusic);
    }

    void RemoveMusic(AudioClip clip)
    {
        int index = musicManager.musics.IndexOf(clip);

        MovePlaylistMusic(index);
        Destroy(playlistMusicDict[clip]);

        musicManager.musics.Remove(clip);
        musicManager.musicPathList.RemoveAt(index);
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
        if (playlistMusicDict.Count > 4)
            playlistContent.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 40);

        foreach (AudioClip clip in musicManager.musics.GetRange(index, musicManager.musics.Count - index))
        {
            if (playlistMusicDict.ContainsKey(clip))
                playlistMusicDict[clip].transform.localPosition += new Vector3(0, 40);
        }
    }

    public void LoadingScreen(bool enable) => loadingScreen.SetActive(enable);

}
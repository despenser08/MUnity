using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class AddMusic : MonoBehaviour
{
    public MusicManager musicManager;
    public UIManager UIManager;
    private readonly List<string> supportedFormats = new List<string> { ".mp3", ".ogg", ".wav", ".aiff", ".aif", ".mod", ".it", ".s3m", ".xm" };

    public void FileSelect()
    {
        string path = EditorUtility.OpenFilePanel("Select a Music", "", string.Join(", ", supportedFormats.ConvertAll(new System.Converter<string, string>(RemoveFirstChar))));

        if (path != null && path != "")
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists && supportedFormats.Contains(fileInfo.Extension))
                LoadSong(fileInfo);
            else
            {
                bool reselect = EditorUtility.DisplayDialog("Error", "Unsupported file type.\nSupported file types: " + string.Join(", ", supportedFormats), "Reselect", "Cancel");
                if (reselect) FileSelect();
            }
        }
    }

    private string RemoveFirstChar(string format) => format.Substring(1);

    public void LoadSong(FileInfo fileInfo) => StartCoroutine(LoadSongCoroutine(fileInfo));

    IEnumerator LoadSongCoroutine(FileInfo fileInfo)
    {
        UnityWebRequest audioFile = UnityWebRequestMultimedia.GetAudioClip("file://" + fileInfo.FullName, AudioType.UNKNOWN);
        yield return audioFile.SendWebRequest();

        if (!audioFile.isNetworkError || !audioFile.isHttpError)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(audioFile);
            if (clip)
            {
                clip.name = fileInfo.Name.Replace(fileInfo.Extension, "");

                musicManager.musics.Add(clip);
                musicManager.musicPathList.Add(fileInfo.FullName);
                UIManager.AddMusic(clip);
                if (musicManager.musics.Count == 1) musicManager.PlayNext();
            }
        }
        else
        {
            Debug.LogError(audioFile.error);
            bool reselect = EditorUtility.DisplayDialog("Error", "An error has occurred. Please try again.", "Reselect", "Cancel");
            if (reselect) FileSelect();
        }
    }
}

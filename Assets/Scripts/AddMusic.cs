using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class AddMusic : MonoBehaviour
{
    public MusicManager musicManager;

    private readonly List<string> supportedFormats = new List<string>
        {".mp3", ".ogg", ".wav", ".aiff", ".aif", ".mod", ".it", ".s3m", ".xm"};

    public void FileSelect()
    {
        while (true)
        {
            var path = EditorUtility.OpenFilePanel("Select a Music", "",
                string.Join(", ", supportedFormats.ConvertAll(RemoveFirstChar)));

            if (string.IsNullOrEmpty(path)) return;
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists && supportedFormats.Contains(fileInfo.Extension))
                LoadSong(fileInfo);
            else
            {
                var reselect = EditorUtility.DisplayDialog("Error",
                    "Unsupported file type.\nSupported file types: " + string.Join(", ", supportedFormats), "Reselect",
                    "Cancel");
                if (reselect) continue;
            }

            break;
        }
    }

    private static string RemoveFirstChar(string format) => format.Substring(1);

    public void LoadSong(FileInfo fileInfo) => StartCoroutine(LoadSongCoroutine(fileInfo));

    private IEnumerator LoadSongCoroutine(FileInfo fileInfo)
    {
        musicManager.uiManager.LoadingScreen(true);

        var audioFile = UnityWebRequestMultimedia.GetAudioClip("file://" + fileInfo.FullName, AudioType.UNKNOWN);
        yield return audioFile.SendWebRequest();

        if (!audioFile.isNetworkError || !audioFile.isHttpError)
        {
            var clip = DownloadHandlerAudioClip.GetContent(audioFile);
            if (clip)
            {
                clip.name = fileInfo.Name.Replace(fileInfo.Extension, "");

                musicManager.musics.Add(clip);
                musicManager.musicPathList.Add(fileInfo.FullName);
                musicManager.uiManager.AddMusic(clip);
                if (musicManager.musics.Count == 1) musicManager.PlayNext();
            }
        }
        else
        {
            Debug.LogError(audioFile.error);
            var reselect = EditorUtility.DisplayDialog("Error", "An error has occurred. Please try again.", "Reselect",
                "Cancel");
            if (reselect) FileSelect();
        }

        musicManager.uiManager.LoadingScreen(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SFB;
using UnityEngine;
using UnityEngine.Networking;

public class AddMusic : MonoBehaviour
{
    public MusicManager musicManager;

    private readonly List<string> supportedFormats = new List<string>
        {"mp3", "ogg", "wav", "aiff", "aif", "mod", "it", "s3m", "xm"};

    public void FileSelect()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select a Music", "",
            new[] {new ExtensionFilter("Sound Files", supportedFormats.ToArray())}
            , true);

        if (paths.Length < 1) return;

        foreach (var path in paths)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists && supportedFormats.Contains(fileInfo.Extension.Substring(1)))
                LoadSong(fileInfo);
            else
            {
                MessageBox.Show(
                    "Can't load \"" + path + "\"\nUnsupported file type.\nSupported file types: " +
                    string.Join(", ", supportedFormats), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

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
            MessageBox.Show("An error has occurred on loading \"" + fileInfo.FullName + "\". Please try again.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        musicManager.uiManager.LoadingScreen(false);
    }
}
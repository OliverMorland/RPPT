using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;
using System.ComponentModel;
using Debug = UnityEngine.Debug;
using AnotherFileBrowser.Windows;

public class ModelAudioLoader : MonoBehaviour
{
    string path;
    public UnityEvent<AudioClip> m_uploadComplete;
    public UnityEvent m_startButtonClicked;
    [SerializeField] TMP_Text m_fileNameLabel;
    [SerializeField] AudioClip m_modelAudioClip;
    [SerializeField] Button m_startButton;
    [SerializeField] TMP_Text m_captionInput;

    private void Start()
    {
        m_startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void OnStartButtonClicked()
    {
        m_startButtonClicked.Invoke();
    }

    public void OpenFileExplorer()
    {
        //path = EditorUtility.OpenFilePanel("Find Model Audio Track", "", "mp3");
        var bp = new BrowserProperties();
        bp.filter = "Image files (*.mp3) | *.mp3";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            //Load image from local path with UWR
            StartCoroutine(LoadAudioClip(path));
        });
    }

    IEnumerator LoadAudioClip(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Could not load audio clip, " + www.error);
                m_fileNameLabel.text = "No Audio Clip Selected";
                m_fileNameLabel.color = Color.red;
            }
            else
            {
                m_modelAudioClip = DownloadHandlerAudioClip.GetContent(www);
                m_modelAudioClip.name = "Model Audio Clip";
                m_fileNameLabel.text = path;
                m_uploadComplete.Invoke(m_modelAudioClip);
            }
        }
    }

    public void EnableStartButton()
    {
        m_startButton.interactable = true;
    }

    public void DisableStartButton()
    {
        m_startButton.interactable = false;
    }

    public string GetCaptionText()
    {
        return m_captionInput.text;
    }

    //Sample code for opening on mac and PC
    //public static void OpenInMacFileBrowser(string path)
    //{
    //    bool openInsidesOfFolder = false;

    //    // try mac
    //    string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

    //    if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
    //    {
    //        openInsidesOfFolder = true;
    //    }

    //    //Debug.Log("macPath: " + macPath);
    //    //Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

    //    if (!macPath.StartsWith("\""))
    //    {
    //        macPath = "\"" + macPath;
    //    }
    //    if (!macPath.EndsWith("\""))
    //    {
    //        macPath = macPath + "\"";
    //    }
    //    string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
    //    //Debug.Log("arguments: " + arguments);
    //    try
    //    {
    //        System.Diagnostics.Process.Start("open", arguments);
    //    }
    //    catch (System.ComponentModel.Win32Exception e)
    //    {
    //        // tried to open mac finder in windows
    //        // just silently skip error
    //        // we currently have no platform define for the current OS we are in, so we resort to t$$anonymous$$s
    //        e.HelpLink = ""; // do anyt$$anonymous$$ng with t$$anonymous$$s variable to silence warning about not using it
    //    }
    //}

    //public static void OpenInWinFileBrowser(string path)
    //{
    //    bool openInsidesOfFolder = false;

    //    // try windows
    //    string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

    //    if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
    //    {
    //        openInsidesOfFolder = true;
    //    }
    //    try
    //    {
    //        System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
    //    }
    //    catch (System.ComponentModel.Win32Exception e)
    //    {
    //        // tried to open win explorer in mac
    //        // just silently skip error
    //        // we currently have no platform define for the current OS we are in, so we resort to t$$anonymous$$s
    //        e.HelpLink = ""; // do anyt$$anonymous$$ng with t$$anonymous$$s variable to silence warning about not using it
    //    }
    //}

    //public static void OpenInFileBrowser(string path)
    //{
    //    OpenInWinFileBrowser(path);
    //    OpenInMacFileBrowser(path);
    //}


}

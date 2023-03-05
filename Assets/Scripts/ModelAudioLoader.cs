using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

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
        path = EditorUtility.OpenFilePanel("Find Model Audio Track", "", "mp3");
        StartCoroutine(LoadAudioClip(path));
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


}

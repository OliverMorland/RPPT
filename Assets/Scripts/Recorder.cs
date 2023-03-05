using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Recorder : MonoBehaviour
{
    [SerializeField] Button m_recordButton;
    [SerializeField] Button m_toggleMicButton;
    [SerializeField] Sprite m_notRecordingIcon;
    [SerializeField] Sprite m_isRecordingIcon;
    [SerializeField] TMP_Text m_buttonText;
    [SerializeField] int m_frequency = 24000;
    [SerializeField] int m_recordingDuration = 10;
    public UnityEvent m_onRecordingStarted;
    public UnityEvent m_onRecordingStopped;
    bool m_isRecording = false;
    int m_currentMicIndex = 0;
    int m_samplesRecordedWhenStopped;
    [SerializeField] AudioClip m_recordedAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        m_recordButton.image.sprite = m_notRecordingIcon;
        m_recordButton.onClick.AddListener(OnRecordButtonClicked);
        m_toggleMicButton.onClick.AddListener(OnMicToggleButtonClicked);
        UpdateMicNameLabel();
    }

    void OnRecordButtonClicked()
    {
        if (m_isRecording)
        {
            StopMicrophoneRecording();
            OnRecordingStoppped();
        }
        else
        {
            OnRecordingStarted();
        }
    }

    void OnRecordingStarted()
    {
        m_isRecording = true;
        m_recordButton.image.sprite = m_isRecordingIcon;
        CreateRecording();
        m_onRecordingStarted.Invoke();
    }

    void OnRecordingStoppped()
    {
        m_isRecording = false;
        m_recordButton.image.sprite = m_notRecordingIcon;
        m_onRecordingStopped.Invoke();
    }

    void OnMicToggleButtonClicked()
    {
        ToggleMicIndex();
        UpdateMicNameLabel();
    }

    void ToggleMicIndex()
    {
        int numberOfMics = Microphone.devices.Length;
        if (m_currentMicIndex >= (numberOfMics - 1))
        {
            m_currentMicIndex = 0;
        }
        else
        {
            m_currentMicIndex++;
        }
    }

    void UpdateMicNameLabel()
    {
        m_buttonText.text = Microphone.devices[m_currentMicIndex];
    }

    void CreateRecording()
    {
        string currentMic = Microphone.devices[m_currentMicIndex];
        m_recordedAudioClip = Microphone.Start(currentMic, false, m_recordingDuration, m_frequency);
    }

    void StopMicrophoneRecording()
    {
        string currentMic = Microphone.devices[m_currentMicIndex];
        int samplesRecorded = Microphone.GetPosition(currentMic);
        m_samplesRecordedWhenStopped = samplesRecorded;
        Microphone.End(currentMic);

    }

    public int GetSamplesRecordedWhenStopped()
    {
        return m_samplesRecordedWhenStopped;
    }

    public AudioClip GetLastRecordedClip()
    {
        return m_recordedAudioClip;
    }


}

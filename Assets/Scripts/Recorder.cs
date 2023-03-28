using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Recorder : MonoBehaviour
{
    [SerializeField] Toggle m_recordButton;
    [SerializeField] Button m_toggleMicButton;
    [SerializeField] TMP_Text m_buttonText;
    [SerializeField] int m_frequency = 24000;
    [SerializeField] int m_recordingDuration = 10;
    [SerializeField] float m_amplification = 2f;
    public UnityEvent m_onRecordingStarted;
    public UnityEvent m_onRecordingStopped;
    int m_currentMicIndex = 0;
    int m_samplesRecordedWhenStopped;
    [SerializeField] AudioClip m_recordedAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        m_recordButton.onValueChanged.AddListener(OnRecordButtonClicked);
        m_toggleMicButton.onClick.AddListener(OnMicToggleButtonClicked);
        UpdateMicNameLabel();
    }

    void OnRecordButtonClicked(bool isOn)
    {
        if (isOn)
        {
            OnRecordingStarted();
        }
        else
        {
            StopMicrophoneRecording();
            OnRecordingStoppped();
        }
    }

    void OnRecordingStarted()
    {
        CreateRecording();
        m_onRecordingStarted.Invoke();
    }

    void OnRecordingStoppped()
    {
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
        AmplifyAudio(m_recordedAudioClip);
        m_samplesRecordedWhenStopped = samplesRecorded;
        Microphone.End(currentMic);

    }

    void AmplifyAudio(AudioClip audioClip)
    {
        float [] data = new float[audioClip.samples];
        audioClip.GetData(data, 0);
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = data[i] * m_amplification;
        }
        audioClip.SetData(data, 0);
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

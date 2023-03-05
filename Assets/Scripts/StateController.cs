using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateController : MonoBehaviour
{
    public enum State { NoAudioLoaded, AudioLoaded, PlayingModelAudio, Recording, PlayingRecordedAudio }
    [SerializeField] State m_currentState = State.NoAudioLoaded;
    [SerializeField] ModelAudioLoader m_modelAudioLoader;
    [SerializeField] Recorder m_recorder;
    [SerializeField] AudioPlayer m_audioPlayer;
    [SerializeField] Button m_loadNewModelAudioButton;
    [SerializeField] TMP_Text m_promptMessage;

    private void Start()
    {
        SetCurrentStateTo(State.NoAudioLoaded);
        m_loadNewModelAudioButton.onClick.AddListener(OnLoadNewModelAudioButtonClicked);
        m_modelAudioLoader.m_uploadComplete.AddListener(OnAudioFileLoaded);
        m_modelAudioLoader.m_startButtonClicked.AddListener(OnStartButtonClicked);
        m_recorder.m_onRecordingStarted.AddListener(OnRecordingStarted);
        m_recorder.m_onRecordingStopped.AddListener(OnRecordingStopped);
        m_audioPlayer.m_finishedPlayingRecordedAudio.AddListener(OnFinishedPlayingBackAudio);
    }

    void OnLoadNewModelAudioButtonClicked()
    {
        SetCurrentStateTo(State.AudioLoaded);
    }

    void OnAudioFileLoaded(AudioClip loadedAudioClip)
    {
        m_audioPlayer.SetModelAudioClip(loadedAudioClip);
        SetCurrentStateTo(State.AudioLoaded);
    }

    void OnStartButtonClicked()
    {
        string captionInput = m_modelAudioLoader.GetCaptionText();
        m_audioPlayer.SetCaption(captionInput);
        SetCurrentStateTo(State.PlayingModelAudio);
    }

    void OnRecordingStarted()
    {
        SetCurrentStateTo(State.Recording);
    }

    void OnRecordingStopped()
    {
        AudioClip recordedAudio = m_recorder.GetLastRecordedClip();
        if (recordedAudio != null)
        {
            m_audioPlayer.SetRecordedAudioClip(recordedAudio);
            int samplesRecorded = m_recorder.GetSamplesRecordedWhenStopped();
            m_audioPlayer.SetSamplesOfRecordedAudio(samplesRecorded);
            SetCurrentStateTo(State.PlayingRecordedAudio);
        }
        else
        {
            Debug.LogError("Failed to record audio.");
        }
        
    }

    void OnFinishedPlayingBackAudio()
    {
        SetCurrentStateTo(State.PlayingModelAudio);
    }

    public void SetCurrentStateTo(State state)
    {
        switch (state)
        {
            case State.NoAudioLoaded:
                m_modelAudioLoader.gameObject.SetActive(true);
                m_modelAudioLoader.DisableStartButton();
                m_audioPlayer.StopAudio();
                break;
            case State.AudioLoaded:
                m_modelAudioLoader.gameObject.SetActive(true);
                m_modelAudioLoader.EnableStartButton();
                m_audioPlayer.StopAudio();
                break;
            case State.PlayingModelAudio:
                m_audioPlayer.ResetProgressBars();
                m_modelAudioLoader.gameObject.SetActive(false);
                m_audioPlayer.PlayModelAudio();
                m_promptMessage.text = "Listen to the model audio";
                break;
            case State.Recording:
                m_modelAudioLoader.gameObject.SetActive(false);
                m_audioPlayer.StopAudio();
                m_promptMessage.text = "Recording your pronounciation";
                break;
            case State.PlayingRecordedAudio:
                m_modelAudioLoader.gameObject.SetActive(false);
                m_audioPlayer.PlayRecordedAudio();
                m_promptMessage.text = "Playing back your recorded audio";
                break;
            default:
                break;
        }
        m_currentState = state;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    AudioSource m_audioSource;
    AudioClip m_modelAudioClip;
    AudioClip m_recordedAudioClip;
    int m_samplesOfRecordedAudio;
    [SerializeField] TMP_Text m_caption;
    [SerializeField] Image m_modelAudioBarFill;
    [SerializeField] Image m_recordedAudioBarFill;
    public UnityEvent m_finishedPlayingRecordedAudio;


    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void SetModelAudioClip(AudioClip audioClip)
    {
        m_modelAudioClip = audioClip;
    }

    public void SetRecordedAudioClip(AudioClip audioClip)
    {
        m_audioSource.volume = 1.0f;
        m_recordedAudioClip = audioClip;
    }

    public void PlayModelAudio()
    {
        m_audioSource.volume = 0.6f;
        PlayAudioWithClip(m_modelAudioClip);
    }

    private void Update()
    {
        if (CurrentClipIsModelAudioClip())
        {
            float audioElapsed = CalculateRatioOfCurrentClipPlayed(m_modelAudioClip.samples);
            if (audioElapsed > 1.0f)
            {
                m_audioSource.clip = null;
            }
            else
            {
                m_modelAudioBarFill.fillAmount = audioElapsed;
            }
        }

        if (CurrentClipIsRecordedAudioClip())
        {
            float audioElapsed = CalculateRatioOfCurrentClipPlayed(m_samplesOfRecordedAudio);
            if (audioElapsed > 1.0f)
            {
                Debug.Log("Finished Playing recorded Audio");
                m_audioSource.clip = null;
                m_finishedPlayingRecordedAudio.Invoke();
            }
            else
            {
                m_recordedAudioBarFill.fillAmount = audioElapsed;
            }
        }
    }

    float CalculateRatioOfCurrentClipPlayed(int maxSamples)
    {
        int samplesPlayed = m_audioSource.timeSamples;
        float totalSamples = maxSamples;
        if (maxSamples == 0)
        {
            Debug.LogError("Recorded Audio have not been defined.");
        }
        float audioElapsed = (samplesPlayed / totalSamples);
        return audioElapsed;
    }

    bool CurrentClipIsModelAudioClip()
    {
        return CurrentClipIs(m_modelAudioClip);
    }

    bool CurrentClipIs(AudioClip referenceClip)
    {
        if (referenceClip != null && m_audioSource.clip != null)
        {
            int referenceClipId = referenceClip.GetInstanceID();
            int currentClipId = m_audioSource.clip.GetInstanceID();
            if (referenceClipId == currentClipId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    bool CurrentClipIsRecordedAudioClip()
    {
        if (m_recordedAudioClip != null && m_audioSource.clip != null)
        {
            int recordedClipId = m_recordedAudioClip.GetInstanceID();
            int currentClipId = m_audioSource.clip.GetInstanceID();
            if (recordedClipId == currentClipId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    void PlayAudioWithClip(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            m_audioSource.clip = audioClip;
            m_audioSource.Play();
        }
        else
        {
            Debug.LogError("No Model audio Clip has been set.");
        }
    }

    public void PlayRecordedAudio()
    {
        PlayAudioWithClip(m_recordedAudioClip);
    }

    public void StopAudio()
    {
        m_audioSource.Stop();
    }

    public void SetCaption(string caption)
    {
        m_caption.text = caption;
    }

    public void SetSamplesOfRecordedAudio(int samples)
    {
        m_samplesOfRecordedAudio = samples;
    }

    public void ResetProgressBars()
    {
        m_modelAudioBarFill.fillAmount = 0;
        m_recordedAudioBarFill.fillAmount = 0;

    }

}

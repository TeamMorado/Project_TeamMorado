using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoSingleton<SoundManager>
{
    public List<AudioData> m_listAudioData = new List<AudioData>();
    protected Dictionary<string, AudioData> map_AudioData = new Dictionary<string, AudioData>();
    public List<AudioSource> m_listAudioSource_Use = new List<AudioSource>();
    public List<AudioSource> m_listAudioSource_UnUse = new List<AudioSource>();
    public AudioSource m_pAudioSource_BGM;

    [Range(0f,1f)]
    public float f_AudioTotalScale = 1f;
    [Range(0f, 1f)]
    public float f_AudioVFXScale = 1f;

    private string prevVfxValueKey = "VfxValueKey";
    private string prevBgmValueKey = "BgmValueKey";
    public float prevVfxValue = 1f;
    public float prevBgmValue = 1f;

    protected override void Setup()
    {
        for (int i = 0; i < m_listAudioData.Count; i++)
        {
            AudioData audioData = m_listAudioData[i];
            map_AudioData.Add(audioData.sz_Key, audioData);
        }

        prevVfxValue = PlayerPrefs.GetFloat(prevVfxValueKey, 1f);
        prevBgmValue = PlayerPrefs.GetFloat(prevBgmValueKey, 1f);
        SetVolumeChangeVFX(prevVfxValue);
        SetVolumeChangeBGM(prevBgmValue);
    }

    public float GetAudioDataSize(AudioClip audioClip)
    {
        foreach (var audioData in map_AudioData)
        {
            if (audioData.Value.audioClip == audioClip)
                return audioData.Value.f_AudioVolume;
        }
        return 1f;
    }

    public void SetVolumeChangeVFX(float value)
    {
        lock(m_listAudioSource_Use)
        {
            for (int i = 0; i < m_listAudioSource_Use.Count; i++)
            {
                AudioSource nAudioSource = m_listAudioSource_Use[i];
                nAudioSource.volume = GetAudioDataSize(nAudioSource.clip) * value;
            }
        }
        lock(m_listAudioSource_UnUse)
        {
            for (int i = 0; i < m_listAudioSource_UnUse.Count; i++)
            {
                AudioSource nAudioSource = m_listAudioSource_UnUse[i];
                nAudioSource.volume = GetAudioDataSize(nAudioSource.clip) * value;
            }
        }
        prevVfxValue = value;
        PlayerPrefs.SetFloat(prevVfxValueKey, prevVfxValue);
    }

    public void SetVolumeChangeBGM(float value)
    {
        if (m_pAudioSource_BGM == null)
            return;
        m_pAudioSource_BGM.volume = GetAudioDataSize(m_pAudioSource_BGM.clip) * value;
        prevBgmValue = value;
        PlayerPrefs.SetFloat(prevBgmValueKey, prevBgmValue);
    }

    public void PlaySFX(string key)
    {
        if(!map_AudioData.ContainsKey(key))
        {
            return;
        }
        AudioData audioData = map_AudioData[key];

        AudioSource audioSource = GetAudioSource();
        audioSource.clip = audioData.audioClip;
        audioSource.volume = audioData.f_AudioVolume * prevVfxValue;
        audioSource.Play();
    }

    public void StopSFX(string key)
    {
        if (!map_AudioData.ContainsKey(key))
        {
            return;
        }
        AudioData audioData = map_AudioData[key];

        AudioSource audioSource = GetAudioSource();
        audioSource.clip = audioData.audioClip;
        audioSource.volume = audioData.f_AudioVolume;
        audioSource.Play();
    }

    public void PlayBGM(string key)
    {
        if (!map_AudioData.ContainsKey(key))
        {
            return;
        }
        AudioData audioData = map_AudioData[key];

        if(m_pAudioSource_BGM == null)
        {
            GameObject nAudioObject = new GameObject("BGM");
            nAudioObject.transform.parent = this.transform;
            m_pAudioSource_BGM = nAudioObject.AddComponent<AudioSource>();
        }
        m_pAudioSource_BGM.clip = audioData.audioClip;
        m_pAudioSource_BGM.volume = audioData.f_AudioVolume * prevBgmValue;
        m_pAudioSource_BGM.loop = true;
        m_pAudioSource_BGM.Play();
    }

    public void StopBGM(string key)
    {
        if (m_pAudioSource_BGM == null)
        {
            return;
        }
        m_pAudioSource_BGM.Stop();
        m_pAudioSource_BGM = null;
    }

    public AudioSource GetAudioSource()
    {
        lock(m_listAudioSource_UnUse)
        {
            AudioSource audioSource = null;
            int listCount = m_listAudioSource_UnUse.Count;
            if (listCount == 0)
            {
                GameObject nAudioObject = new GameObject("AudioSource");
                nAudioObject.transform.parent = this.gameObject.transform;
                audioSource = nAudioObject.AddComponent<AudioSource>();
            }
            else
            {
                audioSource = m_listAudioSource_UnUse[0];
                m_listAudioSource_UnUse.Remove(audioSource);
            }
            m_listAudioSource_Use.Add(audioSource);
            return audioSource;
        }
    }

    private void Update()
    {
        lock(m_listAudioSource_Use)
        {
            for (int i = 0; i < m_listAudioSource_Use.Count; i++)
            {
                AudioSource useAudioSource = m_listAudioSource_Use[i];
                if (useAudioSource.isPlaying == false)
                {
                    m_listAudioSource_UnUse.Add(useAudioSource);
                    m_listAudioSource_Use.Remove(useAudioSource);
                }
            }
        }
    }
}

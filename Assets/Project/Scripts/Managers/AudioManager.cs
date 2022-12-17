using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using UnityEngine.InputSystem;

public class AudioManager : SingletonPersistent<AudioManager>
{
    public AudioMixer audioMixer;

    public AudioSource[] musicSources = new AudioSource[2];
    public List<AudioClip> audioClips = new List<AudioClip>();

    public AudioMixerSnapshot noEffects;
    public AudioMixerSnapshot fullEffects;

    public int sourceCurrentIndex = 0;
    public int sourceNextIndex = 1;
    public int currentClip = 0;

    private bool canBePressed = false;

    public void Start()
    {
        musicSources = GetComponents<AudioSource>();
        musicSources[0].clip = audioClips[0];
    }

    public void SetMasterVolume(float volume)
    {
        musicSources = GetComponents<AudioSource>();
        // Convert the volume value from the range of -80 to 20 to the range of 0 to 1
        float convertedVolume = Mathf.Lerp(0f, 1f, (volume + 80f) / 100f);
        audioMixer.SetFloat("masterVolume", convertedVolume);
    }

    public void SetMusicVolume(float volume)
    {
        // Convert the volume value from the range of -80 to 20 to the range of 0 to 1
        float convertedVolume = Mathf.Lerp(0f, 1f, (volume + 80f) / 100f);
        audioMixer.SetFloat("musicVolume", convertedVolume);
    }

    public void SetSfxVolume(float volume)
    {
        // Convert the volume value from the range of -80 to 20 to the range of 0 to 1
        float convertedVolume = Mathf.Lerp(0f, 1f, (volume + 80f) / 100f);
        audioMixer.SetFloat("sfxVolume", convertedVolume);
    }

    public void SetReverb(float targetVolume, float duration)
    {
        StartCoroutine(FadeReverb(targetVolume, duration));
    }

    private IEnumerator FadeReverb(float targetVolume, float duration)
    {
        float currentTime = 0f;
        float currentVolume;

        audioMixer.GetFloat("reverbVolume", out currentVolume);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(currentVolume, targetVolume, currentTime / duration);
            audioMixer.SetFloat("reverbVolume", newVolume);
            yield return null;
        }
    }

    public void BlendSongs(float blendTime, float volume = 0.5f)
    {
        if(currentClip + 1 == audioClips.Count)
        {
            currentClip = -1;
        }
        musicSources[sourceNextIndex].clip = audioClips[++currentClip];

        // Fade out the volume of the first song over the blendTime
        musicSources[sourceCurrentIndex].DOFade(0, blendTime);

        // At the same time, fade in the volume of the second song over the blendTime
        musicSources[sourceNextIndex].DOFade(volume, blendTime);

        // After the blendTime, start playing the second song
        musicSources[sourceNextIndex].PlayDelayed(blendTime);

        sourceCurrentIndex = sourceNextIndex;
        sourceNextIndex = sourceCurrentIndex == 0 ? 1 : 0;
    }

    public void SetSnapshot(AudioMixerSnapshot snapshotToEnable, AudioMixerSnapshot snapshotToDisable)
    {
        snapshotToEnable.TransitionTo(0.1f);
        snapshotToDisable.TransitionTo(0.1f);
    }
}





using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip clip;
    public float volume = 0.8f;
}

public class SoundSpawner : SingletonPersistent<SoundSpawner>
{
    // A list of sound clips that can be edited in the Inspector
    public List<SoundClip> soundClips;

    // A dictionary that maps sound names to audio sources
    private Dictionary<string, AudioSource> soundSources;

    void Start()
    {
        // Initialize the audio sources for the sound clips
        soundSources = new Dictionary<string, AudioSource>();
        foreach(SoundClip soundClip in soundClips)
        {
            GameObject soundObject = new GameObject(soundClip.name);
            soundObject.transform.parent = transform;

            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.volume = soundClip.volume;
            audioSource.clip = soundClip.clip;
            soundSources.Add(soundClip.name, audioSource);
        }
    }

    public void SpawnSound(string soundName)
    {
        // Play the specified sound
        soundSources[soundName].Play();
    }

    public void StopSound(string soundName)
    {
        // Stop the specified sound
        soundSources[soundName].Stop();
    }
}
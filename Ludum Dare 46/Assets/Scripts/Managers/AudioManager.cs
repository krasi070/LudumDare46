using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    private Dictionary<string, int> _soundIndexByName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);

            return;
        }

        InitSounds();
    }

    public void Play(string name)
    {
        int index = GetSoundIndex(name);

        if (index >= 0)
        {
            sounds[index].source.Play();
        }
    }

    public void Stop(string name)
    {
        int index = GetSoundIndex(name);

        if (index >= 0)
        {
            if (!sounds[index].source.isPlaying)
            {
                Debug.LogWarning($"Request to stop audio clip {name} but it is currently not playing!");

                return;
            }

            sounds[index].source.Stop();
        }
    }

    public bool IsPlaying(string name)
    {
        int index = GetSoundIndex(name);

        if (index >= 0)
        {
            return sounds[index].source.isPlaying;
        }

        return false;
    }

    public void Pause(string name)
    {
        int index = GetSoundIndex(name);

        if (index >= 0)
        {
            if (!sounds[index].source.isPlaying)
            {
                Debug.LogWarning($"Request to pause audio clip {name} but it is currently not playing!");

                return;
            }

            sounds[index].source.Pause();
        }
    }

    public void UnPause(string name)
    {
        int index = GetSoundIndex(name);

        if (index >= 0)
        {
            if (sounds[index].source.isPlaying)
            {
                Debug.LogWarning($"Request to unpause audio clip {name} but it is currently playing!");

                return;
            }
        }

        sounds[index].source.UnPause();
    }

    public void SetVolume(string name, float newVolume)
    {
        int index = GetSoundIndex(name);

        if (index >= 0)
        {
            sounds[index].volume = newVolume;
            sounds[index].source.volume = sounds[index].volume;
        }
    }

    public void SetPitch(string name, float newPitch)
    {
        int index = GetSoundIndex(name);

        if (index >= 0)
        {
            sounds[index].pitch = newPitch;
            sounds[index].source.pitch = sounds[index].pitch;
        }
    }

    private void InitSounds()
    {
        _soundIndexByName = new Dictionary<string, int>();

        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source = gameObject.AddComponent<AudioSource>();
            sounds[i].source.clip = sounds[i].clip;
            sounds[i].source.volume = sounds[i].volume;
            sounds[i].source.pitch = sounds[i].pitch;
            sounds[i].source.loop = sounds[i].loop;

            _soundIndexByName.Add(sounds[i].name, i);
        }
    }

    private int GetSoundIndex(string name)
    {
        if (!_soundIndexByName.ContainsKey(name))
        {
            Debug.LogWarning($"Audio clip {name} not found!");

            return -1;
        }

        return _soundIndexByName[name];
    }
}

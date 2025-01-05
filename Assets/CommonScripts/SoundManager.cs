using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("SoundManager");
                _instance = obj.AddComponent<SoundManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private const int MaxAudioSources = 10;
    private Queue<AudioSource> audioQueue = new Queue<AudioSource>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        for (int i = 0; i < MaxAudioSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioQueue.Enqueue(source);
        }
    }

    public static void PlaySound(string audioPath)
    {
        Instance.PlaySoundInternal(audioPath);
    }

    private void PlaySoundInternal(string audioPath)
    {
        AudioClip clip = Resources.Load<AudioClip>(audioPath);
        Debug.Log(clip);

        if (clip == null)
        {
            Debug.LogWarning($"AudioClip not found at path: {audioPath}");
            return;
        }

        AudioSource source = audioQueue.Dequeue();
        source.clip = clip;
        source.Play();

        audioQueue.Enqueue(source);
    }
}

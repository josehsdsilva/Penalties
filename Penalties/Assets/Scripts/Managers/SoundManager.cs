using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;
    public List<Sound> sounds;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string name)
    {
        audioSource.clip = sounds.Find(q => q.name == name).clip;
        audioSource.Play();
    }
}

[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    public string name;
}

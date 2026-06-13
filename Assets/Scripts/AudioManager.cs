using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource UISource;

    [Header("Audio Clips")]
    public List<AudioClip> MusicList;
    public List<AudioClip> SFXList;
    public List<AudioClip> UIList;


    public void ChangeMusic(int ClipIndex)
    {
        MusicSource.clip = MusicList[ClipIndex];
        MusicSource.Play();
    }
    public void PlaySFX(int ClipIndex)
    {
        SFXSource.PlayOneShot(SFXList[ClipIndex]);
    }
    public void PlayUI(int ClipIndex)
    {
        UISource.PlayOneShot(UIList[ClipIndex]);
    }
}

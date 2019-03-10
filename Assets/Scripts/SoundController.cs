using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    [SerializeField] AudioClip bubbleMove;
    [SerializeField] AudioClip bubbleDestroy;
    private AudioSource bubblePlayer;
    private AudioSource musicPlayer;

    // Use this for initialization
    void Awake()
    {
        bubblePlayer = GameObject.Find("BubbleAudioSource").GetComponent<AudioSource>();
        musicPlayer = GameObject.Find("MusicAudioSource").GetComponent<AudioSource>();

        musicPlayer.enabled = FileController.GetSettings().MusicOn;
    }

    public void PlayMove()
    {
        bubblePlayer.PlayOneShot(bubbleMove);
    }

    public void PlayDestroy()
    {
        bubblePlayer.PlayOneShot(bubbleDestroy);
    }

    public void SetBubbleVolume(float volume)
    {
        bubblePlayer.volume = volume;
    }

    public void MuteSound(bool mute)
    {
        bubblePlayer.enabled = !mute;
    }

    public void MuteMusic(bool mute)
    {
        musicPlayer.enabled = !mute;
        if (!mute)
        {
            musicPlayer.Play();
        }
    }
}

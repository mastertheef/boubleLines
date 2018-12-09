using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    [SerializeField] AudioClip bubbleMove;
    [SerializeField] AudioClip bubbleDestroy;
    private AudioSource bubblePlayer;

    // Use this for initialization
    void Awake () {
        bubblePlayer = GameObject.Find("BubbleAudioSource").GetComponent<AudioSource>();
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
}

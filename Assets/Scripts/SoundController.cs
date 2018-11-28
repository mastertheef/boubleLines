using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    [SerializeField] AudioClip bubbleMove;
    [SerializeField] AudioClip bubbleDestroy;
    private AudioSource player;

    // Use this for initialization
    void Start () {
        player = GetComponent<AudioSource>();
	}
	
	public void PlayMove()
    {
        player.PlayOneShot(bubbleMove);
    }

    public void PlayDestroy()
    {
        player.PlayOneShot(bubbleDestroy);
    }
}

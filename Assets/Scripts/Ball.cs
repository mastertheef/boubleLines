using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public Animator anim;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
        anim.SetTrigger("appear");
    }

    public void Select()
    {
        anim.SetBool("isJumping", true);
    }

    public void Deselect()
    {
        GetComponentInParent<Animator>().SetBool("isJumping", false); // fix for step back after explode
    }

    public Color Color {
        get
        {
            return GetComponent<SpriteRenderer>().color;
        }

        set
        {
            GetComponent<SpriteRenderer>().color = value;
        }
    }
}

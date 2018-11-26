using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public Animator anim;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    public void Select()
    {
        anim.SetBool("isJumping", true);
    }

    public void Deselect()
    {
        anim.SetBool("isJumping", false);
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

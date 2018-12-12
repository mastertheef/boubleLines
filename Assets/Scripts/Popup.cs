using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour {

    [SerializeField] private Animator anim;

	public virtual void Show()
    {
        anim.SetTrigger("Show");
    }

    public virtual void Hide()
    {
        anim.SetTrigger("Hide");
    }
}

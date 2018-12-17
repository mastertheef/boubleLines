using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PopupBase : MonoBehaviour {

    private Animator anim;
    public delegate void PopupCloseHandler(PopupBase popup);
    public static event PopupCloseHandler OnPopupClose;
    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public virtual void Show()
    {
        GetComponent<Animator>().SetBool("Show", true);
    }

    public virtual void Hide()
    {
        GetComponent<Animator>().SetBool("Show", false);

    }

    public virtual void ClosePopup()
    {
        OnPopupClose(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {

    [SerializeField] private Image popupDarken;
    private Stack<Popup> popups;

    private void Awake()
    {
        popups = new Stack<Popup>();
    }

    public void Show(Popup popup)
    {
        if (popups.Any())
        {
            var current = popups.Peek();
            current.Hide();
        }
        popup.Show();
        popups.Push(popup);
        popupDarken.enabled = true;
    }

    public void Close()
    {
        if (popups.Any())
        {
            popups.Pop().Hide();
            var prev = popups.Peek();
            if (prev != null)
            {
                prev.Show();
            }
            else
            {
                popupDarken.enabled = false;
            }
        }
        else
        {
            popupDarken.enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

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

using System.Collections;
using UnityEngine;

public class Pin : MonoBehaviour {

    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
	}

    public void Hide()
    {
        StartCoroutine(hidePin());
    }

    private IEnumerator hidePin()
    {
        anim.SetTrigger("scale-out");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}

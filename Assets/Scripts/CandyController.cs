using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyController : MonoBehaviour {

    Renderer render;

    public GameObject textBonus;

	// Use this for initialization
	void Start () {
        render = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}    

    public void Destroy()
    {
        StartCoroutine("Fade");
        var lookPos = transform.position - Camera.main.transform.position ;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);     
        Instantiate<GameObject>(textBonus, transform.position, rotation);
    }

    IEnumerator Fade()
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = render.material.color;
            c.a = f;
            render.material.color = c;
            yield return new WaitForSeconds(.01f);
        }

        Destroy(gameObject);
    }
}

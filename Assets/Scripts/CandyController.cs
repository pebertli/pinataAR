using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CandyController : MonoBehaviour {

    Renderer render;
    TextMeshPro text;

    public GameObject textBonus;
    public int points;



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
        
        
        
        GameObject g = Instantiate<GameObject>(textBonus, transform.position, rotation);
        text = g.GetComponentInChildren<TextMeshPro>();
        text.SetText(((int) points).ToString());
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

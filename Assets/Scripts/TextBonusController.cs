using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBonusController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndAnimation()
    {
        Destroy(transform.parent.gameObject);
    }
}

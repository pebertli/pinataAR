using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColor : MonoBehaviour {

    public bool Infinite = false;

    ParticleSystem.MainModule _particle;
    float _maxTime;
    // Use this for initialization
    void Start () {
        _particle = gameObject.GetComponent<ParticleSystem>().main;
        _maxTime = _particle.duration;
    }
	
	// Update is called once per frame
	void LateUpdate () {

        if (!Infinite)
        {
            if (_maxTime > 0)
            {
                //randomize the single particle color
                _particle.startColor = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
                _maxTime -= Time.deltaTime;
            }
        }
        else
        {
            _particle.startColor = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);            
        }

    }
}

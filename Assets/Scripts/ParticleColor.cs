using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColor : MonoBehaviour {

    ParticleSystem.MainModule particle;
    float maxTime;
    // Use this for initialization
    void Start () {
        particle = gameObject.GetComponent<ParticleSystem>().main;
        maxTime = particle.duration;
    }
	
	// Update is called once per frame
	void LateUpdate () {

        if (maxTime > 0)
        {
            particle.startColor = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
            maxTime -= Time.deltaTime;
        }

            //new Color(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2), 1);

        // Change only the particles that are alive
        //for (int i = 0; i < ps.main.maxParticles; i++)
        //{
        //    particles[i].startColor = new Color(Random.Range(0, 2), Random.Range(0, 2)
        //        , Random.Range(0, 2), Random.Range(0, 2));
        //}
        //ps.SetParticles(particles, ps.main.maxParticles);
    }
}

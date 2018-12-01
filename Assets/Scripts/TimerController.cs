using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour {

    public GameObject timerText;
    public float timer = 30f;

    private TextMeshProUGUI text;

	// Use this for initialization
	void Start () {
        text = timerText.GetComponent<TextMeshProUGUI>();
        text.SetText(formatTime(timer));
        
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        text.SetText(formatTime(timer));
        if(timer <= 0f)
            GameController.Instance.State = GameController.GameState.GameOver;
    }

    string formatTime(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        string s = string.Format("{0}:{1}", t.Minutes, t.Seconds);

        return s;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour {

    public GameObject TimerText;
    //current timer
    public float Timer = 30f;
    [HideInInspector]
    public bool TimerOn = false;

    private TextMeshProUGUI _text;

	// Use this for initialization
	void Start () {
        _text = TimerText.GetComponent<TextMeshProUGUI>();
        _text.SetText(formatTime(Timer));
        
	}
	
	// Update is called once per frame
	void Update () {
        //still have time and timer is enabled
        if (Timer > 0 && TimerOn)
        {
            Timer -= Time.deltaTime;
            //update UI
            _text.SetText(formatTime(Timer));
        }
        else if(TimerOn && Timer <=0)
        {
            TimerOn = false;
            GameController.Instance.State = GameController.GameState.GameOver;
        }
            
    }

    string formatTime(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        string s = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

        return s;
    }
}

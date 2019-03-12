using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour {

    //public GameObject TimerText;
    //current timer
    public float Timer = 30f;
    [HideInInspector]
    public bool TimerOn = false;

   // private TextMeshProUGUI _text;

	// Use this for initialization
	void Start () {
        //_text = TimerText.GetComponent<TextMeshProUGUI>();
       GameController.Instance.UpdateTimer(Timer);
        
	}
	
	// Update is called once per frame
	void Update () {
        //still have time and timer is enabled
        if (Timer > 0 && TimerOn)
        {
            Timer -= Time.deltaTime;
            //update UI
            GameController.Instance.UpdateTimer(Timer);
        }
        else if(TimerOn && Timer <=0)
        {
            TimerOn = false;
            GameController.Instance.State = GameController.GameState.GameOver;
        }
            
    }

   
   
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadCandyController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    //private void OnEnable()
    //{
    //    EventManager.StartListening("Activate"+ this.transform.gameObject.name, Activate);
    //}

    public void Activate()
    {
        //Debug.Log("Name: " + this.transform.gameObject.name);
        SplatController splatController = GameController.Instance.gameObject.GetComponent<SplatController>();
        splatController.DoSplash(6);
    }
}

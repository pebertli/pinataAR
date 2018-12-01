using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour {

    public GameObject scoreText;
    public float Score = 0f;

    private TextMeshProUGUI text;

    // Use this for initialization
    void Start () {
        text = scoreText.GetComponent<TextMeshProUGUI>();
        text.SetText("Points: " + Score);
    }

    public void addScore(float score)
    {
        Score += score;
        text.SetText("Points: " + Score);
    }
	
}

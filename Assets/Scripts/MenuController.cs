using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public GameObject RestartButton;
    public GameObject ToolsMenu;

    private Animator restartAnimator;
    private Animator toolsAnimator;

    // Use this for initialization
    void Start () {
        toolsAnimator = ToolsMenu.GetComponent<Animator>();
        restartAnimator = RestartButton.GetComponent<Animator>();        
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("g"))
        {
            //GameController.Instance.State = GameController.GameState.Started;
        }
        else if (Input.GetKeyDown("h"))
        {
            //SelectTool(1);
        }
    }

    public void RestartClick()
    {
        GameController.Instance.RestartPinata();               
        
    }

    public void SelectTool(int tool)
    {        
        if (tool != (int)GameController.Instance.Tool)
        {
            GameController.Instance.Tool = (GameController.PlayerTool)tool;
            GameController.Instance.AnimatePlayerTool();
            if (tool == (int) GameController.PlayerTool.Bate)
            {
                toolsAnimator.SetTrigger("hit");
            }
            else if (tool == (int) GameController.PlayerTool.Hand)
            {
                toolsAnimator.SetTrigger("catch");
            }

        }
    }
}

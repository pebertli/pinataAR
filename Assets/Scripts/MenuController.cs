using UnityEngine;

public class MenuController : MonoBehaviour {

    public GameObject RestartButton;
    public GameObject ToolsMenu;
    public GameObject EntryMenu;
    public GameObject GameOverMenu;

    private Animator _restartAnimator;
    private Animator _toolsAnimator;

    // Use this for initialization
    void Start () {
        _toolsAnimator = ToolsMenu.GetComponent<Animator>();
        _restartAnimator = RestartButton.GetComponent<Animator>();        
    }
	
	// Update is called once per frame
	//void Update () {
	//	if(Input.GetKeyDown("g"))
 //       {
 //           //GameController.Instance.State = GameController.GameState.Started;
 //       }
 //       else if (Input.GetKeyDown("h"))
 //       {
 //           //SelectTool(1);
 //       }
 //   }

    public void RestartClick()
    {
        GameController.Instance.RestartPinata();                       
    }


    public void PlayClick()
    {
        GameController.Instance.StartPlay();

    }

    public void SelectTool(int tool)
    {
        //animate the UI
        if (tool != (int)GameController.Instance.Tool)
        {
            if (tool == (int)GameController.PlayerTool.Bate)
            {
                _toolsAnimator.SetTrigger("hit");
            }
            else if (tool == (int)GameController.PlayerTool.Hand)
            {
                _toolsAnimator.SetTrigger("catch");
            }

        }

        GameController.Instance.ChoosePlayerTool((GameController.PlayerTool)tool);

      
    }
}

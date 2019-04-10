using UnityEngine;

public class UIController : MonoBehaviour {

    public GameObject RestartButtonInstance;
    public GameObject ToolsUIInstance;
    public GameObject HUDInstance;
    public GameObject EntryMenuInstance;
    public GameObject GameOverMenuInstance;
    public GameObject TimerUIInstance;
    public GameObject ScoreUIInstance;
    public GameObject FinalScoreUIInstance;
    public GameObject SnackBarInstance;

    private Animator _restartAnimator;
    private Animator _toolsAnimator;

    // Use this for initialization
    void Start () {
        _toolsAnimator = ToolsUIInstance.GetComponent<Animator>();
        _restartAnimator = RestartButtonInstance.GetComponent<Animator>();        
    }

    public void RestartClick()
    {
        GameController.Instance.State = GameController.GameState.SearchingFloor;
    }

    public void MainMenuClick()
    {
        GameController.Instance.State = GameController.GameState.MainMenu;
    }

    public void PlayClick()
    {
        GameController.Instance.State = GameController.GameState.SearchingFloor;

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

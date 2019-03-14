using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public enum GameState
    {
        MainMenu = 0,
        SearchingFloor,
        Ready,
        Playing,
        Broken,
        GameOver
    }

    public enum PlayerTool
    {
        Bate = 0,
        Hand = 1
    }


    private static GameController instance;

    public static GameController Instance
    {
        get { return instance; }
    }

    private GameState _state;
    public GameState State
    {
        get
        {
            return _state;
        }

        set
        {
            Debug.Log(_state+" "+value);

            if (value == GameState.MainMenu)
            {
                _timerController.TimerOn = false;
                PlaneGeneratorInstance.SetActive(false);
                PointCloudInstance.SetActive(false);
                _menu.SnackBarInstance.SetActive(false);
                _menu.EntryMenuInstance.SetActive(true);
                _menu.GameOverMenuInstance.SetActive(false);
                _menu.RestartButtonInstance.SetActive(false);
                _menu.ToolsUIInstance.SetActive(false);
                _menu.SelectTool((int)GameController.PlayerTool.Hand);

                _state = GameState.MainMenu;
            }
            else if (value == GameState.SearchingFloor)
            {
                if (!_arController.HasPlaneDetected)
                {
                    if (_pinataController != null)
                        _pinataController.Restart();
                    _candyController.DestroyCandies();

                    _timerController.TimerOn = false;
                    PlaneGeneratorInstance.SetActive(true);
                    PointCloudInstance.SetActive(true);
                    _menu.SnackBarInstance.SetActive(true);
                    _menu.EntryMenuInstance.SetActive(false);
                    _menu.GameOverMenuInstance.SetActive(false);
                    _menu.RestartButtonInstance.SetActive(false);
                    _menu.ToolsUIInstance.SetActive(false);
                    _menu.SelectTool((int)GameController.PlayerTool.Hand);

                    _state = GameState.SearchingFloor;
                }
                else
                {
                    _state = GameState.SearchingFloor;
                    State = GameState.Ready;
                }
            }
            if (value == GameState.Ready)
            {
                _state = GameState.Ready;
                State = GameState.Playing;
            }
            else if (value == GameState.Playing)
            {

                _candyController.DestroyCandies();
                if (_pinataController != null)
                {
                    _pinataController.Restart();
                }
                else if (_pinataController == null)
                {
                    _menu.SnackBarInstance.SetActive(false);
                    //hide AR plane visual helpers
                    PlaneGeneratorInstance.SetActive(false);
                    PointCloudInstance.SetActive(false);
                    //make pinata look to camera
                    var lookPos = transform.position - CameraInstance.transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);

                    //instancing pinata and invisible floor
                    //how high is the pinata?
                    Vector3 yOffset = new Vector3(0, CameraInstance.transform.position.y + 2f, 0);
                    GameObject dummy = new GameObject();
                    dummy.name = "dummy";
                    Transform anchorTransform = dummy.transform;

                    if (_arController.ARAnchor != null)
                    {
                        anchorTransform = _arController.ARAnchor.transform;
                    }
                    GameObject pinata = Instantiate<GameObject>(PinataPrefab, anchorTransform.position + yOffset, rotation, anchorTransform.transform);
                    Instantiate<GameObject>(PlanePrefab, anchorTransform.position, Quaternion.identity, anchorTransform.transform);

                    _pinataController = pinata.GetComponentInChildren<PinataController>();
                }

                _timerController.TimerOn = true;
                _timerController.Timer = ConstantHelper.PLAY_TIME;
                PlaneGeneratorInstance.SetActive(false);
                PointCloudInstance.SetActive(false);
                _menu.SnackBarInstance.SetActive(false);
                _menu.EntryMenuInstance.SetActive(false);
                _menu.GameOverMenuInstance.SetActive(false);
                _menu.RestartButtonInstance.SetActive(true);
                _menu.ToolsUIInstance.SetActive(true);
                _menu.SelectTool((int)GameController.PlayerTool.Bate);

                _state = GameState.Playing;

            }
            else if (value == GameState.Broken)
            {
                _state = GameState.Broken;
            }
            else if (value == GameState.GameOver)
            {
                _menu.FinalScoreUIInstance.GetComponent<TMPro.TextMeshProUGUI>().SetText("Final Score: " + _score);

                _timerController.TimerOn = false;
                PlaneGeneratorInstance.SetActive(false);
                PointCloudInstance.SetActive(false);
                _menu.SnackBarInstance.SetActive(false);
                _menu.EntryMenuInstance.SetActive(false);
                _menu.GameOverMenuInstance.SetActive(true);
                _menu.RestartButtonInstance.SetActive(false);
                _menu.ToolsUIInstance.SetActive(false);
                _menu.SelectTool((int)GameController.PlayerTool.Hand);

                _state = GameState.GameOver;
            }

           
            

        }
    }

    public PlayerTool Tool;
    public float Score;
    public PlayerController PlayerController;
    public GameObject PinataPrefab;
    public GameObject PlanePrefab;
    public GameObject CameraInstance;
    public GameObject PlaneGeneratorInstance;
    public GameObject PointCloudInstance;

    private UIController _menu;
    private CandySpawnController _candyController;    
    private TimerController _timerController;
    private ARController _arController;
    private PinataController _pinataController;

    private float _score = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    // Use this for initialization
    void Start()
    {
        _menu = GetComponent<UIController>();
        _candyController = GetComponent<CandySpawnController>();
        _timerController = GetComponent<TimerController>();
        _arController = GetComponent<ARController>();

        Tool = PlayerTool.Hand;
        State = GameState.MainMenu;
        AddScore(0);
    }

    private void Update()
    {
        if (State == GameState.SearchingFloor && _arController.HasPlaneDetected)
            State = GameState.Ready;
        else if (State >= GameState.Ready && !_arController.HasPlaneDetected)
            State = GameState.SearchingFloor;

        if (Input.GetKeyDown("a"))
            State = GameState.Playing;
        if (Input.GetKeyDown("b"))
            State = GameState.Broken;
    }

    public void ChoosePlayerTool(PlayerTool tool)
    {
        //animate only if it needs
        if (tool != Tool)
        {
            Tool = tool;
            switch (Tool)
            {
                case PlayerTool.Hand:
                    PlayerController.GetComponentInChildren<Animator>().SetTrigger("hideBate");
                    break;
                case PlayerTool.Bate:
                    PlayerController.GetComponentInChildren<Animator>().SetTrigger("showBate");
                    break;

            }
        }

    }

    public void SpawnCandy(int amount, Vector3 position, float spread)
    {
        _candyController.SpawnCandy(amount, position, spread);        
    }

    public void AddScore(float score)
    {
        _score += score;
        _menu.ScoreUIInstance.GetComponent<TMPro.TextMeshProUGUI>().SetText("Points: "+_score);
    }

    public void UpdateTimer(float time)
    {
        _menu.TimerUIInstance.GetComponent<TMPro.TextMeshProUGUI>().SetText(Util.FormatTimer(time));   
    }
}

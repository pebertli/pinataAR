using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public enum GameState
    {
        SearchingFloor = 0,
        EntryMenu,
        Started,
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
            _state = value;

            if (_state == GameState.SearchingFloor)
            {
                if (PlaneGenerator != null)
                {
                    PlaneGenerator.SetActive(true);
                    PointCloud.SetActive(true);
                }
                _menu.EntryMenu.SetActive(false);
                _menu.RestartButton.SetActive(false);
                _menu.ToolsMenu.SetActive(false);
                _menu.SelectTool((int)GameController.PlayerTool.Hand);
                _timerController.TimerOn = false;
            }
            else if (_state == GameState.Started)
            {
                if (PlaneGenerator != null)
                {
                    PlaneGenerator.SetActive(false);
                    PointCloud.SetActive(false);
                }
                _menu.RestartButton.SetActive(true);
                _menu.ToolsMenu.SetActive(true);
                _menu.SelectTool((int)GameController.PlayerTool.Bate);
                //start timer
                _timerController.TimerOn = true;

            }
            else if (_state == GameState.Broken)
            {
                //stop timer
                //_timerController.TimerOn = false;
                //if (PlaneGenerator != null)
                //{
                //    PlaneGenerator.SetActive(true);
                //    PointCloud.SetActive(true);
                //}
                //menu.RestartButton.SetActive(false);
                //menu.ToolsMenu.SetActive(false);
                //menu.SelectTool((int)GameController.PlayerTool.Hand);
            }
            else if (_state == GameState.GameOver)
            {
                //stop timer
                _timerController.TimerOn = false;
                //if (PlaneGenerator != null)
                //{
                //    PlaneGenerator.SetActive(true);
                //    PointCloud.SetActive(true);
                //}
                _menu.RestartButton.SetActive(false);
                _menu.ToolsMenu.SetActive(false);
                _menu.GameOverMenu.SetActive(true);
                _menu.SelectTool((int)GameController.PlayerTool.Hand);
            }

        }
    }

    public PlayerTool Tool;
    public float Score;
    public PlayerController Player;
    public GameObject PlaneGenerator;
    public GameObject PointCloud;

    private MenuController _menu;
    private CandySpawnController _candyController;
    private ScoreController _scoreController;
    private TimerController _timerController;

    private PinataController _pinata;
    public PinataController Pinata
    {
        get
        {
            return _pinata;
        }

        set
        {
            _pinata = value;
        }
    }

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
        _menu = GetComponent<MenuController>();
        _candyController = GetComponent<CandySpawnController>();
        _scoreController = GetComponent<ScoreController>();
        _timerController = GetComponent<TimerController>();

        Tool = PlayerTool.Hand;
        State = GameState.EntryMenu;
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
                    Player.GetComponentInChildren<Animator>().SetTrigger("hideBate");
                    break;
                case PlayerTool.Bate:
                    Player.GetComponentInChildren<Animator>().SetTrigger("showBate");
                    break;

            }
        }

    }

    public void SpawnCandy(int amount, Vector3 position, float spread)
    {
        _candyController.SpawnCandy(amount, position, spread);
    }

    public void RestartPinata()
    {
        if (State != GameState.SearchingFloor)
        {
            _pinata.Restart();
            _candyController.DestroyCandies();
        }
    }

    public void StartPlay()
    {
        State = GameState.SearchingFloor;
        //menu.EntryMenu.GetComponent<Animator>().SetBool("Fade", false);
        //menu.EntryMenu.GetComponent<Animator>().Play("fade_menu");
    }


    public void addScore(float score)
    {
        _scoreController.addScore(score);
    }
}

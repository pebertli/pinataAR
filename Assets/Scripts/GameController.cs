using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public enum GameState
    {
        SearchingFloor = 0,
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

    public GameState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;

            if (state == GameState.SearchingFloor)
            {
                if (PlaneGenerator != null)
                {
                    PlaneGenerator.SetActive(true);
                    PointCloud.SetActive(true);
                }
                menu.RestartButton.SetActive(false);
                menu.ToolsMenu.SetActive(false);
                menu.SelectTool((int)GameController.PlayerTool.Hand);
            }
            else if (state == GameState.Started)
            {
                if (PlaneGenerator != null)
                {
                    PlaneGenerator.SetActive(false);
                    PointCloud.SetActive(false);
                }
                menu.RestartButton.SetActive(true);
                menu.ToolsMenu.SetActive(true);
                menu.SelectTool((int)GameController.PlayerTool.Bate);
            }
            else if (state == GameState.Broken)
            {
                //if (PlaneGenerator != null)
                //{
                //    PlaneGenerator.SetActive(true);
                //    PointCloud.SetActive(true);
                //}
                //menu.RestartButton.SetActive(false);
                //menu.ToolsMenu.SetActive(false);
                //menu.SelectTool((int)GameController.PlayerTool.Hand);
            }

        }
    }

    private GameState state;
    public PlayerTool Tool;
    public float Score;

    public PlayerController player;
    public PinataController pinata;
    public GameObject PlaneGenerator;
    public GameObject PointCloud;


    private MenuController menu;
    private CandySpawnController CandyController;
    private ScoreController ScoreController;

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
        menu = GetComponent<MenuController>();
        CandyController = GetComponent<CandySpawnController>();
        ScoreController = GetComponent<ScoreController>();

        Tool = PlayerTool.Hand;
        State = GameState.SearchingFloor;
    }

    public void AnimatePlayerTool()
    {
        switch (Tool)
        {
            case PlayerTool.Hand:
                player.GetComponentInChildren<Animator>().SetTrigger("hideBate");
                break;
            case PlayerTool.Bate:
                player.GetComponentInChildren<Animator>().SetTrigger("showBate");
                break;

        }

    }

    public void SpawnCandy(int amount, Vector3 position, float spread)
    {
        CandyController.SpawnCandy(amount, position, spread);
    }

    public void RestartPinata()
    {
        if (State != GameState.SearchingFloor)
        {
            pinata.Restart();
            CandyController.DestroyCandies();
        }
    }

    public void addScore(float score)
    {
        ScoreController.addScore(score);
    }
}

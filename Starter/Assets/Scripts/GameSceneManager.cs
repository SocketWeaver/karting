using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public int lapsToWin = 3;
    public int countdown = 5;

    public GUIManager guiManager;

    public enum GameState { waiting, starting, started, finished };
    public GameState State { get => _state; private set => _state = value; }
    private GameState _state;

    int countdown_;
    int lap_;

    private void Start()
    {
        guiManager.SetLapRecord(lap_, lapsToWin);
    }

    void OnTriggerEnter(Collider other)
    {
        lap_ = lap_ + 1;
        guiManager.SetLapRecord(lap_, lapsToWin);

        if (lap_ == lapsToWin)
        {
            Debug.Log("Winner!!");
            guiManager.SetMainText("1st");
        }
    }

    void Update()
    {
        if (State == GameState.waiting)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                // start the countdown
                Debug.Log("Starting...");
                State = GameState.starting;
                InvokeRepeating("Countdown", 0.0f, 1.0f);
                countdown_ = countdown;
            }
        }
    }

    void Countdown()
    {
        if (State == GameState.starting)
        {
            Debug.Log(countdown_);
            if (countdown_ == 0)
            {
                // countdown is 0, start the game
                guiManager.SetMainText("Go");
                State = GameState.started;
                Debug.Log("Started");
            }
            else
            {
                guiManager.SetMainText(countdown_.ToString());
                countdown_ = countdown_ - 1;
            }
        }
        else
        {
            // clear main text and stop timer
            guiManager.SetMainText("");
            CancelInvoke("Countdown");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using SWNetwork;
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

    GameDataManager gameDataManager;
    const string PLAYER_PRESSED_ENTER = "PlayersPressedEnter";
    const string WIINER_ID = "WinnerId";

    private void Start()
    {
        guiManager.SetLapRecord(lap_, lapsToWin);

        // get a reference of the gameDataManager component
        gameDataManager = GetComponent<GameDataManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        NetworkID networkID = go.GetComponent<NetworkID>();

        // check if the player owns the GameObject, we don't care when the other player crosses the finish line. 
        // Players update their own lap counts.
        if (networkID.IsMine)
        {
            lap_ = lap_ + 1;
            guiManager.SetLapRecord(lap_, lapsToWin);

            if (lap_ == lapsToWin)
            {
                Debug.Log("Winner!!");
                string winnerId = gameDataManager.GetPropertyWithName(WIINER_ID).GetStringValue();
                Debug.Log("OnTriggerEnter winnerID " + winnerId);

                // if winnerId is empty, local player is the winner
                // If the winner did not leave the game and finished the laps again, the player should still be the winner
                if (string.IsNullOrEmpty(winnerId) || winnerId.Equals(NetworkClient.Instance.PlayerId))
                {
                    gameDataManager.Modify(WIINER_ID, NetworkClient.Instance.PlayerId);
                    guiManager.SetMainText("1st");
                }
                else
                {
                    guiManager.SetMainText("2nd");
                }
            }
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

                // Modify the PlayersPressedEnter sync property.
                int playerPressedEnter = gameDataManager.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();
                gameDataManager.Modify(PLAYER_PRESSED_ENTER, playerPressedEnter + 1);
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


    // SceneSpanwer events
    public void OnSpawnerReady(bool finishedSceneSetup)
    {
        // scene has not been set up. spawn a car for the local player.
        if (!finishedSceneSetup)
        {
            /* 
                assign different spawn points for the players in the room
                This is okay for this tutorial as we only have 2 players in a room and we are not handling host migration.
                To properly assign spawn points, you should use GameDataManger or custom room data.
            */
            if (NetworkClient.Instance.IsHost)
            {
                NetworkClient.Instance.LastSpawner.SpawnForPlayer(0, 1);
            }
            else
            {
                NetworkClient.Instance.LastSpawner.SpawnForPlayer(0, 0);
            }

            // tells the SceneSpawner the local player has finished scene setup.
            NetworkClient.Instance.LastSpawner.PlayerFinishedSceneSetup();
        }
    }

    // PlayersPressedEnter events
    public void OnPlayersPressedEnterValueChanged()
    {
        int playerPressedEnter = gameDataManager.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();

        // check if all players have pressed Enter
        if(playerPressedEnter == 2)
        {
            // start the countdown
            InvokeRepeating("Countdown", 0.0f, 1.0f);
            countdown_ = countdown;
        }
    }

    public void OnPlayersPressedEnterValueReady()
    {
        int playerPressedEnter = gameDataManager.GetPropertyWithName(PLAYER_PRESSED_ENTER).GetIntValue();

        // check if all players have pressed Enter
        if (playerPressedEnter == 2)
        {
            // the player probably got disconnected from the room
            // If all players has pressed the Enter key, the game has started already.
            State = GameState.started;
            Debug.Log("Started");
        }
    }

    public void OnPlayersPressedEnterValueConflict(SWSyncConflict conflict, SWSyncedProperty property)
    {
        // If players pressed the Key at the same time, we might get conflict
        // The game server will receive two requests to change the PlayersPressEnter value from 0 to 1
        // The game server will accept the first request and change PlayersPressEnter value to 1
        // The second request will fail and player who sent the second request will get a confict
        int remotePlayerPressed = (int)conflict.remoteValue;

        // Add 1 to the remote PlayerPressedEnter value to resolve the conflict.
        int resolvedPlayerPressed = remotePlayerPressed + 1;

        property.Resolve(resolvedPlayerPressed);
    }

    // WinnerId events
    public void OnWinnerIdValueChanged()
    {
        string winnerId = gameDataManager.GetPropertyWithName(WIINER_ID).GetStringValue();
        Debug.Log("OnWinnerIdValueChanged winnerID " + winnerId);
    }

    public void OnWinnerIdValueReady()
    {
        string winnerId = gameDataManager.GetPropertyWithName(WIINER_ID).GetStringValue();
        Debug.Log("OnWinnerIdValueReady winnerID " + winnerId);
    }
}

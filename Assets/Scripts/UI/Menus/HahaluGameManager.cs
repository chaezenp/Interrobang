using System;
using UnityEngine;

public class HahaluGameManager : MonoBehaviour
{
    public static HahaluGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;


    private enum State
    {
        WaitingToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float waitingToStartTimer = 1f;

    private bool isGamePaused = false;
    private bool isGameOver = false;
    private bool PlayerCaught = false;

    private void Awake()
    {
        Instance = this;

        state = State.WaitingToStart;
    }
    
    private void Start()
    {
        PlayerInputController.Instance.OnPauseAction += Input_OnPauseAction;
    }


    private void Input_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseMenu();
    }

    public void IsPlayerCaught()
    {
        PlayerCaught = true;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, new EventArgs());
                }
                break;
            case State.GamePlaying:
                if (PlayerCaught)
                {
                state = State.GameOver;
                OnStateChanged?.Invoke(this, new EventArgs());
                }
                break;
            case State.GameOver:
                isGameOver = true;
                break;

        }
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }
    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public void TogglePauseMenu()
    {
        if (isGameOver) return;
        Debug.Log(isGamePaused);
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public GameController gameController;

    public Text timerText;
    public Text movesText;

    public GameObject winPanel;

    private float gameTime = 0;
    private bool showTime = false;

    void Awake()
    {
        gameController.OnMovesChanged += ShowMoves;
        gameController.OnGameFinished += StopCount;
        gameController.OnGameStart += StartCount;
    }

    private void StartCount()
    {
        showTime = true;
        winPanel.SetActive(false);
        gameTime = 0;
    }

    private void StopCount()
    {
        showTime = false;
        winPanel.SetActive(true);
    }

    void Update()
    {
        if (showTime)
        {
            gameTime += Time.deltaTime;
            DisplayTime(((int)(gameTime / 60)).ToString(), ((int)(gameTime % 60)).ToString());
        }
    }

    public void Shuffle()
    {
        gameController.StartCoroutine(gameController.Shuffle());
    }

    private void ShowMoves(int moves)
    {
        movesText.text = "Moves: " + moves;
    }

    private void DisplayTime(string minutes, string seconds)
    {
        timerText.text = "Timer: " + (minutes.Length == 1 ? "0" + minutes : minutes) + ":" + (seconds.Length == 1 ? "0" + seconds : seconds);
    }
}
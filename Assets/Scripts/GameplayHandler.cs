using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHandler : MonoBehaviour
{
    [SerializeField] int timeForLevel = 90;
    [SerializeField] private HackButton[] hackButtons;
    [SerializeField] private Image[] connectionLines;
    [SerializeField] private GameObject gamePanel, winPanel, losePanel;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Text timer;    
    [SerializeField] private bool gamePaused = true;
    private int[] linesCurrentPosY;
    private int[] linesLevelPosY;
    private float timeLeft;
    enum movingDir { back = -1, forward = 1}; //to change positions - back for generating a level, forward for solving it

    void Start()
    {
        linesCurrentPosY = new int[connectionLines.Length];
        linesLevelPosY = new int[connectionLines.Length];
    }

    void Update()
    {
        if (!IsPaused())
        {
            timeLeft -= Time.deltaTime;
            timer.text = ((int)timeLeft / 60).ToString("00") + ":" + ((int)timeLeft % 60).ToString("00"); //converting time to XX:XX format
            LoseCheck(); 
        }
    }
    public void OnPlayButtonPress()
    {
        Debug.Log($"Number of buttons needed = {(int)difficultySlider.value}");
        GenerateLevel((int)difficultySlider.value);
        timeLeft = timeForLevel;
        UnpauseGame();
    }

    public void OnHackButtonPress(HackButton button)
    {
        moveLines(movingDir.forward, button);
        if (LinesOutOfBoundsCkeck(-5, 5)) moveLines(movingDir.back, button); //step back if lines are outside of the field
        WinCheck();
    }

    public void OnRestartButtonPless()
    {
        placeLinesInPositions(linesLevelPosY);
        linesLevelPosY.CopyTo(linesCurrentPosY, 0);
        if (IsPaused()) //when done after losing
        {
            timeLeft = timeForLevel;
            UnpauseGame();
        }
    }
    private void WinCheck() //player wins when all lines are on position 0
    {
        bool win = true;
        foreach (int y in linesCurrentPosY)
        {
            if (y != 0) win = false;
        }
        if (win)
        {
            gamePanel.SetActive(false);
            winPanel.SetActive(true);
        }
    }

    private void LoseCheck() //player loses when time runs out
    {
        if (timeLeft <= 0)
        {
            PauseGame();
            gamePanel.SetActive(false);
            losePanel.SetActive(true);
        }
    }

    private void GenerateLevel(int difficulty)
    {
        int numberOfMoves = 0;
        for (int i= 0; i < connectionLines.Length; i++) //reset positions
        {
            linesCurrentPosY[i] = 0;
            linesLevelPosY[i] = 0;
        }
        while (numberOfMoves < difficulty) //difficulty = number of moves needed to solve the level
        {
            int buttonNumber = Random.Range(0, connectionLines.Length); //pick a button
            moveLines(movingDir.back, hackButtons[buttonNumber]); //apply button's moves
            if (LinesOutOfBoundsCkeck(-5, 5)) //step back if lines are outside of the field
            {
                moveLines(movingDir.forward, hackButtons[buttonNumber]);
            } 
            else
            {
                numberOfMoves++;
                Debug.Log(buttonNumber+1);
            }
        }
        linesCurrentPosY.CopyTo(linesLevelPosY, 0);  //initial positions for the generated level in case of a level restart
    }

    private void moveLines(movingDir direction 
                    , HackButton button) //applying button's moves to the level
    {
        for (int i = 0; i < linesCurrentPosY.Length; i++)
        {
            linesCurrentPosY[i] += (int)direction * (int)button.moves[i];
        }
        placeLinesInPositions(linesCurrentPosY);
    }

    private bool LinesOutOfBoundsCkeck(int leftBound, int rightBound)
    {
        bool outOfBounds = false;
        foreach (int pos in linesCurrentPosY)
        {
            if ((pos < leftBound) || (pos > rightBound)) outOfBounds = true;
        }
        return outOfBounds;
    }

    private void placeLinesInPositions(int[] pos) //plase sprites in positions
    {
        for (int i = 0; i < connectionLines.Length; i++)
        {
            connectionLines[i].transform.localPosition = new Vector3(connectionLines[i].transform.localPosition.x, pos[i] * 30, 0);
        }
    }
    public void PauseGame()
    {
        gamePaused = true;
    }
    public void UnpauseGame()
    {
        gamePaused = false;
    }

    public bool IsPaused() => gamePaused ? true : false;

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

 }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackButton : MonoBehaviour
{
    [HideInInspector] public enum directions { down = -1, straight = 0, up = 1};
    public directions[] moves;
    [SerializeField] private Sprite arrowUp, arrowDown, straightLine;
    [SerializeField] private Image[] arrows;

    private void Start()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            switch (moves[i])
            {
                case directions.down:
                    arrows[i].sprite = arrowDown;
                    break;
                case directions.up:
                    arrows[i].sprite = arrowUp;
                    break;
                case directions.straight:
                    arrows[i].sprite = straightLine;
                    break;
            }

        }
    }


}

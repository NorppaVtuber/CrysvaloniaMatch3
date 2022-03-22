using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Moves : MonoBehaviour
{
    public static int remainingMoves;
    public static int gameDifficulty;
    public Text movesText;

    private void Start()
    {
        switch(gameDifficulty)
        {
            case 1: //easy
                remainingMoves = 25;
                break;
            case 2: //normal
                remainingMoves = 20;
                break;
            case 3: //hard
                remainingMoves = 15;
                break;
            case 4: //very hard
                remainingMoves = 10;
                break;
            default:
                remainingMoves = 20;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(remainingMoves >= 10)
        {
            movesText.text = remainingMoves.ToString();
        }
        else
        {
            movesText.text = " " + remainingMoves.ToString();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    WAIT,
    MOVE,
    OVER
}

public class Board : MonoBehaviour
{
    private FindMatches matches;
    public GameState currentState = GameState.MOVE;

    public int height;
    public int width;
    public int offSet;

    public float decreaseRowWaitTime;
    public float spawnPieceWaitTime;

    public GameObject tilePrefab;
    public GameObject[] objects;
    public GameObject[] destroyEffect;

    public Slider scoreSlider;

    private BackgroundTile[,] tiles;
    public GameObject[,] allObjects;

    public GamePiece currentPiece;

    // Start is called before the first frame update
    void Start()
    {
        matches = FindObjectOfType<FindMatches>();
        scoreSlider.value = 0;
        tiles = new BackgroundTile[width, height];
        allObjects = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++) // i = columns, j = rows
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + ")";
                int objectToUse = Random.Range(0, objects.Length);
                int maxIterations = 0;
                while(MatchesAt(i, j, objects[objectToUse]) && maxIterations < 100)
                {
                    objectToUse = Random.Range(0, objects.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                Vector3 tilePos = new Vector3(tempPos.x, tempPos.y, -0.03f);
                GameObject _object = Instantiate(objects[objectToUse], tilePos, Quaternion.identity);
                _object.GetComponent<GamePiece>().row = j;
                _object.GetComponent<GamePiece>().column = i;
                _object.transform.parent = this.transform;
                allObjects[i, j] = _object;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allObjects[column -1, row].tag == piece.tag && allObjects[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allObjects[column, row - 1].tag == piece.tag && allObjects[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allObjects[column, row -1].tag == piece.tag && allObjects[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allObjects[column - 1, row].tag == piece.tag && allObjects[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        GamePiece firstPiece = matches.currentMatches[0].GetComponent<GamePiece>();

        if (firstPiece != null)
        {
            foreach (GameObject gamePiece in matches.currentMatches)
            {
                GamePiece piece = gamePiece.GetComponent<GamePiece>();
                if(piece.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if(piece.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    private void CheckToMakePowerUps()
    {
        if(matches.currentMatches.Count == 4 || matches.currentMatches.Count == 7)
        {
            matches.CheckFlames();
        }
        if(matches.currentMatches.Count == 5 || matches.currentMatches.Count == 8)
        {
            if(ColumnOrRow())
            {
                if(currentPiece != null)
                {
                    if (currentPiece.isMatched)
                    {
                        if (!currentPiece.isLightning)
                        {
                            currentPiece.isMatched = false;
                            currentPiece.MakeLightningBottle();
                        }
                    }
                    else
                    {
                        if(currentPiece.otherObject != null)
                        {
                            GamePiece otherPiece = currentPiece.otherObject.GetComponent<GamePiece>();
                            if (otherPiece.isMatched)
                            {
                                if (!otherPiece.isLightning)
                                {
                                    otherPiece.isMatched = false;
                                    otherPiece.MakeLightningBottle();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentPiece != null)
                {
                    if (currentPiece.isMatched)
                    {
                        if (!currentPiece.isBomb)
                        {
                            currentPiece.isMatched = false;
                            currentPiece.MakeBomb();
                        }
                    }
                    else
                    {
                        if (currentPiece.otherObject != null)
                        {
                            GamePiece otherPiece = currentPiece.otherObject.GetComponent<GamePiece>();
                            if (otherPiece.isMatched)
                            {
                                if (!otherPiece.isBomb)
                                {
                                    otherPiece.isMatched = false;
                                    otherPiece.MakeBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allObjects[column, row].GetComponent<GamePiece>().isMatched)
        {
            if(matches.currentMatches.Count >= 4)
            {
                CheckToMakePowerUps();
            }
            scoreSlider.value += allObjects[column, row].GetComponent<GamePiece>().score;
            int i;
            switch (allObjects[column, row].tag)
            {
                case "Dog treat":
                    i = 0;
                    break;
                case "Lightning bottle":
                    i = 1;
                    break;
                case "Carrot":
                    i = 2;
                    break;
                case "Cheese":
                    i = 3;
                    break;
                case "Clover":
                    i = 4;
                    break;
                case "Feather":
                    i = 5;
                    break;
                case "Flames":
                    i = 6;
                    break;
                case "Lightning":
                    i = 7;
                    break;
                case "Mouse":
                    i = 8;
                    break;
                case "Pumpkin":
                    i = 9;
                    break;
                case "Starfish":
                    i = 10;
                    break;
                default:
                    i = 0;
                    break;
            }
            GameObject particle = Instantiate(destroyEffect[i], allObjects[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allObjects[column, row]);
            allObjects[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allObjects[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        matches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allObjects[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allObjects[i, j].GetComponent<GamePiece>().row -= nullCount;
                    allObjects[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(decreaseRowWaitTime);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allObjects[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    int objectToUse = Random.Range(0, objects.Length);
                    GameObject piece = Instantiate(objects[objectToUse], tempPos, Quaternion.identity);
                    piece.transform.parent = this.transform;
                    allObjects[i, j] = piece;
                    piece.GetComponent<GamePiece>().row = j;
                    piece.GetComponent<GamePiece>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allObjects[i, j] != null)
                {
                    if (allObjects[i, j].GetComponent<GamePiece>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {

        RefillBoard();
        yield return new WaitForSeconds(spawnPieceWaitTime);


        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(spawnPieceWaitTime);
            DestroyMatches();
        }
        matches.currentMatches.Clear();
        currentPiece = null;
        if(currentState != GameState.OVER)
        {
            yield return new WaitForSeconds(.5f);
            currentState = GameState.MOVE;
        }
        if(currentState == GameState.OVER)
        {
            Debug.Log(scoreSlider.value);
        }
    }
}

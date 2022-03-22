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

    public Slider scoreSlider;

    private BackgroundTile[,] tiles;
    public GameObject[,] allObjects;

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

    private void DestroyMatchesAt(int column, int row)
    {
        if (allObjects[column, row].GetComponent<GamePiece>().isMatched)
        {
            scoreSlider.value += allObjects[column, row].GetComponent<GamePiece>().score;
            matches.currentMatches.Remove(allObjects[column, row]);
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
                if(allObjects[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
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
        if(currentState != GameState.OVER)
        {
            yield return new WaitForSeconds(.5f);
            currentState = GameState.MOVE;
        }
    }
}

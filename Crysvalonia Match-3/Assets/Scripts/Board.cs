using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will handle creating the board and moving the pieces on the board
/// </summary>
public class Board : MonoBehaviour
{
    public static Board Instance;

    FindMatches matches;
    GameState currentState = GameState.MOVE;

    [Header("Board setup")]
    [SerializeField] int height;
    [SerializeField] int width;
    [SerializeField] int offSet;

    [SerializeField] float decreaseRowWaitTime;
    [SerializeField] float spawnPieceWaitTime;

    [SerializeField] GameObject tilePrefab; //is this even needed anymore?
    [SerializeField] GamePiece[] objects; //why u empty
    //public GameObject[] destroyEffect; //the objects will tell their own destroy effect, no need to keep comparing string tags

    [SerializeField] Slider scoreSlider; //TODO: Move this to a score script of some kind

    public int GetBoardHeight() { return height; }
    public int GetBoardWidth() { return width; }


    BackgroundTile[,] tiles;

    GamePiece currentPiece;
    public GamePiece GetCurrentPiece() { return currentPiece; }

    GameObject[,] allObjects;
    public GameObject[,] GetAllObjects() { return allObjects; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        matches = FindMatches.Instance;
        scoreSlider.value = 0;
        tiles = new BackgroundTile[width, height];
        allObjects = new GameObject[width, height];
        setUp();
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allObjects[i, j] != null)
                {
                    destroyMatchesAt(i, j);
                }
            }
        }
        matches.ClearMatchList();
        StartCoroutine(decreaseRowCo());
    }

    void setUp()
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
                while(matchesAt(i, j, objects[objectToUse]) && maxIterations < 100)
                {
                    objectToUse = Random.Range(0, objects.Length);
                    maxIterations++;
                }

                Vector3 tilePos = new Vector3(tempPos.x, tempPos.y, -0.03f);
                GameObject _object = Instantiate(objects[objectToUse].gameObject, tilePos, Quaternion.identity);
                _object.GetComponent<GamePiece>().InitializePiece(j, i);
                _object.transform.parent = this.transform;
                allObjects[i, j] = _object;
            }
        }
    }

    bool matchesAt(int column, int row, GamePiece piece)
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

    bool columnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        GamePiece firstPiece = matches.GetCurrentMatches()[0].GetComponent<GamePiece>();

        if (firstPiece != null)
        {
            foreach (GameObject gamePiece in matches.GetCurrentMatches())
            {
                GamePiece piece = gamePiece.GetComponent<GamePiece>();
                if(piece.GetRow() == firstPiece.GetRow())
                {
                    numberHorizontal++;
                }
                if(piece.GetColumn() == firstPiece.GetColumn())
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical >= 5 || numberHorizontal >= 5);
    }

    void checkToMakePowerUps() //TODO: Make this prettier
    {
        List<GameObject> _currentMatches = matches.GetCurrentMatches();
        if(_currentMatches.Count == 4 || _currentMatches.Count == 7)
        {
            matches.CheckFlames();
        }
        if(_currentMatches.Count == 5 || _currentMatches.Count == 8)
        {
            if(columnOrRow())
            {
                if(currentPiece != null)
                {
                    if (currentPiece.GetIsMatched())
                    {
                        if (currentPiece.GetSpecialID() != PieceID.LIGHTNING_BOTTLE)
                        {
                            currentPiece.MakeSpecial(PieceID.LIGHTNING_BOTTLE);
                        }
                    }
                    else
                    {
                        if(currentPiece.GetSwappingObject() != null)
                        {
                            GamePiece otherPiece = currentPiece.GetSwappingObject();
                            if (otherPiece.GetIsMatched())
                            {
                                if (otherPiece.GetSpecialID() == PieceID.LIGHTNING_BOTTLE)
                                {
                                    otherPiece.MakeSpecial(PieceID.LIGHTNING_BOTTLE);
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
                    if (currentPiece.GetIsMatched())
                    {
                        if (currentPiece.GetSpecialID() != PieceID.BOMB)
                        {
                            currentPiece.MakeSpecial(PieceID.BOMB);
                        }
                    }
                    else
                    {
                        if (currentPiece.GetSwappingObject() != null)
                        {
                            GamePiece otherPiece = currentPiece.GetSwappingObject();
                            if (otherPiece.GetIsMatched())
                            {
                                if (otherPiece.GetSpecialID() != PieceID.BOMB)
                                {
                                    otherPiece.MakeSpecial(PieceID.BOMB);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void destroyMatchesAt(int column, int row)
    {
        GamePiece _currentPiece = allObjects[column, row].GetComponent<GamePiece>();
        if (_currentPiece.GetIsMatched())
        {
            if(matches.GetCurrentMatches().Count >= 4)
            {
                checkToMakePowerUps();
            }
            scoreSlider.value += _currentPiece.GetScore();
            _currentPiece.OnMatch();
            
            allObjects[column, row] = null;
        }
    }

    IEnumerator decreaseRowCo()
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
                    allObjects[i, j].GetComponent<GamePiece>().SetRow(allObjects[i, j].GetComponent<GamePiece>().GetRow() - nullCount);
                    allObjects[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(decreaseRowWaitTime);
        StartCoroutine(fillBoardCo());
    }

    void refillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allObjects[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    int objectToUse = Random.Range(0, objects.Length);
                    GameObject piece = Instantiate(objects[objectToUse].gameObject, tempPos, Quaternion.identity);
                    piece.transform.parent = transform;
                    allObjects[i, j] = piece;
                    piece.GetComponent<GamePiece>().InitializePiece(j, i);
                }
            }
        }
    }

    bool matchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allObjects[i, j] != null)
                {
                    if (allObjects[i, j].GetComponent<GamePiece>().GetIsMatched())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator fillBoardCo()
    {

        refillBoard();
        yield return new WaitForSeconds(spawnPieceWaitTime);


        while (matchesOnBoard())
        {
            yield return new WaitForSeconds(spawnPieceWaitTime);
            DestroyMatches();
        }
        matches.ClearMatchList();
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

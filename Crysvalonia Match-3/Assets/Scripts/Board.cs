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
    [SerializeField] GameObject[] objects;
    //public GameObject[] destroyEffect; //the objects will tell their own destroy effect, no need to keep comparing string tags

    [SerializeField] Slider scoreSlider; //TODO: Move this to a score script of some kind
    GameController controllerInstance;

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
        controllerInstance = GameController.Instance;
        if (controllerInstance != null)
            objects = controllerInstance.GetGamePieces();
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
                Vector2 _tempPos = new Vector2(i, j + offSet);
                GameObject _backgroundTile = Instantiate(tilePrefab, _tempPos, Quaternion.identity) as GameObject;
                _backgroundTile.transform.parent = this.transform;
                _backgroundTile.name = "(" + i + ", " + j + ")";
                int _objectToUse = Random.Range(0, objects.Length);
                int _maxIterations = 0;
                while(matchesAt(i, j, objects[_objectToUse].GetComponent<GamePiece>()) && _maxIterations < 100)
                {
                    _objectToUse = Random.Range(0, objects.Length);
                    _maxIterations++;
                }

                Vector3 tilePos = new Vector3(_tempPos.x, _tempPos.y, -0.03f);
                GameObject _object = Instantiate(objects[_objectToUse].gameObject, tilePos, Quaternion.identity);
                _object.GetComponent<GamePiece>().InitializePiece(j, i);
                _object.transform.parent = this.transform;
                allObjects[i, j] = _object;
            }
        }
    }

    bool matchesAt(int _column, int _row, GamePiece _piece)
    {
        if(_column > 1 && _row > 1)
        {
            if(allObjects[_column -1, _row].tag == _piece.tag && allObjects[_column - 2, _row].tag == _piece.tag)
            {
                return true;
            }
            if (allObjects[_column, _row - 1].tag == _piece.tag && allObjects[_column, _row - 2].tag == _piece.tag)
            {
                return true;
            }
        }
        else if(_column <= 1 || _row <= 1)
        {
            if(_row > 1)
            {
                if(allObjects[_column, _row -1].tag == _piece.tag && allObjects[_column, _row - 2].tag == _piece.tag)
                {
                    return true;
                }
            }
            if (_column > 1)
            {
                if (allObjects[_column - 1, _row].tag == _piece.tag && allObjects[_column - 2, _row].tag == _piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool columnOrRow()
    {
        int _numberHorizontal = 0;
        int _numberVertical = 0;
        GamePiece _firstPiece = matches.GetCurrentMatches()[0].GetComponent<GamePiece>();

        if (_firstPiece != null)
        {
            foreach (GameObject _gamePiece in matches.GetCurrentMatches())
            {
                GamePiece _piece = _gamePiece.GetComponent<GamePiece>();
                if(_piece.GetRow() == _firstPiece.GetRow())
                {
                    _numberHorizontal++;
                }
                if(_piece.GetColumn() == _firstPiece.GetColumn())
                {
                    _numberVertical++;
                }
            }
        }
        return (_numberVertical >= 5 || _numberHorizontal >= 5);
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
                            GamePiece _otherPiece = currentPiece.GetSwappingObject();
                            if (_otherPiece.GetIsMatched())
                            {
                                if (_otherPiece.GetSpecialID() == PieceID.LIGHTNING_BOTTLE)
                                {
                                    _otherPiece.MakeSpecial(PieceID.LIGHTNING_BOTTLE);
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
                            GamePiece _otherPiece = currentPiece.GetSwappingObject();
                            if (_otherPiece.GetIsMatched())
                            {
                                if (_otherPiece.GetSpecialID() != PieceID.BOMB)
                                {
                                    _otherPiece.MakeSpecial(PieceID.BOMB);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void destroyMatchesAt(int _column, int _row)
    {
        GamePiece _currentPiece = allObjects[_column, _row].GetComponent<GamePiece>();
        if (_currentPiece.GetIsMatched())
        {
            if(matches.GetCurrentMatches().Count >= 4)
            {
                checkToMakePowerUps();
            }
            scoreSlider.value += _currentPiece.GetScore();
            _currentPiece.OnMatch();
            
            allObjects[_column, _row] = null;
        }
    }

    IEnumerator decreaseRowCo()
    {
        int _nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allObjects[i, j] == null)
                {
                    _nullCount++;
                }
                else if (_nullCount > 0)
                {
                    allObjects[i, j].GetComponent<GamePiece>().SetRow(allObjects[i, j].GetComponent<GamePiece>().GetRow() - _nullCount);
                    allObjects[i, j] = null;
                }
            }
            _nullCount = 0;
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
                    Vector2 _tempPos = new Vector2(i, j + offSet);
                    int _objectToUse = Random.Range(0, objects.Length);
                    GameObject _piece = Instantiate(objects[_objectToUse], _tempPos, Quaternion.identity);
                    _piece.transform.parent = transform;
                    allObjects[i, j] = _piece;
                    _piece.GetComponent<GamePiece>().InitializePiece(j, i);
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

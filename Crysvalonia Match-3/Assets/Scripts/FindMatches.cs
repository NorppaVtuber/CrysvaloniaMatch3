using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script will handle any matches that happen
/// </summary>
public class FindMatches : MonoBehaviour
{
    public static FindMatches Instance;

    GameController controllerInstance;
    Board board;

    List<GameObject> currentMatches;

    public List<GameObject> GetCurrentMatches() { return currentMatches; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        controllerInstance = GameController.Instance;
        board = Board.Instance;
        currentMatches = new List<GameObject>(); //always start with an empty list
    }
    
    public void FindAllMatches()
    {
        StartCoroutine(findAllMatchesCo());
    }

    public void CheckFlames()
    {
        GamePieceObject currentPiece = board.GetCurrentPiece();
        if (currentPiece != null)
        {
            bool otherIsMatched = currentPiece.GetSwappingObject().GetIsMatched() && currentPiece.GetSwappingObject() != null;
            if (currentPiece.GetIsMatched())
            {
                if ((currentPiece.GetMoveAngle() > -45 && currentPiece.GetMoveAngle() <= 45) //moves up
                    || (currentPiece.GetMoveAngle() < -135 || currentPiece.GetMoveAngle() >= 135)) //moves down
                {
                    currentPiece.MakeSpecial(PieceID.FLAME, false);
                }
                else //if the piece isn't moving up or down it's moving to the side
                {
                    currentPiece.MakeSpecial(PieceID.FLAME, true);
                }
            }
            else if (otherIsMatched)
            {
                GamePieceObject otherPiece = currentPiece.GetSwappingObject();
                if (otherPiece.GetIsMatched())
                {
                    if ((currentPiece.GetMoveAngle() > -45 && currentPiece.GetMoveAngle() <= 45)
                     || (currentPiece.GetMoveAngle() < -135 || currentPiece.GetMoveAngle() >= 135))
                    {
                        otherPiece.MakeSpecial(PieceID.FLAME, false);
                    }
                    else
                    {
                        otherPiece.MakeSpecial(PieceID.FLAME, true);
                    }
                }
            }
        }
    }

    public void GetPieceIDs(PieceID pieceID)
    {
        GameObject[,] allObjects = board.GetAllObjects();
        for (int i = 0; i < board.GetBoardWidth(); i++)
        {
            for (int j = 0; j < board.GetBoardHeight(); j++)
            {
                if (allObjects[i, j] != null)
                {
                    GamePieceObject currentPiece = allObjects[i, j].GetComponent<GamePieceObject>();
                    if (currentPiece.GetBaseID() == pieceID)
                    {
                        currentPiece.OnMatch();
                    }
                }
            }
        }
    }

    List<GameObject> getAdjacentPieces(int column, int row)
    {
        List<GameObject> adjacentPieces = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j < row + 1; j++)
            {
                if(i >= 0 && i < board.GetBoardWidth() && j >= 0 && j < board.GetBoardHeight())
                {
                    adjacentPieces.Add(board.GetAllObjects()[i, j]);
                    board.GetAllObjects()[i, j].GetComponent<GamePieceObject>().OnMatch();
                }
            }
        }
        return adjacentPieces;
    }

    List<GameObject> isBomb(GamePieceObject piece1, GamePieceObject piece2, GamePieceObject piece3) //TODO: Make this look prettier
    {
        List<GameObject> currentPieces = new List<GameObject>();

        if (piece1.GetSpecialID() == PieceID.BOMB)
        {
            currentMatches.Union(getAdjacentPieces(piece1.GetColumn(), piece1.GetRow()));
        }
        if (piece2.GetSpecialID() == PieceID.BOMB)
        {
            currentMatches.Union(getAdjacentPieces(piece2.GetColumn(), piece2.GetRow()));
        }
        if (piece3.GetSpecialID() == PieceID.BOMB)
        {
            currentMatches.Union(getAdjacentPieces(piece3.GetColumn(), piece3.GetRow()));
        }
        return currentPieces;
    }

    List<GameObject> isColumnRowFlame(GamePieceObject piece1, GamePieceObject piece2, GamePieceObject piece3)
    {
        List<GameObject> currentPieces = new List<GameObject>();

        if (piece1.GetSpecialID() == PieceID.FLAME)
        {
            currentMatches.Union(getColumnRowPieces(piece1.IsColumnFlame(), piece1.GetColumn()));
        }
        if (piece2.GetSpecialID() == PieceID.FLAME)
        {
            currentMatches.Union(getColumnRowPieces(piece2.IsColumnFlame(), piece2.GetColumn()));
        }
        if (piece3.GetSpecialID() == PieceID.FLAME)
        {
            currentMatches.Union(getColumnRowPieces(piece3.IsColumnFlame(), piece3.GetColumn()));
        }
        return currentPieces;
    }

    void addToListAndMatch(GameObject piece)
    {
        if (!currentMatches.Contains(piece))
        {
            currentMatches.Add(piece);
        }
        piece.GetComponent<GamePieceObject>().OnMatch();
    }

    IEnumerator findAllMatchesCo() //TODO: rewrite this so 1. there isn't bazillion nestled if statements and 2. there isn't so much repeat code
    {
        yield return new WaitForSeconds(.2f);
        GameObject[,] allObjects = board.GetAllObjects();

        for (int i = 0; i < board.GetBoardWidth(); i++)
        {
            for (int j = 0; j < board.GetBoardHeight(); j++)
            {
                GameObject currentPiece = allObjects[i, j];
                if (currentPiece != null)
                {
                    GamePieceObject currentPieceComp = currentPiece.GetComponent<GamePieceObject>();
                    if (i > 0 && i < board.GetBoardWidth() - 1)
                    {
                        GameObject leftPiece = allObjects[i - 1, j];
                        GameObject rightPiece = allObjects[i + 1, j];

                        if (leftPiece != null && rightPiece != null)
                        {
                            GamePieceObject leftPieceComp = leftPiece.GetComponent<GamePieceObject>();
                            GamePieceObject rightPieceComp = rightPiece.GetComponent<GamePieceObject>();

                            if (leftPiece.tag == currentPiece.tag && rightPiece.tag == currentPiece.tag)
                            {
                                currentMatches.Union(isColumnRowFlame(currentPieceComp, leftPieceComp, rightPieceComp));
                                currentMatches.Union(isBomb(currentPieceComp, leftPieceComp, rightPieceComp));

                                addToListAndMatch(leftPiece);
                                addToListAndMatch(currentPiece);
                                addToListAndMatch(rightPiece);
                            }
                            
                        }
                    }
                    if (j > 0 && j < board.GetBoardHeight() - 1)
                    {
                        GameObject upPiece = allObjects[i, j + 1];
                        GameObject downPiece = allObjects[i, j - 1];

                        if (upPiece != null && downPiece != null)
                        {
                            GamePieceObject upPieceComp = upPiece.GetComponent<GamePieceObject>();
                            GamePieceObject downPieceComp = downPiece.GetComponent<GamePieceObject>();

                            if (upPiece.tag == currentPiece.tag && downPiece.tag == currentPiece.tag)
                            {

                                currentMatches.Union(isColumnRowFlame(currentPieceComp, upPieceComp, downPieceComp));
                                currentMatches.Union(isBomb(currentPieceComp, upPieceComp, downPieceComp));

                                addToListAndMatch(upPiece);
                                addToListAndMatch(currentPiece);
                                addToListAndMatch(downPiece);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get either a row or a colun of game pieces
    /// </summary>
    /// <param name="isColumn">Is the columnRow value a column?</param> 
    /// <param name="columnRow">Which column or row is wanted?</param> 
    /// <returns></returns>
    List<GameObject> getColumnRowPieces(bool isColumn, int columnRow)
    {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < board.GetBoardHeight(); i++)
        {
            if (isColumn)
            {
                if (board.GetAllObjects()[columnRow, i] != null)
                {
                    pieces.Add(board.GetAllObjects()[columnRow, i]);
                    board.GetAllObjects()[columnRow, i].GetComponent<GamePieceObject>().OnMatch();
                }
            }
            else
            {
                if (board.GetAllObjects()[i, columnRow] != null)
                {
                    pieces.Add(board.GetAllObjects()[i, columnRow]);
                    board.GetAllObjects()[i, columnRow].GetComponent<GamePieceObject>().OnMatch();
                }
            }
        }
        return pieces;
    }
}

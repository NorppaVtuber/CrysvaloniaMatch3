using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script will handle any matches that happen
/// </summary>
public class FindMatches : MonoBehaviour
{
    GameController controllerInstance;
    Board board;

    List<GameObject> currentMatches;

    public List<GameObject> GetCurrentMatches() { return currentMatches; }

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
        if (board.currentPiece != null)
        {
            bool otherIsMatched = board.currentPiece.GetSwappingObject().GetIsMatched() && board.currentPiece.GetSwappingObject() != null;
            if (board.currentPiece.GetIsMatched())
            {
                if ((board.currentPiece.GetMoveAngle() > -45 && board.currentPiece.GetMoveAngle() <= 45) //moves up
                    || (board.currentPiece.GetMoveAngle() < -135 || board.currentPiece.GetMoveAngle() >= 135)) //moves down
                {
                    board.currentPiece.MakeSpecial(PieceID.ROW_FLAME);
                }
                else //if the piece isn't moving up or down it's moving to the side
                {
                    board.currentPiece.MakeSpecial(PieceID.COLUMN_FLAME);
                }
            }
            else if (otherIsMatched)
            {
                GamePieceObject otherPiece = board.currentPiece.GetSwappingObject();
                if (otherPiece.GetIsMatched())
                {
                    if ((board.currentPiece.GetMoveAngle() > -45 && board.currentPiece.GetMoveAngle() <= 45)
                     || (board.currentPiece.GetMoveAngle() < -135 || board.currentPiece.GetMoveAngle() >= 135))
                    {
                        otherPiece.MakeSpecial(PieceID.ROW_FLAME);
                    }
                    else
                    {
                        otherPiece.MakeSpecial(PieceID.COLUMN_FLAME);
                    }
                }
            }
        }
    }

    public void GetPieceIDs(PieceID pieceID)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allObjects[i, j] != null)
                {
                    GamePieceObject currentPiece = board.allObjects[i, j].GetComponent<GamePieceObject>();
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
                if(i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    adjacentPieces.Add(board.allObjects[i, j]);
                    board.allObjects[i, j].GetComponent<GamePieceObject>().OnMatch();
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

    List<GameObject> isRowFlame(GamePieceObject piece1, GamePieceObject piece2, GamePieceObject piece3)
    {
        List<GameObject> currentPieces = new List<GameObject>();

        if (piece1.GetSpecialID() == PieceID.ROW_FLAME)
        {
            currentMatches.Union(getRowPieces(piece1.GetRow()));
        }
        if (piece2.GetSpecialID() == PieceID.ROW_FLAME)
        {
            currentMatches.Union(getRowPieces(piece2.GetRow()));
        }
        if (piece3.GetSpecialID() == PieceID.ROW_FLAME)
        {
            currentMatches.Union(getRowPieces(piece3.GetRow()));
        }
        return currentPieces;
    }

    List<GameObject> isColumnFlame(GamePieceObject piece1, GamePieceObject piece2, GamePieceObject piece3)
    {
        List<GameObject> currentPieces = new List<GameObject>();

        if (piece1.GetSpecialID() == PieceID.COLUMN_FLAME)
        {
            currentMatches.Union(getColumnPieces(piece1.GetColumn()));
        }
        if (piece2.GetSpecialID() == PieceID.COLUMN_FLAME)
        {
            currentMatches.Union(getColumnPieces(piece2.GetColumn()));
        }
        if (piece3.GetSpecialID() == PieceID.COLUMN_FLAME)
        {
            currentMatches.Union(getColumnPieces(piece3.GetColumn()));
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

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentPiece = board.allObjects[i, j];
                if (currentPiece != null)
                {
                    GamePieceObject currentPieceComp = currentPiece.GetComponent<GamePieceObject>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftPiece = board.allObjects[i - 1, j];
                        GameObject rightPiece = board.allObjects[i + 1, j];

                        if (leftPiece != null && rightPiece != null)
                        {
                            GamePieceObject leftPieceComp = leftPiece.GetComponent<GamePieceObject>();
                            GamePieceObject rightPieceComp = rightPiece.GetComponent<GamePieceObject>();

                            if (leftPiece.tag == currentPiece.tag && rightPiece.tag == currentPiece.tag)
                            {
                                currentMatches.Union(isRowFlame(currentPieceComp, leftPieceComp, rightPieceComp));
                                currentMatches.Union(isColumnFlame(currentPieceComp, leftPieceComp, rightPieceComp));
                                currentMatches.Union(isBomb(currentPieceComp, leftPieceComp, rightPieceComp));

                                addToListAndMatch(leftPiece);
                                addToListAndMatch(currentPiece);
                                addToListAndMatch(rightPiece);
                            }
                            
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upPiece = board.allObjects[i, j + 1];
                        GameObject downPiece = board.allObjects[i, j - 1];

                        if (upPiece != null && downPiece != null)
                        {
                            GamePieceObject upPieceComp = upPiece.GetComponent<GamePieceObject>();
                            GamePieceObject downPieceComp = downPiece.GetComponent<GamePieceObject>();

                            if (upPiece.tag == currentPiece.tag && downPiece.tag == currentPiece.tag)
                            {

                                currentMatches.Union(isColumnFlame(currentPieceComp, upPieceComp, downPieceComp));
                                currentMatches.Union(isRowFlame(currentPieceComp, upPieceComp, downPieceComp));
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

    List<GameObject> getColumnPieces(int column)
    {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if(board.allObjects[column, i] != null)
            {
                pieces.Add(board.allObjects[column, i]);
                board.allObjects[column, i].GetComponent<GamePieceObject>().OnMatch();
            }
        }
        return pieces;
    }

    List<GameObject> getRowPieces(int row)
    {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allObjects[i, row] != null)
            {
                pieces.Add(board.allObjects[i, row]);
                board.allObjects[i, row].GetComponent<GamePieceObject>().OnMatch();
            }
        }
        return pieces;
    }
}

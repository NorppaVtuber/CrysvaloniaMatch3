using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsBomb(GamePiece piece1, GamePiece piece2, GamePiece piece3)
    {
        List<GameObject> currentPieces = new List<GameObject>();

        if (piece1.isBomb)
        {
            currentMatches.Union(GetAdjacentPieces(piece1.column, piece1.row));
        }
        if (piece2.isBomb)
        {
            currentMatches.Union(GetAdjacentPieces(piece2.column, piece2.row));
        }
        if (piece3.isBomb)
        {
            currentMatches.Union(GetAdjacentPieces(piece3.column, piece3.row));
        }
        return currentPieces;
    }

    private List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> adjacentPieces = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j < row + 1; j++)
            {
                if(i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    adjacentPieces.Add(board.allObjects[i, j]);
                    board.allObjects[i, j].GetComponent<GamePiece>().isMatched = true;
                }
            }
        }
        return adjacentPieces;
    }

    private List<GameObject> IsRowFlame(GamePiece piece1, GamePiece piece2, GamePiece piece3)
    {
        List<GameObject> currentPieces = new List<GameObject>();

        if (piece1.isRowFlame)
        {
            currentMatches.Union(GetRowPieces(piece1.row));
        }
        if (piece2.isRowFlame)
        {
            currentMatches.Union(GetRowPieces(piece2.row));
        }
        if (piece3.isRowFlame)
        {
            currentMatches.Union(GetRowPieces(piece3.row));
        }
        return currentPieces;
    }

    private List<GameObject> IsColumnFlame(GamePiece piece1, GamePiece piece2, GamePiece piece3)
    {
        List<GameObject> currentPieces = new List<GameObject>();

        if (piece1.isColumnFlame)
        {
            currentMatches.Union(GetColumnPieces(piece1.column));
        }
        if (piece2.isColumnFlame)
        {
            currentMatches.Union(GetColumnPieces(piece2.column));
        }
        if (piece3.isColumnFlame)
        {
            currentMatches.Union(GetColumnPieces(piece3.column));
        }
        return currentPieces;
    }

    private void AddToListAndMatch(GameObject piece)
    {
        if (!currentMatches.Contains(piece))
        {
            currentMatches.Add(piece);
        }
        piece.GetComponent<GamePiece>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject piece1, GameObject piece2, GameObject piece3)
    {
        AddToListAndMatch(piece1);
        AddToListAndMatch(piece2);
        AddToListAndMatch(piece3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentPiece = board.allObjects[i, j];
                if (currentPiece != null)
                {
                    GamePiece currentPieceComp = currentPiece.GetComponent<GamePiece>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftPiece = board.allObjects[i - 1, j];
                        GameObject rightPiece = board.allObjects[i + 1, j];

                        if (leftPiece != null && rightPiece != null)
                        {
                            GamePiece leftPieceComp = leftPiece.GetComponent<GamePiece>();
                            GamePiece rightPieceComp = rightPiece.GetComponent<GamePiece>();

                            if (leftPiece.tag == currentPiece.tag && rightPiece.tag == currentPiece.tag)
                            {
                                currentMatches.Union(IsRowFlame(currentPieceComp, leftPieceComp, rightPieceComp));
                                currentMatches.Union(IsColumnFlame(currentPieceComp, leftPieceComp, rightPieceComp));
                                currentMatches.Union(IsBomb(currentPieceComp, leftPieceComp, rightPieceComp));

                                GetNearbyPieces(leftPiece, currentPiece, rightPiece);
                            }
                            
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upPiece = board.allObjects[i, j + 1];
                        GameObject downPiece = board.allObjects[i, j - 1];

                        if (upPiece != null && downPiece != null)
                        {
                            GamePiece upPieceComp = upPiece.GetComponent<GamePiece>();
                            GamePiece downPieceComp = downPiece.GetComponent<GamePiece>();

                            if (upPiece.tag == currentPiece.tag && downPiece.tag == currentPiece.tag)
                            {

                                currentMatches.Union(IsColumnFlame(currentPieceComp, upPieceComp, downPieceComp));
                                currentMatches.Union(IsRowFlame(currentPieceComp, upPieceComp, downPieceComp));
                                currentMatches.Union(IsBomb(currentPieceComp, upPieceComp, downPieceComp));

                                GetNearbyPieces(upPiece, currentPiece, downPiece);

                            }
                        }
                    }
                }
            }
        }
    }

    public void GetColors(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if(board.allObjects[i, j] != null)
                {
                    if(board.allObjects[i, j].tag == color)
                    {
                        board.allObjects[i, j].GetComponent<GamePiece>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if(board.allObjects[column, i] != null)
            {
                pieces.Add(board.allObjects[column, i]);
                board.allObjects[column, i].GetComponent<GamePiece>().isMatched = true;
            }
        }
        return pieces;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allObjects[i, row] != null)
            {
                pieces.Add(board.allObjects[i, row]);
                board.allObjects[i, row].GetComponent<GamePiece>().isMatched = true;
            }
        }
        return pieces;
    }

    public void CheckFlames()
    {
        if(board.currentPiece != null)
        {
            bool otherIsMatched = board.currentPiece.otherObject.GetComponent<GamePiece>().isMatched && board.currentPiece.otherObject != null;
            if (board.currentPiece.isMatched)
            {
                board.currentPiece.isMatched = false;
                if((board.currentPiece.moveAngle > -45 && board.currentPiece.moveAngle <= 45)
                    || (board.currentPiece.moveAngle < -135 || board.currentPiece.moveAngle >= 135))
                {
                    board.currentPiece.MakeRowFlame();
                }
                else
                {
                    board.currentPiece.MakeColumnFlame();
                }
            }
            else if(otherIsMatched)
            {
                GamePiece otherPiece = board.currentPiece.otherObject.GetComponent<GamePiece>();
                if(otherPiece.isMatched)
                {
                    otherPiece.isMatched = false;
                    if ((board.currentPiece.moveAngle > -45 && board.currentPiece.moveAngle <= 45)
                     || (board.currentPiece.moveAngle < -135 || board.currentPiece.moveAngle >= 135))
                    {
                        otherPiece.MakeRowFlame();
                    }
                    else
                    {
                        otherPiece.MakeColumnFlame();
                    }
                }
            }
        }
    }
}

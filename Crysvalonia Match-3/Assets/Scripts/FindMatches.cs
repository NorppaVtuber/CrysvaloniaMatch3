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

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentPiece = board.allObjects[i, j];
                if(currentPiece != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftPiece = board.allObjects[i - 1, j];
                        GameObject rightPiece = board.allObjects[i + 1, j];
                        if(leftPiece != null && rightPiece != null)
                        {
                            if(leftPiece.tag == currentPiece.tag && rightPiece.tag == currentPiece.tag)
                            {
                                if(currentPiece.GetComponent<GamePiece>().isRowFlame
                                    || leftPiece.GetComponent<GamePiece>().isRowFlame
                                    || rightPiece.GetComponent<GamePiece>().isRowFlame)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }
                                if(currentPiece.GetComponent<GamePiece>().isColumnFlame)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }
                                if (leftPiece.GetComponent<GamePiece>().isColumnFlame)
                                {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                }
                                if (rightPiece.GetComponent<GamePiece>().isColumnFlame)
                                {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                }

                                if (!currentMatches.Contains(leftPiece))
                                {
                                    currentMatches.Add(leftPiece);
                                }
                                leftPiece.GetComponent<GamePiece>().isMatched = true;
                                if (!currentMatches.Contains(rightPiece))
                                {
                                    currentMatches.Add(rightPiece);
                                }
                                rightPiece.GetComponent<GamePiece>().isMatched = true;
                                if (!currentMatches.Contains(currentPiece))
                                {
                                    currentMatches.Add(currentPiece);
                                }
                                currentPiece.GetComponent<GamePiece>().isMatched = true;
                            }
                        }
                    }
                }
                if (j > 0 && j < board.height - 1)
                {
                    GameObject upPiece = board.allObjects[i, j + 1];
                    GameObject downPiece = board.allObjects[i, j - 1];
                    if (upPiece != null && downPiece != null)
                    {
                        if (upPiece.tag == currentPiece.tag && downPiece.tag == currentPiece.tag)
                        {
                            if (currentPiece.GetComponent<GamePiece>().isColumnFlame
                                    || downPiece.GetComponent<GamePiece>().isColumnFlame
                                    || upPiece.GetComponent<GamePiece>().isColumnFlame)
                            {
                                currentMatches.Union(GetColumnPieces(i));
                            }
                            if (currentPiece.GetComponent<GamePiece>().isRowFlame)
                            {
                                currentMatches.Union(GetRowPieces(j));
                            }
                            if (upPiece.GetComponent<GamePiece>().isRowFlame)
                            {
                                currentMatches.Union(GetRowPieces(j + 1));
                            }
                            if (downPiece.GetComponent<GamePiece>().isRowFlame)
                            {
                                currentMatches.Union(GetRowPieces(j - 1));
                            }

                            if (!currentMatches.Contains(upPiece))
                            {
                                currentMatches.Add(upPiece);
                            }
                            upPiece.GetComponent<GamePiece>().isMatched = true;
                            if (!currentMatches.Contains(downPiece))
                            {
                                currentMatches.Add(downPiece);
                            }
                            downPiece.GetComponent<GamePiece>().isMatched = true;
                            if (!currentMatches.Contains(currentPiece))
                            {
                                currentMatches.Add(currentPiece);
                            }
                            currentPiece.GetComponent<GamePiece>().isMatched = true;

                        }
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
            bool otherIsMatched = board.currentPiece.otherObject.GetComponent<GamePiece>().isMatched;
            if (board.currentPiece.isMatched)
            {
                board.currentPiece.isMatched = false;
                int typeOfFlame = Random.Range(0, 100);
                if(typeOfFlame < 50)
                {
                    board.currentPiece.MakeColumnFlame();
                }
                else if(typeOfFlame >= 50)
                {
                    board.currentPiece.MakeRowFlame();
                }
            }
            else if(otherIsMatched)
            {

            }
        }
    }
}

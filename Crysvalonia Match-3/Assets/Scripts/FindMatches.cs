using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

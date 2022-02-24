using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    [Header("Board variables")]
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private int prevColumn;
    private int prevRow;
    public bool isMatched = false;

    private Board board;
    private GameObject otherObject;

    private Vector2 firstPos;
    private Vector2 finalPos;
    private Vector2 tempPos;
    private float moveAngle;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        prevRow = row;
        prevColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMatched)
        {
            //remove game piece
        }
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .1f);
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allObjects[column, row] = this.gameObject;
        }
        if(Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .1f);
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            board.allObjects[column, row] = this.gameObject;
        }
        targetX = column;
        targetY = row;
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherObject != null)
        {
            if(!isMatched && !otherObject.GetComponent<GamePiece>().isMatched)
            {
                otherObject.GetComponent<GamePiece>().row = row;
                otherObject.GetComponent<GamePiece>().column = column;
                row = prevRow;
                column = prevColumn;
            }
            otherObject = null;
        }
    }

    void OnMouseDown()
    {
        firstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        finalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        moveAngle = Mathf.Atan2(finalPos.y - firstPos.y, finalPos.x - firstPos.x) * 180 / Mathf.PI;
        Debug.Log(moveAngle);
        if(moveAngle != 0) // figure out something better for this bug
        {
           MovePieces(); 
        }
    }

    void MovePieces()
    {
        if(moveAngle > -45 && moveAngle <= 45 && column < board.width - 1)
        {
            //Right
            otherObject = board.allObjects[column = (column + 1), row];
            otherObject.GetComponent<GamePiece>().column = (column - 1);
            //column = (column + 1);
        }
        else if(moveAngle > 45 && moveAngle <= 135 && row < board.height - 1)
        {
            //Up
            otherObject = board.allObjects[column, row = (row + 1)];
            otherObject.GetComponent<GamePiece>().row = (row - 1);
            //row = (row + 1);
        }
        else if((moveAngle > 135 || moveAngle <= -135) && column > 0)
        {
            //Left
            otherObject = board.allObjects[column = (column - 1), row];
            otherObject.GetComponent<GamePiece>().column = (column + 1);
            //column = (column - 1);
        }
        else if((moveAngle < -45 || moveAngle >= 135) && row > 0)
        {
            //Down
            otherObject = board.allObjects[column, row = (row - 1)];
            otherObject.GetComponent<GamePiece>().row = (row + 1);
            //row = (row - 1);
        }
        FindMatches();
        StartCoroutine(CheckMoveCo());
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftObject1 = board.allObjects[column - 1, row];
            GameObject rightObject1 = board.allObjects[column + 1, row];
            GameObject leftObject2 = null;
            GameObject rightObject2 = null;
            if(column + 2 < board.width - 1)
            {
                rightObject2 = board.allObjects[column + 2, row];
            }
            if(column - 2 > 0)
            {
                leftObject2 = board.allObjects[column - 2, row];
            }
            if (leftObject1.tag == this.gameObject.tag && rightObject1.tag == this.gameObject.tag)
            {
                leftObject1.GetComponent<GamePiece>().isMatched = true;
                rightObject1.GetComponent<GamePiece>().isMatched = true;
                isMatched = true;
            }
            else if (leftObject2 != null)
            {
                if (leftObject1.tag == this.gameObject.tag && leftObject2.tag == this.gameObject.tag)
                {
                    leftObject1.GetComponent<GamePiece>().isMatched = true;
                    leftObject2.GetComponent<GamePiece>().isMatched = true;
                    isMatched = true;
                }
            }
            else if(rightObject2 != null)
            {
                if (rightObject1.tag == this.gameObject.tag && rightObject2.tag == this.gameObject.tag)
                {
                    rightObject1.GetComponent<GamePiece>().isMatched = true;
                    rightObject2.GetComponent<GamePiece>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upObject1 = board.allObjects[column, row + 1];
            GameObject downObject1 = board.allObjects[column, row - 1];
            GameObject upObject2 = null;
            GameObject downObject2 = null;
            if (row + 2 < board.height - 1)
            {
                upObject2 = board.allObjects[column, row + 2];
            }
            if(row - 2 > 0)
            {
                downObject2 = board.allObjects[column, row - 2];
            }
            if (upObject1.tag == this.gameObject.tag && downObject1.tag == this.gameObject.tag)
            {
                upObject1.GetComponent<GamePiece>().isMatched = true;
                downObject1.GetComponent<GamePiece>().isMatched = true;
                isMatched = true;
            }
            else if (upObject2 != null)
            {
                if (upObject1.tag == this.gameObject.tag && upObject2.tag == this.gameObject.tag)
                {
                    upObject1.GetComponent<GamePiece>().isMatched = true;
                    upObject2.GetComponent<GamePiece>().isMatched = true;
                    isMatched = true;
                }
            }
            else if(downObject2 != null)
            { 
                if (downObject1.tag == this.gameObject.tag && downObject2.tag == this.gameObject.tag)
                {
                    downObject1.GetComponent<GamePiece>().isMatched = true;
                    downObject2.GetComponent<GamePiece>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        Debug.Log("Match Found");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private Board board;
    private GameObject otherObject;

    private Vector2 firstPos;
    private Vector2 finalPos;
    private Vector2 tempPos;
    public float moveAngle;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .4f);
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allObjects[column, row] = this.gameObject;
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
        MovePieces();
    }

    void MovePieces()
    {
        if(moveAngle > -45 && moveAngle <= 45 && column < board.width)
        {
            //Right
            otherObject = board.allObjects[column++, row];
            otherObject.GetComponent<GamePiece>().column--;
            column++;
        }
        else if(moveAngle > 45 && moveAngle <= 135 && row < board.height)
        {
            //Up
            otherObject = board.allObjects[column, row++];
            otherObject.GetComponent<GamePiece>().row--;
            row++;
        }
        else if((moveAngle > 135 || moveAngle <= -135) && column > 0)
        {
            //Left
            otherObject = board.allObjects[column--, row];
            otherObject.GetComponent<GamePiece>().column++;
            column--;
        }
        else if(moveAngle < -45 && moveAngle >= 135 && row > 0)
        {
            //Down
            otherObject = board.allObjects[column, row--];
            otherObject.GetComponent<GamePiece>().row++;
            row--;
        }
    }
}

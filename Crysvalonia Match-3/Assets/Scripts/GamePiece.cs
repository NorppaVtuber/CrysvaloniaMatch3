using System.Collections;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    [Header("Board variables")]
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    public int prevColumn;
    public int prevRow;
    public bool isMatched = false;

    private Board board;
    private GameObject otherObject;

    private Vector2 firstPos;
    private Vector2 finalPos;
    private Vector2 tempPos;
    private float moveAngle;
    private float swipeResist = 1f;

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
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>(); // change the opacity of the matched sprite
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1) //move towards target
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .1f);
            if(board.allObjects[column, row] != this.gameObject)
            {
                board.allObjects[column, row] = this.gameObject;
            }
        }
        else //directly set position
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        if(Mathf.Abs(targetY - transform.position.y) > .1) //same but in y position
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .1f); 
            if (board.allObjects[column, row] != this.gameObject)
            {
                board.allObjects[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
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
            else
            {
                board.DestroyMatches();
            }
            otherObject = null;
        } 
    }

    private void OnMouseDown()
    {
        firstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        if(Mathf.Abs(finalPos.y - firstPos.y) > swipeResist || Mathf.Abs(finalPos.x - firstPos.x) > swipeResist)
        {
            moveAngle = Mathf.Atan2(finalPos.y - firstPos.y, finalPos.x - firstPos.x) * 180 / Mathf.PI;
            Debug.Log(moveAngle);
            MovePieces();
        }
    }

    private void MovePieces()
    {
        if(moveAngle > -45 && moveAngle <= 45 && column < board.width - 1)
        {
            //Right
            otherObject = board.allObjects[column = (column + 1), row];
            otherObject.GetComponent<GamePiece>().column = (column - 1);
        }
        else if(moveAngle > 45 && moveAngle <= 135 && row < board.height - 1)
        {
            //Up
            otherObject = board.allObjects[column, row = (row + 1)];
            otherObject.GetComponent<GamePiece>().row = (row - 1);
        }
        else if((moveAngle > 135 || moveAngle <= -135) && column > 0)
        {
            //Left
            otherObject = board.allObjects[column = (column - 1), row];
            otherObject.GetComponent<GamePiece>().column = (column + 1);
        }
        else if((moveAngle < -45 || moveAngle >= 135) && row > 0)
        {
            //Down
            otherObject = board.allObjects[column, row = (row - 1)];
            otherObject.GetComponent<GamePiece>().row = (row + 1);
        }
        StartCoroutine(CheckMoveCo());
    }

    void FindMatches() //column = pystyyn, row = vaakaan
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject left1 = board.allObjects[column - 1, row];
            GameObject right1 = board.allObjects[column + 1, row];
            if (left1 != null && right1 != null)
            {
                if (left1.tag == this.gameObject.tag && right1.tag == this.gameObject.tag)
                {
                    left1.GetComponent<GamePiece>().isMatched = true;
                    right1.GetComponent<GamePiece>().isMatched = true;
                    isMatched = true;
                    Debug.Log("Match");
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject up1 = board.allObjects[column, row + 1];
            GameObject down1 = board.allObjects[column, row - 1];
            if (up1 != null && down1 != null)
            {
                if (up1.tag == this.gameObject.tag && down1.tag == this.gameObject.tag)
                {
                    up1.GetComponent<GamePiece>().isMatched = true;
                    down1.GetComponent<GamePiece>().isMatched = true;
                    isMatched = true;
                    Debug.Log("Match");
                }
            }
        }
    }
}

using System.Collections;
using UnityEngine;

public class GamePiece : MonoBehaviour //TODO: Delete once all pieces can function as Scriptable Objects
{
    /*[Header("Board variables")]
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    public int prevColumn;
    public int prevRow;
    public bool isMatched = false;
    public int score;

    [Header("PowerUp Stuffs")]
    public bool isLightning;
    public bool isColumnFlame;
    public bool isRowFlame;
    public bool isBomb;
    public GameObject columnFlame;
    public GameObject rowFlame;
    public GameObject lightningBottle;
    public GameObject bomb;

    private FindMatches matches;

    private Board board;
    public GameObject otherObject;

    private Vector2 firstPos;
    private Vector2 finalPos;
    private Vector2 tempPos;
    public float moveAngle;
    private float swipeResist = 1f;

    // Start is called before the first frame update
    void Start()
    {
        isColumnFlame = false;
        isRowFlame = false;
        isLightning = false;
        isBomb = false;

        board = FindObjectOfType<Board>();
        matches = FindObjectOfType<FindMatches>();
        isColumnFlame = false;
        isRowFlame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>(); // change the opacity of the matched sprite
            mySprite.color = new Color(1f, 1f, 1f, 1f);
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
            matches.FindAllMatches();
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
            matches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
        if(Moves.remainingMoves <= 0)
        {
            board.currentState = GameState.OVER;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if (isLightning)
        {
            matches.GetColors(otherObject.tag);
            isMatched = true;
        }
        else if (otherObject.GetComponent<GamePiece>().isLightning)
        {
            matches.GetColors(this.gameObject.tag);
            otherObject.GetComponent<GamePiece>().isMatched = true;
        }

        yield return new WaitForSeconds(.5f);
        if(otherObject != null)
        {
            if(!isMatched && !otherObject.GetComponent<GamePiece>().isMatched)
            {
                otherObject.GetComponent<GamePiece>().row = row;
                otherObject.GetComponent<GamePiece>().column = column;
                row = prevRow;
                column = prevColumn;
                yield return new WaitForSeconds(.5f);
                board.currentPiece = null;
                board.currentState = GameState.MOVE;
            }
            else
            {
                Moves.remainingMoves--;
                board.DestroyMatches();
            }
            //otherObject = null;
        } 
    }

    private void OnMouseDown()
    {
        if(board.currentState == GameState.MOVE)
        {
            firstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.MOVE)
        {
            finalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
        
    }

    private void CalculateAngle()
    {
        if(!Menu.isPaused)
        {
            if (Mathf.Abs(finalPos.y - firstPos.y) > swipeResist || Mathf.Abs(finalPos.x - firstPos.x) > swipeResist)
            {
                board.currentState = GameState.WAIT;
                moveAngle = Mathf.Atan2(finalPos.y - firstPos.y, finalPos.x - firstPos.x) * 180 / Mathf.PI;
                //Debug.Log(moveAngle);
                MovePieces();
                board.currentPiece = this;
            }
            else
            {
                board.currentState = GameState.MOVE;
            }
        }
    }

    private void MovePieces()
    {
        if(moveAngle > -45 && moveAngle <= 45 && column < board.width - 1)
        {
            //Right
            prevRow = row;
            prevColumn = column;
            otherObject = board.allObjects[column = (column + 1), row];
            otherObject.GetComponent<GamePiece>().column = (column - 1);
            StartCoroutine(CheckMoveCo());
        }
        else if(moveAngle > 45 && moveAngle <= 135 && row < board.height - 1)
        {
            //Up
            prevRow = row;
            prevColumn = column;
            otherObject = board.allObjects[column, row = (row + 1)];
            otherObject.GetComponent<GamePiece>().row = (row - 1);
            StartCoroutine(CheckMoveCo());
        }
        else if((moveAngle > 135 || moveAngle <= -135) && column > 0)
        {
            //Left
            prevRow = row;
            prevColumn = column;
            otherObject = board.allObjects[column = (column - 1), row];
            otherObject.GetComponent<GamePiece>().column = (column + 1);
            StartCoroutine(CheckMoveCo());
        }
        else if((moveAngle < -45 || moveAngle >= 135) && row > 0)
        {
            //Down
            prevRow = row;
            prevColumn = column;
            otherObject = board.allObjects[column, row = (row - 1)];
            otherObject.GetComponent<GamePiece>().row = (row + 1);
            StartCoroutine(CheckMoveCo());
        }
        
         board.currentState = GameState.MOVE;
        
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

    public void MakeColumnFlame()
    {
        isColumnFlame = true;
        GameObject flame = Instantiate(columnFlame, transform.position, Quaternion.identity);
        flame.transform.parent = this.transform;
    }

    public void MakeRowFlame()
    {
        isRowFlame = true;
        GameObject flame = Instantiate(rowFlame, transform.position, Quaternion.identity);
        flame.transform.parent = this.transform;
    }

    public void MakeLightningBottle()
    {
        isLightning = true;
        GameObject bottle = Instantiate(lightningBottle, transform.position, Quaternion.identity);
        bottle.transform.parent = this.transform;
        SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
        sprite.color = new Color(0f, 0f, 0f, 0f);
    }

    public void MakeBomb()
    {
        isBomb = true;
        GameObject bomber = Instantiate(bomb, transform.position, Quaternion.identity);
        bomber.transform.parent = this.transform;
    }*/
}

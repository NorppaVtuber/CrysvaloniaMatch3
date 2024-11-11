using UnityEngine;

/// <summary>
/// This script is the base for all the new game pieces.
/// </summary>
[CreateAssetMenu(fileName = "GamePiece", menuName = "Scriptable Objects/GamePiece")]
public class GamePieceObject : ScriptableObject
{
    [SerializeField] PieceID baseID = PieceID.NONE;
    [SerializeField] PieceID specialID = PieceID.NONE;

    public PieceID GetBaseID() { return baseID; }
    public PieceID GetSpecialID() { return specialID; }

    int column;
    int row;
    public int GetRow() { return row; }
    public int GetColumn() { return column; }

    bool isColumnFlame;
    public bool IsColumnFlame() { return isRowFlame; }

    int targetX;
    int targetY;

    int prevColumn;
    int prevRow;

    bool isMatched;
    public bool GetIsMatched() { return isMatched; }

    [SerializeField] int score = 200;

    [SerializeField] Sprite mySprite;
    [SerializeField] GameObject destroyParticlePrefab;

    GamePieceObject swappingObject;
    public GamePieceObject GetSwappingObject() { return swappingObject; }

    Vector2 firstPos;
    Vector2 secondPos;
    Vector2 helperPos;
    float moveAngle;
    public float GetMoveAngle() { return moveAngle; }
    [SerializeField] float swipeResist = 1f; //the minimum distance a piece has to be moved for the move to "count". This is needed to avoid wasting moves on misclicks and taps.

    Board board;
    FindMatches matches;

    public void InitializePiece()
    {
        if(baseID == PieceID.NONE)
        {
            Debug.LogError("baseID is not set!");
        }
        specialID = PieceID.NONE; //none of the pieces can be special right after spawning in

        board = Board.Instance;
        matches = FindMatches.Instance;
    }

    public void OnMatch()
    {
        isMatched = true;

        if(specialID == PieceID.LIGHTNING_BOTTLE)
        {
            matches.GetPieceIDs(swappingObject.GetSpecialID());
        }
    }

    /// <summary>
    /// Set a new special ID for the game piece
    /// </summary>
    /// <param name="newSpecialID">What is the new special ID?</param>
    /// <param name="columnFlame">If the new special ID is "Flame", will this be a column flame?</param>
    public void MakeSpecial(PieceID newSpecialID, bool columnFlame)
    {
        if (specialID == PieceID.NONE) //if a piece is already special, let's not change its specialty
            return;

        isMatched = false;
        specialID = newSpecialID;

        if (columnFlame)
            isColumnFlame = true;
        else
            isColumnFlame = false;
    }
}

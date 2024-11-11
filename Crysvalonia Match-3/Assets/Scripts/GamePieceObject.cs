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
    int targetX;
    int targetY;
    int prevColumn;
    int prevRow;
    bool isMatched;
    [SerializeField] int score = 200;

    [SerializeField] Sprite mySprite;
    [SerializeField] GameObject destroyParticlePrefab;

    GamePieceObject swappingObject;

    Vector2 firstPos;
    Vector2 secondPos;
    Vector2 helperPos;
    float moveAngle;
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

        board = board.Instance;
        matches = FindFirstObjectByType<FindMatches>(); //There will only ever be one FindMatches
    }

    public void OnMatch()
    {
        isMatched = true;

        if(specialID == PieceID.LIGHTNING_BOTTLE)
        {
            matches.GetColors(swappingObject.GetSpecialID()); //TODO: Change matches.GetColors to function with PieceID instead
        }
    }
}

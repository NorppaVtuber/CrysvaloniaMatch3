using UnityEngine;

public enum GameState
{
    WAIT,
    MOVE,
    OVER
}

public enum GameDifficulty
{
    EASY,
    NORMAL,
    HARD,
    EXTRA_HARD
}

public enum PieceID
{
    //Each piece will have a "basic" ID and a special ID.
    NONE,

    //basic pieces
    CARROT,
    FEATHER,
    MOUSE,
    CLOVER,
    CHEESE,
    DOG_TREAT,
    PUMPKIN,
    STARFISH,

    //special pieces
    LIGHTNING_BOTTLE,
    ROW_FLAME,
    COLUMN_FLAME,
    BOMB
}

public enum MenuID
{
    START_MENU,
    OPTIONS_MENU,
    CREDITS_MENU,
    RESET_MENU
}
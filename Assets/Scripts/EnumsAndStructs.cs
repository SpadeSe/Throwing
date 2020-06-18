using System.Collections.Generic;

[System.Serializable]
public enum PlayerSide
{
    RED,
    BLUE
}

[System.Serializable]
public struct SideRecords
{
    PlayerSide side;
    List<Player> players;
    int Score;
    int DeadCount;
}
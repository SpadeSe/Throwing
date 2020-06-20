using System.Collections.Generic;
using UnityEngine;

public class Definitions
{
    public static string playerControllerTag = "PlayerController";
}

[System.Serializable]
public enum PlayerSide
{
    RED,
    BLUE
}

[System.Serializable]
public class SideRecords
{
    public PlayerSide side;
    public List<PlayerCharacter> players;
    public int Score;
    public int DeadCount;
}

[System.Serializable]
public struct CharacterSelectInfo
{
    public Sprite displaySprite;
    public string description;
    public GameObject displayPrefab;
    public GameObject inGamePrefab;
}

[System.Serializable]
public struct RoomRecord
{
    public int id;
    public string name;
    public CharacterSelectInfo charInfo;
    public PlayerSide side;

    public RoomRecord(int id, string name, CharacterSelectInfo charInfo, PlayerSide side) : this()
    {
        this.id = id;
        this.name = name;
        this.charInfo = charInfo;
        this.side = side;
    }
}
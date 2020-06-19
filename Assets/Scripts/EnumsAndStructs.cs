using System.Collections.Generic;
using UnityEngine;

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
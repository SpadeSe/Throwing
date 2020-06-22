using System.Collections.Generic;
using UnityEngine;
using System.Text;
using ExitGames.Client.Photon;

public class Definitions
{
    public static string playerControllerTag = "PlayerController";
    public static string roomRecorderTag = "RoomRecorder";
    public static string playerStartTag = "PlayerStart";

    public static string characterSelectDisplaySpriteResourcePath = "displaySprite/";
    public static string inGameCharacterPrefabResourcePath = "charactersFinal/";
}

public enum RoomState: byte
{
    Preparing,   //房间中未准备状态
    ReadyToPlay, //已经准备, 处于进入游戏前
    Playing,     //进入了游戏, 正在玩
    EndPlay      //在游戏结算, 或已经结算完成
}

[System.Serializable]
public enum PlayerSide: byte
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
public class RoomRecord
{
    public int id;
    public string playerName;
    public string displaySpriteName;
    public string inGamePrefabName;
    public PlayerSide side;

    public RoomRecord()
    {

    }

    public RoomRecord(int id, string playerName, string dsname, string igpname, PlayerSide side)
    {
        this.id = id;
        this.playerName = playerName;
        displaySpriteName = dsname;
        inGamePrefabName = igpname;
        this.side = side;
    }

    public static byte[] SerializeClass(object roomRecord)
    {
        RoomRecord record = (RoomRecord)roomRecord;
        byte[] pnBytes = UTF8Encoding.Default.GetBytes(record.playerName);
        byte[] snBytes = UTF8Encoding.Default.GetBytes(record.displaySpriteName);
        byte[] igpnBytes = UTF8Encoding.Default.GetBytes(record.inGamePrefabName);
        byte[] bytes = new byte[4 + pnBytes.Length + 4 + snBytes.Length + 4 + igpnBytes.Length + 4 + 1];
        int index = 0;
        Protocol.Serialize(record.id, bytes, ref index);
        Protocol.Serialize(pnBytes.Length, bytes, ref index);
        pnBytes.CopyTo(bytes, index); index += pnBytes.Length;
        Protocol.Serialize(snBytes.Length, bytes, ref index);
        snBytes.CopyTo(bytes, index); index += snBytes.Length;
        Protocol.Serialize(igpnBytes.Length, bytes, ref index);
        igpnBytes.CopyTo(bytes, index); index += igpnBytes.Length;
        bytes[index] = (byte)record.side;

        return bytes;
    }

    public static object DeserializeClass(byte[] bytes)
    {
        RoomRecord record = new RoomRecord();
        int index = 0;
        Protocol.Deserialize(out record.id, bytes, ref index);
        int length = 0;
        Protocol.Deserialize(out length, bytes, ref index);
        record.playerName = UTF8Encoding.Default.GetString(bytes, index, length);
        index += length;
        Protocol.Deserialize(out length, bytes, ref index);
        record.displaySpriteName = UTF8Encoding.Default.GetString(bytes, index, length);
        index += length;
        Protocol.Deserialize(out length, bytes, ref index);
        record.inGamePrefabName = UTF8Encoding.Default.GetString(bytes, index, length);
        index += length;
        byte side = bytes[index];
        record.side = (PlayerSide)side;
        return record;
    }
}
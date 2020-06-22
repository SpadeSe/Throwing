/*
 * 用来记录当前房间下玩家的角色prefab选择以及阵容选择
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(PhotonView))]
public class RoomRecorder : MonoBehaviourPun, IPunObservable
{
    public bool clearRecord = false;
    public List<RoomRecord> redRecords;
    public List<RoomRecord> blueRecords;
    public int curIdx = 0; //从0开始不断增加, 不回退.(反正溢出的可能性几乎没有)

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = Definitions.roomRecorderTag;
        DontDestroyOnLoad(gameObject);
        PhotonPeer.RegisterType(typeof(RoomRecord), (byte)'r', RoomRecord.SerializeClass, RoomRecord.DeserializeClass);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int CallRegisterToRoom(string playername, string spriteprefabname, string ingameprefabname)
    {
        PhotonView view = PhotonView.Get(this);
        int toReturnId = curIdx;
        view.RPC("RegisterToRoom", RpcTarget.All, playername, spriteprefabname, ingameprefabname);
        return toReturnId;
        
    }

    //玩家初次进入房间的时候要注册进这个roomRecorder, 然后recoder会返回一个int来标识
    [PunRPC]
    public void RegisterToRoom(string playername, string spriteprefabname, string ingameprefabname)
    {
        Debug.Log(playername);
        RoomRecord newRecord = new RoomRecord
        (
            curIdx,
            playername,
            spriteprefabname,
            ingameprefabname,
            redRecords.Count <= blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE
        );
        if (newRecord.side == PlayerSide.RED)
        {
            redRecords.Add(newRecord);
        }
        else
        {
            blueRecords.Add(newRecord);
        }
        curIdx++;
    }

    //玩家离开房间的时候需要调用, 将记录移除
    public void LeaveRoom(int id, PlayerSide prevSide)
    {
        //if(!redRecords.Remove(redRecords.Find((r) => r.id == id)))
        //{
        //    blueRecords.Remove(blueRecords.Find((r) => r.id == id));
        //}
        if(prevSide == PlayerSide.RED)
        {
            redRecords.Remove(redRecords.Find((r) => r.id == id));
        }
        else
        {
            blueRecords.Remove(blueRecords.Find((r) => r.id == id));
        }
    }

    //public void ChangeName(int id, PlayerSide prevSide, string name)
    //{
    //    List<RoomRecord> changeRecord = redRecords;
    //    if(prevSide == PlayerSide.BLUE)
    //    {
    //        changeRecord = blueRecords;
    //    }
    //    for(int i = 0; i < changeRecord.Count; i++)
    //    {
    //        if(changeRecord[i].id == id)
    //        {
    //            changeRecord[i].name = name;
    //            break;
    //        }
    //    }
    //}

    public void ChangeSide(int id, PlayerSide prevSide, PlayerSide newSide)
    {
        if(prevSide == newSide)
        {
            return;
        }
        List<RoomRecord> changeRecord = redRecords;
        if (prevSide == PlayerSide.BLUE)
        {
            changeRecord = blueRecords;
        }
        for (int i = 0; i < changeRecord.Count; i++)
        {
            if (changeRecord[i].id == id)
            {
                changeRecord[i].side = newSide;
                break;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(redRecords.ToArray());
            stream.SendNext(blueRecords.ToArray());
            stream.SendNext(curIdx);
        }
        else
        {
            redRecords = new List<RoomRecord>((RoomRecord[])stream.ReceiveNext());
            blueRecords = new List<RoomRecord>((RoomRecord[])stream.ReceiveNext());
            curIdx = (int)stream.ReceiveNext();
        }
    }
}

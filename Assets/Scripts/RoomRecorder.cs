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

public delegate void GameBeginEvent();

[RequireComponent(typeof(PhotonView))]
public class RoomRecorder : MonoBehaviourPun, IPunObservable
{
    public PhotonView view;
    public GameBeginEvent gameBeginEvent;
    public PlayerController playerControllerLocal;

    public RoomState state = RoomState.Preparing;
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
        
        if(state == RoomState.ReadyToPlay)
        {
            //开始计时, 计分.
            Debug.Log("Start to play");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public KeyValuePair<int, PlayerSide> CallRegisterToRoom(PlayerController controller, string playername, string spriteprefabname, string ingameprefabname)
    {
        playerControllerLocal = controller;
        KeyValuePair<int, PlayerSide> result = new KeyValuePair<int, PlayerSide>(curIdx,
            (redRecords.Count <= blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE));
        int toReturnId = curIdx;
        view = PhotonView.Get(this);
        Debug.Log(redRecords.Count <= blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE);
        view.RPC("RegisterToRoom", RpcTarget.All, playername, spriteprefabname, ingameprefabname);
        return result;
    }

    //玩家初次进入房间的时候要注册进这个roomRecorder, 然后recoder会返回一个int来标识
    [PunRPC]
    public void RegisterToRoom(string playername, string spriteprefabname, string ingameprefabname)
    {
        //Debug.Log(playername);
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

    public bool CanStartGame()
    {
        return redRecords.Count == blueRecords.Count;
    }

    public void CallStartGame() {
        view = PhotonView.Get(this);
        view.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.IsVisible)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        PhotonNetwork.LoadLevel("TestPlayScene");
        StartCoroutine(WaitForSceneLoading());
        //while(PhotonNetwork.LevelLoadingProgress < 1f)
        //{
        //    //这里之后可以用来显示loading界面
        //    Debug.Log("<color=aqua>SceneLoading</color>");
        //}
        //state = RoomState.ReadyToPlay;
        //gameBeginEvent?.Invoke();
    }

    #region CallBacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(state);
            stream.SendNext(redRecords.ToArray());
            stream.SendNext(blueRecords.ToArray());
            stream.SendNext(curIdx);
        }
        else
        {
            state = (RoomState)stream.ReceiveNext();
            redRecords = new List<RoomRecord>((RoomRecord[])stream.ReceiveNext());
            blueRecords = new List<RoomRecord>((RoomRecord[])stream.ReceiveNext());
            curIdx = (int)stream.ReceiveNext();
        }
    }
    #endregion

    IEnumerator WaitForSceneLoading()
    {
        while(PhotonNetwork.LevelLoadingProgress < 1.0f)
        {
            yield return new WaitForSeconds(1.0f / PhotonNetwork.SendRate);
        }
        state = RoomState.ReadyToPlay;
        gameBeginEvent?.Invoke();
    }
}

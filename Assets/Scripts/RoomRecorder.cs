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

        Debug.Log("AutoSyncScene?:" + PhotonNetwork.AutomaticallySyncScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallRegisterToRoom(PlayerController controller, string playername, string spriteprefabname, string ingameprefabname)
    {
        playerControllerLocal = controller;
        //KeyValuePair<int, PlayerSide> result = new KeyValuePair<int, PlayerSide>(curIdx,
        //    (redRecords.Count <= blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE));
        //int toReturnId = curIdx;
        //Debug.Log(redRecords.Count <= blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE);
        photonView.RPC("RegisterToRoom", RpcTarget.AllViaServer, 
            PhotonNetwork.LocalPlayer.ActorNumber,
            playername, spriteprefabname, ingameprefabname);
        //return result;
    }

    /// <summary>
    /// 玩家初次进入房间的时候要注册进这个roomRecorder, 
    /// 然后recoder会给playercontroller分配id和side
    /// </summary>
    /// <param name="PlayerNumber"></param>
    /// <param name="playername"></param>
    /// <param name="spriteprefabname"></param>
    /// <param name="ingameprefabname"></param>
    [PunRPC]
    public void RegisterToRoom(int PlayerNumber, string playername, string spriteprefabname, string ingameprefabname)
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
        if(PlayerNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerControllerLocal.SetRoomInfo(newRecord.id, newRecord.side);
        }
        curIdx++;
    }


    /// <summary>
    /// 玩家离开房间的时候需要调用, 将记录移除
    /// TODO: 这应该使一个RPC, 不过还没做离开房间的功能所以暂时算了
    /// </summary>
    /// <param name="id"></param>
    /// <param name="prevSide"></param>
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
        photonView.RPC("StartGame", RpcTarget.All);
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
        StartCoroutine(WaitForBeingReadyToPlay());
        //while(PhotonNetwork.LevelLoadingProgress < 1f)
        //{
        //    //这里之后可以用来显示loading界面
        //    Debug.Log("<color=aqua>SceneLoading</color>");
        //}
        //state = RoomState.ReadyToPlay;
        //gameBeginEvent?.Invoke();
    }
    
    [PunRPC]
    public void SetSceneLoadedLocal(int idInRoom, PlayerSide side)
    {
        if(side == PlayerSide.RED)
        {
            foreach (var record in redRecords)
            {
                if (record.id == idInRoom)
                {
                    record.playSceneLoaded = true;
                    break;
                }
            }
        }
        else
        {
            foreach (var record in blueRecords)
            {
                if (record.id == idInRoom)
                {
                    record.playSceneLoaded = true;
                    break;
                }
            }
        }
    }

    public bool AllSceneLoaded()
    {
        bool allReady = true;
        foreach (var record in redRecords)
        {
            if (!record.playSceneLoaded)
            {
                allReady = false;
                break;
            }
        }
        if (allReady)
        {
            foreach (var record in blueRecords)
            {
                if (!record.playSceneLoaded)
                {
                    allReady = false;
                    break;
                }
            }
        }
        return allReady;
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
            RoomState receivedState = (RoomState)stream.ReceiveNext();
            if(receivedState > state)
            {
                state = receivedState;
            }
            redRecords = new List<RoomRecord>((RoomRecord[])stream.ReceiveNext());
            blueRecords = new List<RoomRecord>((RoomRecord[])stream.ReceiveNext());

            curIdx = (int)stream.ReceiveNext();
        }
    }
    #endregion

    #region Coroutines
    IEnumerator WaitForSceneLoading()
    {
        Debug.Log("<color=aqua>start waiting for sceneloading</color>");
        while(PhotonNetwork.LevelLoadingProgress < 1.0f)
        {
            yield return new WaitForSeconds(1.0f / PhotonNetwork.SendRate);
        }
        photonView.RPC("SetSceneLoadedLocal", RpcTarget.All, playerControllerLocal.idInRoom, playerControllerLocal.side);
    }
    
    IEnumerator WaitForBeingReadyToPlay()
    {
        while (!AllSceneLoaded())
        {
            yield return new WaitForSeconds(1.0f / PhotonNetwork.SendRate);
        }
        Debug.Log("<color=aqua>Ready To Play</color>");
        state = RoomState.ReadyToPlay;
        gameBeginEvent?.Invoke();
    }
    #endregion
}

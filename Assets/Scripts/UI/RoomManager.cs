/*
 * RoomManager
 * 囊括了显示房间列表, 显示房间内容, 阵营选择, 准备等各种功能的一个大类
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(CharacterSelect))]
public class RoomManager : MonoBehaviourPunCallbacks
{

    [Header("Hand Init")]
    public GameObject RoomScrollObj;//需要这个scroll有scrollRect和ToggleGroup
    public GameObject RoomPanel;

    [Header("Settings")]
    public GameObject RoomSelectTogglePrefab;
    public int maxPlayer = 3;


    [Header("For Debug")]
    public ScrollRect RoomScroll;
    public ToggleGroup RoomGroup;
    public PlayerController playerController;
    public Dictionary<string, RoomInfo> roomInfos;

    public RoomRecorder roomRecorder;
    public bool isManager;

    public void Init()
    {
        RoomScroll = RoomScrollObj.GetComponent<ScrollRect>();
        RoomGroup = RoomScrollObj.GetComponent<ToggleGroup>();
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag(Definitions.playerControllerTag).GetComponent<PlayerController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RectTransform contentTrans = RoomScroll.content;
        //TODO:
        foreach(RoomInfo room in roomList)
        {
            if(!room.IsVisible)// || room.RemovedFromList)
            {
                if (roomInfos.ContainsKey(room.Name))
                {
                    roomInfos.Remove(room.Name);
                }
            }
            else
            {
                roomInfos.Add(room.Name, room);
            }
        }
    }



    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //如果不是创建房间的人, 那么就找到roomRecorder, 然后注册进去.
        //如果是创建房间的人, 那么就创建一个新的roomRecorder
        roomRecorder = GameObject.Find("RoomRecorder").GetComponent<RoomRecorder>();
        if(roomRecorder == null)
        {
            roomRecorder = PhotonNetwork.Instantiate("RoomRecorder", Vector3.zero, Quaternion.Euler(Vector3.zero))
                .GetComponent<RoomRecorder>();
            isManager = true;
        }
        else
        {
            isManager = false;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public void ChangeSide(bool temp)
    {

    }
}

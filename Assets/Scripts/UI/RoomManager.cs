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
    public GameObject RoomPanel;
    public GameObject RoomScrollObj;//需要这个scroll有scrollRect和ToggleGroup
    public GameObject PlayerNameInputObj;
    public GameObject CreateRoomInputPanel;

    [Header("Settings")]
    public GameObject RoomSelectTogglePrefab;
    public int maxPlayer = 3;


    [Header("For Debug")]
    public InputField PlayerNameInput;
    public InputField RoomNameInput;
    public ScrollRect RoomScroll;
    public ToggleGroup RoomGroup;
    public PlayerController playerController;
    public Dictionary<string, RoomInfo> roomInfoDict;

    public RoomRecorder roomRecorder;
    public bool isManager;

    public void Init()
    {
        PlayerNameInput = PlayerNameInputObj.GetComponent<InputField>();
        RoomNameInput = CreateRoomInputPanel.GetComponentInChildren<InputField>();
        RoomScroll = RoomScrollObj.GetComponent<ScrollRect>();
        RoomGroup = RoomScrollObj.GetComponent<ToggleGroup>();
        roomInfoDict = new Dictionary<string, RoomInfo>();
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag(Definitions.playerControllerTag).GetComponent<PlayerController>();
        }
        PlayerNameInput.onEndEdit.AddListener(ChangeName);

        playerController.playerName = PlayerNameInput.text;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        CreateRoomInputPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, null);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("房间信息更新");
        base.OnRoomListUpdate(roomList);
        //update dict
        foreach (RoomInfo room in roomList)
        {
            if (!room.IsVisible)// || room.RemovedFromList)
            {
                if (roomInfoDict.ContainsKey(room.Name))
                {
                    roomInfoDict.Remove(room.Name);
                }
            }
            else
            {
                if (roomInfoDict.ContainsKey(room.Name))
                {
                    roomInfoDict[room.Name] = room;
                }
                else
                {
                    roomInfoDict.Add(room.Name, room);
                }
            }
        }
        //update display
        RectTransform contentTrans = RoomScroll.content;
        int curToggle = contentTrans.childCount;
        for (; curToggle < roomInfoDict.Count; curToggle++)
        {
            GameObject newToggle = Instantiate(RoomSelectTogglePrefab, contentTrans);
        }
        int idx = 0;
        foreach(var room in roomInfoDict)
        {
            Transform child = contentTrans.GetChild(idx);
            Text[] texts = child.GetComponentsInChildren<Text>();
            texts[0].text = "房间名: " + room.Value.Name;
            texts[1].text = "人属: " + room.Value.PlayerCount + " / " + (int)room.Value.MaxPlayers;
            idx++;
        }
        for(; idx < curToggle; idx++)
        {
            contentTrans.GetChild(idx).gameObject.SetActive(false);
        }
    }

    public void CreateRoomConfirm()
    {
        string roomname = RoomNameInput.text;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayer;
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        if (PhotonNetwork.CreateRoom(roomname, options))
        {
            PopupHint.PopupUI("成功创建房间", (RectTransform)transform);//.parent);
            //PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, null);

            CreateRoomInputPanel.SetActive(false);
        }
        else
        {
            PopupHint.PopupUI("创建房间失败", (RectTransform)transform);//.parent);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //如果不是创建房间的人, 那么就找到roomRecorder, 然后注册进去.
        //如果是创建房间的人, 那么就创建一个新的roomRecorder
        GameObject roomRecorderObj = GameObject.Find("RoomRecorder");
        if(roomRecorderObj == null)
        {
            roomRecorder = PhotonNetwork.Instantiate("RoomRecorder", Vector3.zero, Quaternion.Euler(Vector3.zero))
                .GetComponent<RoomRecorder>();
            isManager = true;
        }
        else
        {
            roomRecorder = roomRecorderObj.GetComponent<RoomRecorder>();
            isManager = false;
        }
        playerController.idInRoom = roomRecorder.RegisterToRoom(
            playerController.playerName, 
            playerController.selectCharacter.displaySprite.name, 
            playerController.selectCharacter.inGamePrefab.name);

        RoomPanel.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public void ChangeSide(bool temp)
    {

    }

    public void ChangeName(string newName)
    {

    }
}

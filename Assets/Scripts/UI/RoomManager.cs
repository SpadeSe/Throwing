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
    public GameObject PlayerNameInputObj;
    public GameObject CreateRoomInputPanel;
    public GameObject JoinRoomButton;
    public GameObject RoomPanel;
    public GameObject PlayButton;

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
    public string selectedRoomName = "";

    public RoomRecorder roomRecorder;
    public bool isManager = false;

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
        JoinRoomButton.GetComponent<Button>().onClick.RemoveAllListeners();
        JoinRoomButton.GetComponent<Button>().onClick.AddListener(JoinRoomButtonClicked);
        PlayButton.GetComponent<Button>().onClick.RemoveAllListeners();
        PlayButton.GetComponent<Button>().onClick.AddListener(PlayButtonClicked);

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

    
    
    
    

    public void ChangeSide(bool temp)
    {

    }

    public void ChangeName(string newName)
    {
        playerController.playerName = newName;
    }

    #region callbacks
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
            if (!room.IsVisible || !room.IsOpen || room.RemovedFromList)
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
            RoomScroll.GetComponent<ToggleGroup>().RegisterToggle(newToggle.GetComponent<Toggle>());
        }
        int idx = 0;
        foreach (var room in roomInfoDict)
        {
            Transform child = contentTrans.GetChild(idx);
            Text[] texts = child.GetComponentsInChildren<Text>();
            texts[0].text = "房间名: " + room.Value.Name;
            texts[1].text = "人属: " + room.Value.PlayerCount + " / " + (int)room.Value.MaxPlayers;
            idx++;
        }
        for (; idx < curToggle; idx++)
        {
            contentTrans.GetChild(idx).gameObject.SetActive(false);
            contentTrans.GetChild(idx).GetComponent<Toggle>().isOn = false;
        }
        if(roomInfoDict.Count > 0)
        {
            contentTrans.GetChild(0).GetComponent<Toggle>().isOn = true;
            var iter = roomInfoDict.Values.GetEnumerator();
            if(iter.Current == null)
            {
                iter.MoveNext();
            }
            selectedRoomName = iter.Current.Name;
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        roomRecorder = PhotonNetwork.Instantiate("RoomRecorder", Vector3.zero, Quaternion.Euler(Vector3.zero))
                .GetComponent<RoomRecorder>();
        isManager = true;
        //playerController.idInRoom = roomRecorder.CallRegisterToRoom(
        //    playerController.playerName,
        //    playerController.selectCharacter.displaySprite.name,
        //    playerController.selectCharacter.inGamePrefab.name);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //如果不是创建房间的人, 那么就找到roomRecorder, 然后注册进去.
        //如果是创建房间的人, 那么就创建一个新的roomRecorder
        StartCoroutine(FindRoomRecorderAndRegister());
        if (!isManager)
        {
            //GameObject roomRecorderObj = GameObject.Find("RoomRecorder");
            //if (roomRecorderObj == null)
            //{
            //    roomRecorder = PhotonNetwork.Instantiate("RoomRecorder", Vector3.zero, Quaternion.Euler(Vector3.zero))
            //        .GetComponent<RoomRecorder>();
            //}
            //else
            //{
            //    roomRecorder = roomRecorderObj.GetComponent<RoomRecorder>();
            //}
            //playerController.idInRoom = roomRecorder.CallRegisterToRoom(
            //    playerController.playerName,
            //    playerController.selectCharacter.displaySprite.name,
            //    playerController.selectCharacter.inGamePrefab.name);
        }
        RoomPanel.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }
    #endregion

    #region delegate events

    public void CreateRoomConfirm()
    {
        string roomname = RoomNameInput.text;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayer;
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
            PopupHint.PopupUI("创建房间失败: 未加入大厅");
            return;
        }
        if (PhotonNetwork.CreateRoom(roomname, options))
        {
            PopupHint.PopupUI("成功创建房间");//.parent);
            //PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, null);

            CreateRoomInputPanel.SetActive(false);
        }
        else
        {
            PopupHint.PopupUI("创建房间失败");//.parent);
        }
    }

    public void ChooseRoom(bool on)
    {
        if (on)
        {
            RectTransform contentTrans = RoomScroll.content;
            int idx = 0;
            foreach(var room in roomInfoDict)
            {
                if (contentTrans.GetChild(idx).GetComponent<Toggle>().isOn)
                {
                    selectedRoomName = room.Key;
                    break;
                }
                idx++;
            }
        }
    }

    public void JoinRoomButtonClicked()
    {
        Debug.Log("JoinRoomButtonClicked");
        if(selectedRoomName == "")
        {
            PopupHint.PopupUI("未选择房间, 无法加入");
            return;
        }
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PopupHint.PopupUI("加入房间失败");
            return;
        }
        if (PhotonNetwork.JoinRoom(selectedRoomName))
        {
            PopupHint.PopupUI("成功加入房间");

        }
        else
        {
            PopupHint.PopupUI("加入房间失败");
        }
    }

    public void PlayButtonClicked()
    {
        if (!roomRecorder.CanStartGame())
        {
            PopupHint.PopupUI("人数不均, 不能开始游戏");
            return;
        }

        roomRecorder.CallStartGame();
    }

    #endregion

    IEnumerator FindRoomRecorderAndRegister()
    {
        GameObject roomRecorderObj = GameObject.FindGameObjectWithTag(Definitions.roomRecorderTag);
        while (roomRecorderObj == null)
        {
            yield return new WaitForSeconds(1.0f / PhotonNetwork.SendRate);
            roomRecorderObj = GameObject.FindGameObjectWithTag(Definitions.roomRecorderTag);
        }
        roomRecorder = roomRecorderObj.GetComponent<RoomRecorder>();
        yield return new WaitForSeconds(2.0f / PhotonNetwork.SendRate);
        Debug.Log(roomRecorder.curIdx);
        Debug.Log(roomRecorder.redRecords.Count <= roomRecorder.blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE);
        playerController.ConnectToRoom(roomRecorder);
    }
}

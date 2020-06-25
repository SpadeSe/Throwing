using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public RoomRecorder roomRecorder;
    public int idInRoom = -1;

    public string playerName;
    public CharacterSelectInfo selectCharacter;
    public GameObject inGameCharacterObj;
    public PlayerCharacter controllingCharacter;

    public PlayerSide side;
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.tag = Definitions.playerControllerTag;
    }
    
    void Start()
    {

    }

    private void FixedUpdate()
    {
        if(controllingCharacter != null 
            && controllingCharacter.liveState == CharacterLiveState.Alive)
        {
            if (controllingCharacter.hasWeapon() && Input.GetMouseButton(1))
            {
                controllingCharacter.targeting = true;
            }
            else
            {
                if (controllingCharacter.targeting == true && controllingCharacter.hasWeapon())
                {
                    controllingCharacter.throwing = true;
                }
                controllingCharacter.targeting = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (PhotonNetwork.IsConnected)
                {
                    controllingCharacter.CallDealWithFocusingObj();
                }
                else
                {
                    controllingCharacter.DealWithFocusingObj();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConnectToRoom(RoomRecorder recoder)
    {
        //PhotonNetwork.LocalPlayer.ActorNumber
        roomRecorder = recoder;
        roomRecorder.gameBeginEvent += GameBegin;
        //Debug.Log(roomRecorder.curIdx);
        //Debug.Log(roomRecorder.redRecords.Count <= roomRecorder.blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE);
        roomRecorder.CallRegisterToRoom(
            this,
            playerName,
            selectCharacter.displaySprite.name,
            selectCharacter.inGamePrefab.name);
    }

    public void SetRoomInfo(int idInRoom, PlayerSide side)
    {
        Debug.Log("RoomInfoSetup: " + idInRoom + " " + side);
        this.idInRoom = idInRoom;
        this.side = side;
    }


    public void GameBegin()
    {
        PlayerStart playerStarts = GameObject.FindWithTag(Definitions.playerStartTag).GetComponent<PlayerStart>();
        playerStarts.CallGetStartTrans(this);
        //Transform startTrans = playerStarts.getStartTrans(side);
        //if (roomRecorder != null && roomRecorder.state == RoomState.ReadyToPlay)
        //{
        //    inGameCharacterObj = Instantiate(selectCharacter.inGamePrefab);
        //    controllingCharacter = inGameCharacterObj.GetComponentInChildren<PlayerCharacter>();
        //    controllingCharacter.respawnTrans = startTrans;
        //    controllingCharacter.Respawn();
        //}
    }

    public void InitCharacter(Transform startTrans)
    {
        Debug.Log("<color=red>RoomState: " + roomRecorder.state + "</color>");
        if (roomRecorder != null && roomRecorder.state == RoomState.ReadyToPlay)
        {
            Debug.Log("<color=aqua>id: " + idInRoom + " InitCharacter</color>");
            inGameCharacterObj = PhotonNetwork.Instantiate(Definitions.inGameCharacterPrefabResourcePath + 
                selectCharacter.inGamePrefab.name,
                startTrans.position, startTrans.rotation);
            controllingCharacter = inGameCharacterObj.GetComponentInChildren<PlayerCharacter>();
            controllingCharacter.respawnTrans = startTrans;
            controllingCharacter.Respawn();
        }
    }

    IEnumerator waitForCharacterStart()
    {
        while(controllingCharacter == null)
        {
            yield return new WaitForSeconds(1.0f / PhotonNetwork.SendRate);
        }
        //TODO: 允许游戏进行之类的emmm
        PopupHint.PopupUI("角色已生成");
    }
}

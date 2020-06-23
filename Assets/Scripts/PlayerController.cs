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
        var result = roomRecorder.CallRegisterToRoom(
            this,
            playerName,
            selectCharacter.displaySprite.name,
            selectCharacter.inGamePrefab.name);
        idInRoom = result.Key;
        side = result.Value;
    }

    public void InitCharacter(Transform startTrans)
    {
        if (roomRecorder != null && roomRecorder.state == RoomState.ReadyToPlay)
        {
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

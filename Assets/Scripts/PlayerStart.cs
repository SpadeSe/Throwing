using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//这个也得同步
[RequireComponent(typeof(PhotonView))]
public class PlayerStart : MonoBehaviour, IPunObservable
{
    public List<Transform> redStarts;
    public int redCount = 0;
    public List<Transform> blueStarts;
    List<bool> BlueStartsTaken;
    public int blueCount = 0;
    public PlayerController playerControllerLocal;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = Definitions.playerStartTag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallGetStartTrans(PlayerController controller)
    {
        playerControllerLocal = controller;
        PhotonView view = PhotonView.Get(this);
        Debug.Log("LocalNumber:" + PhotonNetwork.LocalPlayer.ActorNumber);
        view.RPC("GetStartTrans", RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber, playerControllerLocal.side);
    }

    [PunRPC]
    public void GetStartTrans(int playerNumber, PlayerSide side)
    {
        Debug.Log("PlayerNumber: " + playerNumber);
        //TODO: 检测这里的溢出等各种问题
        if(side == PlayerSide.RED)
        {
            if(playerNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerControllerLocal.InitCharacter(redStarts[redCount]);
            }
            redCount++;
        }
        else
        {
            if (playerNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerControllerLocal.InitCharacter(blueStarts[blueCount]);
            }
            blueCount++;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(redCount);
            //stream.SendNext(blueCount);
        }
        else
        {
            //redCount = (int)stream.ReceiveNext();
            //blueCount = (int)stream.ReceiveNext();
        }
    }
}

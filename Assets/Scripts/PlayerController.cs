using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public int idInRoom;

    public string playerName;
    public CharacterSelectInfo selectCharacter;
    public PlayerSide side;
    // Start is called before the first frame update
    private void Awake()
    {
        gameObject.tag = Definitions.playerControllerTag;

    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

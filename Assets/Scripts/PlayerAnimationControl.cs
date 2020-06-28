/*
 * 
 * 放在骨骼上面来控制动画
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;



[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerCharacter))]
public class PlayerAnimationControl : MonoBehaviourPun
{
    public Animator animControl;
    public PlayerCharacter playerCharacter;
    public FirstPersonAIO movement;
    public float HorizontalInput = 0.0f;
    public float VerticalInput = 0.0f;
    public float turnInput = 0.0f;
    //public bool targeting = false;
    //public bool throwing = false;
    //public int attackType = 0;
    
    

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        animControl = GetComponent<Animator>();
        playerCharacter = GetComponent<PlayerCharacter>();
        movement = playerCharacter.moveControl;
    }
    private void FixedUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }
        if(playerCharacter.liveState != CharacterLiveState.Alive)//靶子也得处理死亡动画
        {
            switch (playerCharacter.liveState)
            {
                case CharacterLiveState.Dying_Forward:
                    animControl.SetTrigger("DyingForward");
                    break;
                case CharacterLiveState.Dying_Backward:
                    animControl.SetTrigger("DyingBackward");
                    break;
                case CharacterLiveState.Dying_Left:
                    animControl.SetTrigger("DyingLeft");
                    break;
                case CharacterLiveState.Dying_Right:
                    animControl.SetTrigger("DyingRight");
                    break;
                case CharacterLiveState.Respawn:
                    animControl.SetTrigger("Respawn");
                    playerCharacter.liveState = CharacterLiveState.Alive;
                    break;
                default:break;
            }
            if(playerCharacter.liveState != CharacterLiveState.Alive)
            {
                playerCharacter.liveState = CharacterLiveState.Dead;
            }
            return;
        }
        if (playerCharacter.isStaticTarget)
        {
            return;
        }
        //state
        animControl.SetBool("Armed", playerCharacter.hasWeapon());
        //Move
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        animControl.SetFloat("Horizontal", HorizontalInput);
        animControl.SetFloat("Vertical", VerticalInput);
        if (Mathf.Abs(HorizontalInput) > 0 || Mathf.Abs(VerticalInput) > 0)
        {
            animControl.SetBool("Moving", true);
        }
        else
        {
            animControl.SetBool("Moving", false);
        }
        animControl.SetFloat("MoveSpeed", movement.isSprinting ? movement.sprintSpeed / movement.walkSpeed : 1.0f);
        //Throw
        animControl.SetInteger("AttackType", playerCharacter.attackType);
        animControl.SetBool("Throwing", playerCharacter.throwing);
        animControl.SetBool("Targeting", playerCharacter.targeting);
        //if (playerControl.hasWeapon() &&)
        //{
        //    targeting = true;
        //}
        //else
        //{
        //    if (targeting && playerControl.hasWeapon())
        //    {
        //        throwing = true;
        //        animControl.SetBool("Throwing", true);
        //    }
        //    targeting = false;
        //}
    }

    #region 动画事件
    public void StartThrowing()
    {

    }

    public void EndThrowing()
    {
        //Debug.Log("<color=blue>throwing End</color>");
        animControl.SetBool("Throwing", false);
        playerCharacter.throwing = false;
    }

    public void ThrowOut()
    {
        //Debug.Log("<color=blue>throw out</color>");
        if (PhotonNetwork.IsConnected)
        {
            playerCharacter.CallThrow();
        }
        else
        {
            playerCharacter.Throw();
        }
    }

    public void DeadAnimEnd()
    {
        playerCharacter.DeadEnd();
    }
    #endregion
}

/*
 * 
 * 放在骨骼上面来控制动画
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class PlayerAnimationControl : MonoBehaviour
{
    public Animator animControl;
    public Player playerControl;
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
        playerControl = GetComponent<Player>();
        movement = playerControl.moveControl;
    }
    private void FixedUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (playerControl.isStaticTarget)
        {
            return;
        }
        //state
        animControl.SetBool("Armed", playerControl.hasWeapon());
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
        animControl.SetInteger("AttackType", playerControl.attackType);
        animControl.SetBool("Throwing", playerControl.throwing);
        animControl.SetBool("Targeting", playerControl.targeting);
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
        playerControl.throwing = false;
    }

    public void ThrowOut()
    {
        //Debug.Log("<color=blue>throw out</color>");
        playerControl.Throw();
    }
    #endregion
}

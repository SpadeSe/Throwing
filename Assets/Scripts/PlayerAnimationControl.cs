/*
 * 
 * 放在骨骼上面来控制动画
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Animator))]
public class PlayerAnimationControl : MonoBehaviour
{
    public FirstPersonAIO movement;
    public Animator animControl;
    public Player playerControl;
    public float HorizontalInput = 0.0f;
    public float VerticalInput = 0.0f;
    public bool targeting = false;
    public bool throwing = false;
    public int attackType = 0;
    

    // Start is called before the first frame update
    private void Awake()
    {
        animControl = GetComponent<Animator>();
        playerControl = GetComponent<Player>();
    }

    void Start()
    {
       
    }
    private void FixedUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
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
        animControl.SetFloat("MoveSpeed", movement.speed);
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

    public void StartThrowing()
    {

    }

    public void EndThrowing()
    {
        Debug.Log("<color=blue>throwing End</color>");
        animControl.SetBool("Throwing", false);
        playerControl.throwing = false;
    }

    public void ThrowOut()
    {
        Debug.Log("<color=blue>throw out</color>");
        playerControl.Throw();
    }
}

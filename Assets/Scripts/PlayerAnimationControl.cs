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
        //state
        animControl.SetBool("Armed", playerControl.hasWeapon());
        //Move
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        animControl.SetFloat("Horizontal", HorizontalInput);
        animControl.SetFloat("Vertical", VerticalInput);
        if(Mathf.Abs(HorizontalInput) > 0 || Mathf.Abs(VerticalInput) > 0)
        {
            animControl.SetBool("Moving", true);
        }
        else
        {
            animControl.SetBool("Moving", false);
        }
        animControl.SetFloat("MoveSpeed", movement.speed);
        //Throw
        animControl.SetInteger("AttackType", attackType);
        if (playerControl.hasWeapon() && Input.GetMouseButton(1))
        {
            targeting = true;
            animControl.SetBool("Targeting", true);
        }
        else
        {
            if (targeting && playerControl.hasWeapon())
            {
                throwing = true;
                animControl.SetBool("Throwing", true);
            }
            targeting = false;
            animControl.SetBool("Targeting", false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartThrowing()
    {

    }

    public void EndThrowing()
    {
        Debug.Log("throwing set false");
        animControl.SetBool("Throwing", false);
        throwing = false;
    }

    public void ThrowOut()
    {
        playerControl.Throw();
    }
}

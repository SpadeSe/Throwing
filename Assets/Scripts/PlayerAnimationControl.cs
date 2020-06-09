using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    public FirstPersonAIO movement;
    public Animator animControl;
    public float HorizontalInput = 0.0f;
    public float VerticalInput = 0.0f;
    public bool targetTing = false;
    public bool throwing = false;
    public int attackType = 0;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    private void FixedUpdate()
    {
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
        if (Input.GetMouseButton(1))
        {
            targetTing = true;
            animControl.SetBool("Targeting", true);
        }
        else
        {
            if (targetTing)
            {
                throwing = true;
                animControl.SetBool("Throwing", true);
            }
            targetTing = false;
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
}

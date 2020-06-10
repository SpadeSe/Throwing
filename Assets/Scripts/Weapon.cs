using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{
    public Animator animControl;
    //public Rigidbody rigid;
    [Header("Moving")]
    public bool moving = false;
    public float gravityScale = 0.05f;
    public float StartSpeed = 1.5f;
    public Vector3 debugPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        animControl = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animControl.SetBool("Throwing", moving);
        if (moving)
        {
            GetComponent<Rigidbody>().AddForce(Physics.gravity * gravityScale);
            Debug.Log("<color=blue>Velocity * time:" + GetComponent<Rigidbody>().velocity * Time.deltaTime + "</color>");
            Debug.Log("<color=blue>Delta Pos:" + (transform.position - debugPos) + "</color>");
            debugPos = transform.position;
        }
        else
        {
            GetComponent<Rigidbody>().useGravity = false;

        }
    }

    public void ThrowOut(Vector3 dir)
    {
        if (moving)
        {
            Debug.LogError("Already thrown out");
            return;
        }
        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.AddForce(StartSpeed * dir, ForceMode.Impulse);
        Debug.Log("<color=aqua>Force: " + (StartSpeed * dir) + "</color>");
        rigid.useGravity = false;
        //rigid.interpolation = RigidbodyInterpolation.None;
        moving = true;
        debugPos = transform.position;
    }
}

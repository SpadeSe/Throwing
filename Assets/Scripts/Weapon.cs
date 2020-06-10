using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{

    [Header("Moving")]
    public bool moving = false;
    public float gravityScale = 0.05f;
    public float StartSpeed = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            GetComponent<Rigidbody>().AddForce(Physics.gravity * gravityScale);
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
        rigid.AddForce(StartSpeed * dir, ForceMode.Impulse);
        rigid.useGravity = false;
        
        moving = true;
    }
}

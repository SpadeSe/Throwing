using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{
    public Animator animControl;
    //public Rigidbody rigid;
    [Header("High Light")]
    public Material highlightMat;
    public Color highlightColor = Color.yellow;
    public GameObject highlightPrefab;
    public GameObject highlightObj;
    [Header("State")]
    public int type = 0;
    public bool taken = false;
    public bool focused = false;
    [Header("Moving")]
    public bool moving = false;
    public float gravityScale = 0.05f;
    public float StartSpeed = 1.5f;
    Vector3 debugPos = Vector3.zero;
    [Header("RotateAdjust")]
    public GameObject WeaponHead;
    // Start is called before the first frame update
    void Start()
    {
        animControl = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //deal with focus
        if (!taken)
        {
            DealWithFocus();
        }
        //animation
        if(animControl.runtimeAnimatorController != null)
        {
            animControl.SetBool("Throwing", moving);
        }
        //movement
        if (moving)
        {
            GetComponent<Rigidbody>().AddForce(Physics.gravity * gravityScale);
            //Debug.Log("<color=blue>Velocity * time:" + GetComponent<Rigidbody>().velocity * Time.deltaTime + "</color>");
            //Debug.Log("<color=blue>Delta Pos:" + (transform.position - debugPos) + "</color>");
            debugPos = transform.position;
            AdjustRotation(GetComponent<Rigidbody>().velocity);
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
        if(WeaponHead != null)
        {
            AdjustRotation(dir);
        }
        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.AddForce(StartSpeed * dir, ForceMode.Impulse);
        //Debug.Log("<color=aqua>Force: " + (StartSpeed * dir) + "</color>");
        rigid.useGravity = false;
        //rigid.interpolation = RigidbodyInterpolation.None;
        moving = true;
        debugPos = transform.position;
    }

    void AdjustRotation(Vector3 dir)
    {
        if(WeaponHead == null)
        {
            Debug.LogError("Not Spear");
            return;
        }
        Vector3 weaponHeadDir = WeaponHead.transform.position - transform.position;
        Vector3 axis = Vector3.Cross(dir, weaponHeadDir);
        transform.Rotate(axis, -Vector3.Angle(weaponHeadDir, dir), Space.World);
    }

    public void DrawDebug(Vector3 dir)
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, transform.right, Color.green);
        Debug.DrawRay(transform.position, transform.up, Color.blue);
        if(WeaponHead != null)
        {
            Vector3 weaponHeadDir = WeaponHead.transform.position - transform.position;
            Debug.DrawRay(transform.position, 3 * weaponHeadDir, Color.magenta);
            Vector3 axis = Vector3.Cross(dir, weaponHeadDir);
            Debug.DrawRay(transform.position, 3 * axis, Color.cyan);
        }
    }

    private void DealWithFocus()
    {
        
        if (focused)
        {
            if(highlightMat != null && highlightObj == null)
            {
                highlightObj = Instantiate<GameObject>(highlightPrefab, transform);
                Collider[] colliders = highlightObj.GetComponentsInChildren<Collider>();
                foreach(var collider in colliders)
                {
                    collider.enabled = false;
                }
                Renderer[] renderers = highlightObj.GetComponentsInChildren<Renderer>();
                foreach(var renderer in renderers)
                {
                    //TODO: 这里可能有问题
                    renderer.material = highlightMat;
                    renderer.material.SetColor("g_vOutlineColor", highlightColor);
                }
            }
        }
        else
        {
            Destroy(highlightObj);
            highlightObj = null;
        } 
    }
}

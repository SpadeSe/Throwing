using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Surface : Focusable
{
    [Header("Broken")]
    public Collider brokenCollider;
    public bool canBeDestroyed = false;
    public bool destroyed = false;
    // Start is called before the first frame update
    void Start()
    {
        if (canBeDestroyed)
        {
            Transform childBlock = transform.GetChild(0);
            brokenCollider = childBlock.GetComponent<Collider>();
            childBlock.gameObject.layer = LayerMask.NameToLayer("PlayerBlock");
            if (destroyed)
            {
                Broken();
            }
            else
            {
                Fixed();
            }
            focusPrefab = gameObject;
        }
        else
        {
            focusable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Broken()
    {
        GetComponent<Collider>().isTrigger = true;//让武器可以掉下去, 并且不触发函数
        brokenCollider.enabled = true;
        GetComponent<Renderer>().enabled = false;
        destroyed = true;
        focusable = true;
    }

    public void Fixed()
    {
        GetComponent<Collider>().isTrigger = false;
        brokenCollider.enabled = false;
        GetComponent<Renderer>().enabled = true;
        destroyed = false;
        focusable = false;
    }



    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.transform.parent == null)
        //{
        //    return;
        //}
        //Debug.Log(collision.gameObject.name);
        Weapon weapon = collision.transform.GetComponent<Weapon>();
        if (weapon != null)
        {
            //Debug.Log(weapon.gameObject.name);
            if (!weapon.isBomb)
            {
                //TODO: 调整武器位置和角度
                weapon.AdjustPosAndRotToSurface(collision);
                weapon.ForceStop();
                if (weapon.canDestroy) {
                    weapon.DestroySurface(this);
                }
                weapon.MinusUseCount();
                weapon.ClearState();
            }
            else
            {

            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Weapon weapon = collision.transform.GetComponent<Weapon>();
        if(weapon != null)
        {
            if (weapon.isBomb)
            {
                weapon.BombBounce(collision);
            }
        }
    }
}

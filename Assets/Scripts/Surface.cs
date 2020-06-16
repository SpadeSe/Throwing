using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Surface : Focusable
{
    //public List<Weapon> weaponList;
    [Header("Broken")]
    public Collider brokenCollider;
    public bool canBeDestroyed = false;
    public bool destroyed = false;
    public float fixTime = 3.0f;
    Coroutine fixRoutine;
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
        if(fixRoutine != null)
        {
            if (!focused)
            {
                StopCoroutine(fixRoutine);
                fixRoutine = null;
                Debug.Log("Fix stalled");
                //TODO: 提示修复失败

            }
        }
    }

    public void Broken()
    {
        GetComponent<Collider>().isTrigger = true;//让武器可以掉下去, 并且不触发函数
        brokenCollider.enabled = true;
        GetComponent<Renderer>().enabled = false;
        destroyed = true;
        focusable = true;
        Debug.Log(gameObject.name + " is broken");
        //TODO: 提示被破坏
    }

    public void Fixed()
    {
        GetComponent<Collider>().isTrigger = false;
        brokenCollider.enabled = false;
        GetComponent<Renderer>().enabled = true;
        destroyed = false;
        focusable = false;
        Debug.Log(gameObject.name + " has been fixed");
        //TODO: 提示已修复
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
            //weaponList.Add(weapon);
            //Debug.Log(weapon.gameObject.name);
            if (!weapon.isBomb)
            {
                weapon.MinusUseCount();
                //TODO: 调整武器位置和角度
                if (weapon.canDestroy && this.canBeDestroyed)
                {
                    weapon.DestroySurface(this);
                }
                else
                {
                    weapon.AdjustPosAndRotToSurface(collision);
                    weapon.ForceStop();
                    weapon.ClearState();
                }
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
            //weaponList.Remove(weapon);
            if (weapon.isBomb)
            {
                weapon.BombBounce(collision);
            }
        }
    }

    public void StartFixing()
    {
        fixRoutine = StartCoroutine(Fixing(fixTime));
    }

    IEnumerator Fixing(float duration)
    {
        yield return new WaitForSeconds(duration);
        Fixed();
    }
}

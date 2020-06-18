using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Surface : Focusable
{
    public List<Weapon> weaponList;
    [Header("Broken")]
    public bool canBeDestroyed = false;
    public Collider brokenCollider;
    public bool destroyed = false;
    public bool fixing = false;
    public float fixTime = 3.0f;
    public float fixProgress = 0.0f;
    public GameObject fixingUI;
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
        GetComponent<Collider>().isTrigger = true;//让砸破它的武器可以掉下去, 并且不触发函数
        brokenCollider.enabled = true;
        GetComponent<Renderer>().enabled = false;
        destroyed = true;
        focusable = true;
        //暂时隐藏上面的武器
        foreach (Weapon weapon in weaponList)
        {
            weapon.gameObject.SetActive(false);
        }
        //Debug.Log(gameObject.name + " is broken");
        //TODO: 提示被破坏
    }

    public void Fixed()
    {
        fixing = false;
        GetComponent<Collider>().isTrigger = false;//恢复原来功能
        brokenCollider.enabled = false;
        GetComponent<Renderer>().enabled = true;
        destroyed = false;
        focusable = false;
        //重新显示上面的武器
        foreach (Weapon weapon in weaponList)
        {
            if(weapon.bornSurface == this)
            {
                weapon.ReGen();
            }
            weapon.gameObject.SetActive(true);
        }
        //Debug.Log(gameObject.name + " has been fixed");
        //if(fixingUI != null)
        //{
        //    fixingUI.SetActive(false);
        //}
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
                if (weapon.canDestroy && canBeDestroyed)
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

    private void OnTriggerEnter(Collider other)
    {
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if(weapon != null)
        {
            AddWeaponToList(weapon);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if (weapon != null)
        {
            weaponList.Remove(weapon);
        }
    }

    public void AddWeaponToList(Weapon weapon)
    {
        if(!weaponList.Find((w)=>weapon == w))
        {
            weaponList.Add(weapon);
        }
    }

    public void StartFixing()//GameObject fUI = null)
    {
        //fixingUI = fUI;
        fixRoutine = StartCoroutine(Fixing());
        fixing = true;
        //if (fixingUI != null)
        //{
        //    fixingUI.SetActive(true);
        //    fixingUI.GetComponent<Scrollbar>().size = 0;
        //}
        fixProgress = 0.0f;
    }

    IEnumerator Fixing()
    {
        while(fixProgress < fixTime)
        {
            fixProgress += Time.deltaTime;
            //if (fixingUI != null)
            //{
            //    //fixingUI.SetActive(true);
            //    fixingUI.GetComponent<Scrollbar>().size = fixProgress / fixTime;
            //}
            yield return null;
        }
        //yield return new WaitForSeconds(duration);
        Fixed();
    }
}

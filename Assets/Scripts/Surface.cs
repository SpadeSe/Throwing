using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Surface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.transform.parent == null)
        //{
        //    return;
        //}
        Debug.Log(collision.gameObject.name);
        Weapon weapon = collision.transform.GetComponent<Weapon>();
        if (weapon != null)
        {
            Debug.Log(weapon.gameObject.name);
            if (!weapon.isBomb)
            {
                //TODO: 调整武器位置和角度
                weapon.AdjustPosAndRotToSurface(collision);
                weapon.ForceStop();
                weapon.WeaponCollider.isTrigger = true;
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

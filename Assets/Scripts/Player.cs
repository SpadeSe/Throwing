﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HittenEvent();
public delegate void DeadEvent();

public class Player : MonoBehaviour
{
    [Header("Must Init")]
    public Transform weaponSlot;
    public Camera playerCam;

    [Header("debug")]
    public bool targeting = false;
    public bool throwing = false;
    public int attackType = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    private void FixedUpdate()
    {
        if(hasWeapon() && Input.GetMouseButton(1))
        {
            targeting = true;
        }
        else
        {
            if(targeting == true && hasWeapon())
            {
                throwing = true;
            }
            targeting = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targeting)
        {
            DrawLine();
        }
    }

    public void DrawLine()
    {
        if (hasWeapon())
        {
            Transform weapon = weaponSlot.GetChild(0);
            Debug.DrawRay(playerCam.transform.position, 10 * playerCam.transform.forward, Color.blue);
            Debug.DrawRay(weapon.position, weapon.forward, Color.blue);
            Vector3 start = weapon.position;
            Vector3 dir = playerCam.transform.forward;
            float speed = weapon.GetComponent<Weapon>().StartSpeed;
            float gravity = weapon.GetComponent<Weapon>().gravityScale;
            float time = 3;
            for (float i = 0.0f; i < time; i += Time.deltaTime)
            {
                Vector3 end = start + dir * speed * Time.deltaTime;
                Debug.DrawLine(start, end, Color.yellow);
                dir += Physics.gravity * weapon.GetComponent<Weapon>().gravityScale * Time.deltaTime;
                start = end;
            }
        }
    }

    public void Throw()
    {
        if (!hasWeapon())
        {
            Debug.Log("<color=red>No Weapon</color>");
            return;
        }
        Transform weapon = weaponSlot.GetChild(0);
        GameObject copy = GameObject.Instantiate<GameObject>(weapon.gameObject, weaponSlot.transform);
        copy.transform.parent = null;
        //weaponSlot.DetachChildren();
        copy.GetComponent<Weapon>().ThrowOut(playerCam.transform.forward);
    }

    public bool hasWeapon()
    {
        if(weaponSlot == null)
        {
            return false;
        }
        return weaponSlot.childCount > 0;
    }
}

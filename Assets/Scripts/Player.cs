using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HittenEvent();
public delegate void DeadEvent();

public class Player : MonoBehaviour
{
    [Header("Must Init")]
    public Transform weaponSlot;
    public Camera playerCam;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Throw()
    {
        if (!hasWeapon())
        {
            Debug.Log("<color=red>No Weapon</color>");
            return;
        }
        Transform weapon = weaponSlot.GetChild(0);
        //weaponSlot.DetachChildren();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TempWeapon : ItemBase
{
    public GameObject weaponObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeEffect(Weapon weapon)
    {
        base.TakeEffect(weapon);
        GameObject tempWeapon = null;
        //if (!PhotonNetwork.IsConnected)
        //{
        //    //weaponObj.GetComponent<PhotonView>().enabled = false;
        tempWeapon = Instantiate(weaponObj);
        //}
        //else
        //{
        //}//
        //tempWeapon = PhotonNetwork.Instantiate(Definitions.TempWeaponResourcePath + weaponObj.name, Vector3.zero, Quaternion.identity);
        weapon.owner.TakeWeapon(tempWeapon);
    }
}

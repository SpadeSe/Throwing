using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GameObject tempWeapon = Instantiate<GameObject>(weaponObj);
        weapon.owner.TakeWeapon(tempWeapon);
    }
}

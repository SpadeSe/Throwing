using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : ItemBase
{
    public int healPoint = 3;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeEffect(Weapon weapon)
    {
        base.TakeEffect(weapon);
        if(weapon.owner != null)
        {
            weapon.owner.ReceiveHeal(healPoint);
        }
    }
}

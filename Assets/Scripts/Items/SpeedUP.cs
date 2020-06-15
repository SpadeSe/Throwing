using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUP : ItemBase
{
    public float upRate = 1.0f;
    public float duration = 5.0f;
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
            weapon.owner.MakeSpeedUp(upRate, duration);
        }
    }
}

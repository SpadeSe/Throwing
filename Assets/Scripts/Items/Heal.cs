using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Heal : MonoBehaviour
{
    int healPoint = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if (weapon != null)
        {
            if(weapon.owner != null)
            {
                weapon.owner.ReceiveHeal(healPoint);
            }
        }
    }
}

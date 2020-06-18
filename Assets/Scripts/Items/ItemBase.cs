using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemBase : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void Init()
    {
        GetComponent<Collider>().isTrigger = true;
        Debug.Log(gameObject.name);
    }

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TakeEffect(Weapon weapon)
    {
        Debug.Log(weapon.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if(weapon != null)
        {
            if (weapon.moving == false || weapon.owner == null)
            {
                return;
            }
            TakeEffect(weapon);
        }
    }
}

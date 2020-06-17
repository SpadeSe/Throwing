using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    public GameObject EnterParticle;
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
        if(EnterParticle != null)
        {
            GameObject particle = Instantiate(EnterParticle, transform);
            particle.transform.position = other.transform.position;
        }
        //处理武器
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if (weapon != null)
        {
            weapon.ReGen();
        }
    }
}

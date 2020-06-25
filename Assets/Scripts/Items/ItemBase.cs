using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemBase : MonoBehaviour
{
    public bool haveTakenEffect = false;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public GameObject hitParticle;
    // Start is called before the first frame update
    public virtual void Init()
    {
        GetComponent<Collider>().isTrigger = true;
        Debug.Log(gameObject.name);
        audioSource = GetComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }
    }

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO: 这后期可能要改成RPC调用
    public virtual void TakeEffect(Weapon weapon)
    {
        Debug.Log(weapon.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!haveTakenEffect)
        {
            Debug.Log(other.gameObject.name);
            Weapon weapon = other.GetComponentInParent<Weapon>();
            if (weapon != null)
            {
                if (weapon.moving == false || weapon.owner == null)
                {
                    return;
                }
                float destroyDelay = 0f;
                if(audioSource != null && hitSound != null)
                {
                    audioSource.clip = hitSound;
                    destroyDelay = Mathf.Max(hitSound.length, destroyDelay);
                    audioSource.Play();
                }
                if(hitParticle != null)
                {
                    GameObject particle = Instantiate(hitParticle);
                    particle.transform.position = transform.position;
                    particle.GetComponent<ParticleSystem>().Play();
                    destroyDelay = Mathf.Max(particle.GetComponent<ParticleSystem>().main.duration, destroyDelay);
                    Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
                }
                TakeEffect(weapon);
                haveTakenEffect = true;

                Destroy(gameObject, destroyDelay);
            }
        }
    }
}

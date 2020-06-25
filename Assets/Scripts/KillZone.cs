using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    public GameObject EnterParticle;
    [Header("Audio")]
    public AudioSource audioSource;
    [Tooltip("进入水面时的音效")]
    public AudioClip WeaponEnterClip;
    public AudioClip PlayerEnterClip;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
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
            if (WeaponEnterClip != null)
            {
                audioSource.clip = WeaponEnterClip;
                audioSource.Play();
            }
            weapon.ReGen();
            return;
        }

        PlayerCharacter player = other.GetComponentInChildren<PlayerCharacter>();
        if (player != null)
        {
            if (PlayerEnterClip != null)
            {
                audioSource.clip = PlayerEnterClip;
                audioSource.Play();
            }
            player.Suicide();
        }
    }
}

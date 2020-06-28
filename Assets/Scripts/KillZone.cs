using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    public GameObject EnterParticle;
    public List<Weapon> weaponList;
    public List<PlayerCharacter> playerList;
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
        //处理武器
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if (weapon != null && weapon.moving)
            //&& !weaponList.Find(w => w == weapon))
        {
            //Debug.Log("WeaponEnter");
            GenParticle(weapon.transform);
            //weaponList.Add(weapon);
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
            //&& !playerList.Find(f => f == player ))
        {
            //Debug.Log("playerEnter");
            GenParticle(player.transform);
            //playerList.Add(player);
            if (PlayerEnterClip != null)
            {
                audioSource.clip = PlayerEnterClip;
                audioSource.Play();
            }
            player.Suicide();
        }
    }

    public void GenParticle(Transform other)
    {
        if (EnterParticle != null)
        {
            GameObject particle = Instantiate(EnterParticle);//, transform);
            particle.transform.position = other.position;
            Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
        }
    }
    
}

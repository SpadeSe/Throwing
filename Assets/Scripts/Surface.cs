using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Surface : Focusable
{
    public List<Weapon> weaponList;
    [Header("Broken")]
    public bool canBeDestroyed = false;
    public Collider brokenCollider;
    public bool destroyed = false;
    public bool fixing = false;
    public PlayerCharacter fixingPlayer;
    public float fixTime = 3.0f;
    public float fixProgress = 0.0f;
    public GameObject fixingUI;
    Coroutine fixRoutine;
    [Header("Audio")]
    public AudioSource audioSource;
    [Tooltip("投掷类武器击中的音效")]
    public AudioClip weaponHitSound;
    [Tooltip("炸弹反弹的音效")]
    public AudioClip bombBounceSound;
    [Tooltip("被砸碎的音效音效")]
    public AudioClip BrokenSound;
    [Tooltip("修复中音效")]
    public AudioClip FixingSound;
    [Tooltip("修复时的音效")]
    public AudioClip fixedSound;
    [Header("Particle")]
    public GameObject brokenParticlePrefab;
    public GameObject fixingParticlePrefab;
    public GameObject fixingParticle;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
        if (canBeDestroyed)
        {
            Transform childBlock = transform.GetChild(0);
            brokenCollider = childBlock.GetComponent<Collider>();
            childBlock.gameObject.layer = LayerMask.NameToLayer("PlayerBlock");
            if (destroyed)
            {
                Broken();
            }
            else
            {
                ToggleBrokenOrFixedState(false);
            }
            focusPrefab = gameObject;
        }
        else
        {
            focusable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(fixRoutine != null)
        {
            if (!focused)
            {
                fixing = false;
                StopCoroutine(fixRoutine);
                fixRoutine = null;
                Debug.Log("Fix stalled");
                PopupHint.PopupUI("修复失败");
                fixingPlayer = null;
                if(fixingParticle != null)
                {
                    fixingParticle.GetComponent<ParticleSystem>().Stop();
                }
                if (audioSource.isPlaying)
                {
                    audioSource.loop = false;
                    audioSource.Stop();
                }
                //TODO: 提示修复失败, 这里架构可能还要改

            }
        }
    }

    public void Broken()
    {
        ToggleBrokenOrFixedState(true);
        //暂时隐藏上面的武器
        foreach (Weapon weapon in weaponList)
        {
            weapon.gameObject.SetActive(false);
        }
        if(BrokenSound != null)
        {
            audioSource.clip = BrokenSound;
            audioSource.loop = false;
            audioSource.Play();
        }
        if(brokenParticlePrefab != null)
        {
            GameObject brokenParticle = Instantiate(brokenParticlePrefab, transform);
            brokenParticle.GetComponent<ParticleSystem>().Play();
            Destroy(brokenParticle, brokenParticle.GetComponent<ParticleSystem>().main.duration);
        }
        //Debug.Log(gameObject.name + " is broken");
        //TODO: 提示被破坏
    }

    public void ToggleBrokenOrFixedState(bool broken)
    {
        GetComponent<Collider>().isTrigger = broken;//让砸破它的武器可以掉下去, 并且不触发函数
        brokenCollider.enabled = broken;
        GetComponent<Renderer>().enabled = !broken;
        destroyed = broken;
        focusable = broken;
    }

    public void Fixed()
    {
        fixProgress = 0.0f;
        fixing = false;
        ToggleBrokenOrFixedState(false);
        if(fixingPlayer != null)
        {
            fixingPlayer.fixingSurface = null;
        }
        fixingPlayer = null;
        if (fixedSound != null)
        {
            audioSource.clip = fixedSound;
            audioSource.Play();
            audioSource.loop = false;
        }
        if (fixingParticle != null)
        {
            Destroy(fixingParticle);
            fixingParticle = null;
        }
        //重新显示上面的武器
        foreach (Weapon weapon in weaponList)
        {
            if(weapon.bornSurface == this)
            {
                weapon.ReGen();
            }
            weapon.gameObject.SetActive(true);
        }
        fixRoutine = null;
        PopupHint.PopupUI("修复成功");
    }



    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.transform.parent == null)
        //{
        //    return;
        //}
        //Debug.Log(collision.gameObject.name);
        Weapon weapon = collision.transform.GetComponent<Weapon>();
        if (weapon != null)
        {
            //weaponList.Add(weapon);
            //Debug.Log(weapon.gameObject.name);
            if (!weapon.isBomb)
            {
                weapon.MinusUseCount();
                //TODO: 调整武器位置和角度
                if (weapon.canDestroy && canBeDestroyed)
                {
                    weapon.DestroySurface(this, collision);
                }
                else
                {
                    if(weaponHitSound != null)
                    {
                        audioSource.clip = weaponHitSound;
                        audioSource.loop = false;
                        audioSource.Play();
                    }
                    weapon.AdjustPosAndRotToSurface(collision);
                    weapon.ForceStop();
                    if (weapon.canTransfer)//如果可以传送, 辣么就要传送
                    {
                        weapon.Transfer();
                    }
                    weapon.ClearState();
                }
            }
            else
            {

            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Weapon weapon = collision.transform.GetComponent<Weapon>();
        if(weapon != null)
        {
            //weaponList.Remove(weapon);
            if (weapon.isBomb)
            {
                weapon.BombBounce(collision);
                if(bombBounceSound != null)
                {
                    audioSource.clip = bombBounceSound;
                    audioSource.Play();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if(weapon != null)
        {
            AddWeaponToList(weapon);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Weapon weapon = other.GetComponentInParent<Weapon>();
        if (weapon != null)
        {
            weaponList.Remove(weapon);
        }
    }

    public void AddWeaponToList(Weapon weapon)
    {
        if(!weaponList.Find((w)=>weapon == w))
        {
            weaponList.Add(weapon);
        }
    }

    public void StartFixing(PlayerCharacter player)//GameObject fUI = null)
    {
        fixingPlayer = player;
        //fixingUI = fUI;
        fixRoutine = StartCoroutine(Fixing());
        fixing = true;
        //if (fixingUI != null)
        //{
        //    fixingUI.SetActive(true);
        //    fixingUI.GetComponent<Scrollbar>().size = 0;
        //}
        //fixProgress = 0.0f;
        if(FixingSound != null)
        {
            audioSource.clip = FixingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        if(fixingParticle == null)
        {
            if(fixingParticlePrefab != null)
            {
                fixingParticle = Instantiate(fixingParticlePrefab, transform);
                fixingParticle.GetComponent<ParticleSystem>().Play();
            }
        }
        else
        {
            fixingParticle.GetComponent<ParticleSystem>().Play();
        }
    }

    IEnumerator Fixing()
    {
        while(fixProgress < fixTime)
        {
            fixProgress += Time.deltaTime;
            //if (fixingUI != null)
            //{
            //    //fixingUI.SetActive(true);
            //    fixingUI.GetComponent<Scrollbar>().size = fixProgress / fixTime;
            //}
            yield return null;
        }
        //yield return new WaitForSeconds(duration);
        Fixed();
    }
}

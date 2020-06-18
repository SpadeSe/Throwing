/*
 * Weapon
 * 制作一个weapon的方法
 * 在角色的weaponslot下挂一个空物体作为武器的父物体, transform都是初始状态
 * 挂上weapon脚本
 * rigidbody 重力和drag都不要, iskinematic勾上.
 * animator上一个合适的
 * 下挂武器, 加上weaponhead
 * 给weapon脚本设置里面各种东西. 完成!
 * 
 * 注意: 用collisionEnter的时候不用在parent找weapon, 而triggerEnter需要从parent找weapon
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Weapon : Focusable
{
    [HideInInspector]
    public Animator animControl;
    //public Rigidbody rigid;
    //[Header("High Light")]
    //public Material highlightMat;
    //public Color highlightColor = Color.yellow;
    //public GameObject highlightPrefab;
    //public GameObject highlightObj;
    [Header("Display")]
    public string weaponName;
    public Sprite UISprite;
    [Header("State")]
    public int damage = 2;
    public int useCount = -1;//临时武器的使用计数, 如果小于0表示非临时武器
    public int type = 0;
    public bool taken = false;
    //public bool focused = false;
    public Player owner = null;
    public bool canDestroy = false;
    public Surface bornSurface;
    //public Surface hitSurface = null;
    [Header("Moving")]
    public Collider WeaponCollider;
    public bool moving = false;
    public float gravityScale = 0.05f;
    public float StartSpeed = 1.5f;
    Vector3 debugPos = Vector3.zero;
    [Header("TransAdjust")]
    public GameObject WeaponHead;
    public Vector3 startPos;
    public Quaternion startRotate;
    [Header("Bomb")]
    public bool isBomb = false;
    public SphereCollider burstRange;
    public List<Player> burstAffectPlayers;
    public GameObject burstHintObj;
    public GameObject burstParticlePrefab;
    public Coroutine burstRoutine;
    public float burstCountDown = 3.0f;
    public int maxBounce = 2;
    int bounceCount = 2;
    // Start is called before the first frame update
    void Start()
    {
        focusUIHint = "按<size=27><color=yellow>E</color></size>拾取";

        animControl = GetComponent<Animator>();

        AdjustPosAndRotToSurface();
        startPos = transform.position;
        startRotate = transform.rotation;
        //ForceStop();
        if (isBomb)
        {
            bounceCount = maxBounce;
            if(burstRange != null)
            {
                burstRange.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //deal with focus
        DealWithFocus();
        //animation
        if(animControl.runtimeAnimatorController != null)
        {
            animControl.SetBool("Throwing", moving);
        }
        //movement
        if (moving)
        {
            //加重力
            GetComponent<Rigidbody>().AddForce(Physics.gravity * gravityScale);
            //Debug.Log("<color=blue>Velocity * time:" + GetComponent<Rigidbody>().velocity * Time.deltaTime + "</color>");
            //Debug.Log("<color=blue>Delta Pos:" + (transform.position - debugPos) + "</color>");
            debugPos = transform.position;
            //调整指向
            if(WeaponHead != null)
            {
                AdjustRotation(GetComponent<Rigidbody>().velocity);
            }
        }
        else
        {
            //GetComponent<Rigidbody>().useGravity = false;
            ForceStop();
        }
        //if (!taken)
        //{
        //    Debug.Log(GetComponent<Rigidbody>().angularVelocity);
        //}
    }

    //武器被扔出前的各种处理
    public void ThrowOut(Vector3 dir)
    {
        if (moving)
        {
            Debug.LogError("Already thrown out");
            return;
        }
        if(WeaponHead != null)
        {
            AdjustRotation(dir);
        }
        Rigidbody rigid = GetComponent<Rigidbody>();
        //启用力
        rigid.isKinematic = false;
        //给初速度
        rigid.AddForce(StartSpeed * dir, ForceMode.Impulse);
        //Debug.Log("<color=aqua>Force: " + (StartSpeed * dir) + "</color>");
        rigid.useGravity = false;
        debugPos = transform.position;
        //转为碰撞体
        WeaponCollider.isTrigger = false;
        //改变状态
        taken = false;
        moving = true;
        if (isBomb)
        {
            bounceCount = maxBounce;
        }
    }
    //调整武器的指向
    void AdjustRotation(Vector3 dir)
    {
        if(WeaponHead == null)
        {
            Debug.LogError("No WeaponHead");
            return;
        }
        Vector3 weaponHeadDir = WeaponHead.transform.position - transform.position;
        Vector3 axis = Vector3.Cross(dir, weaponHeadDir);
        transform.Rotate(axis, -Vector3.Angle(weaponHeadDir, dir), Space.World);
    }

    //绘制debug线条
    public void DrawDebug(Vector3 dir)
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, transform.right, Color.green);
        Debug.DrawRay(transform.position, transform.up, Color.blue);
        if(WeaponHead != null)
        {
            Vector3 weaponHeadDir = WeaponHead.transform.position - transform.position;
            Debug.DrawRay(transform.position, 3 * weaponHeadDir, Color.magenta);
            Vector3 axis = Vector3.Cross(dir, weaponHeadDir);
            Debug.DrawRay(transform.position, 3 * axis, Color.cyan);
        }
    }
    

    //简易的判断weapon是否能take的函数
    public bool CanTake()
    {
        //return !(taken || moving);
        if (!isBomb)
        {
            return !(taken || moving);
        }
        else
        {
            return !(taken || moving) && (bounceCount == maxBounce);
        }
    }

    public void Taken(Player player)
    {
        focusable = false;
        owner = player;
        //Fixme: 这里会有点儿问题;
        transform.SetParent(player.weaponSlot.transform);
        transform.SetAsFirstSibling();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
        taken = true;
        GetComponent<Rigidbody>().isKinematic = true;
        if (isBomb)
        {
            if(burstHintObj != null)
            {
                burstHintObj.SetActive(false);
            }
        }
    }

    public void Drop(Transform target=null)
    {
        if(target == null)
        {
            //TODO: 直接丢弃时的办法
            return;
        }
        ClearState();
        //RaycastHit hit = new RaycastHit();
        //Physics.Raycast(target.position, new Vector3(0, -1, 0), out hit, 100.0f);
        //Vector3 distance = Vector3.zero;
        //if (target.GetComponent<Weapon>().WeaponHead != null && hit.collider.GetComponent<Surface>())
        //{
        //    distance = hit.point - target.position;
        //}
        //TODO: 修改这里的位置变化方式
        transform.parent = null;
        Vector3 newPos = target.transform.position;
        if(WeaponHead != null && target.GetComponent<Weapon>().WeaponHead != null)
        {
            newPos.y += (transform.position.y - WeaponHead.transform.position.y) -
                (target.position.y - target.GetComponent<Weapon>().WeaponHead.transform.position.y);
        }
        transform.position = newPos;
        transform.rotation = target.transform.rotation;
        transform.localScale = target.transform.localScale;
        taken = false;
    }

    //强制停止
    public void ForceStop()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        moving = false;
    }
    //public void HitSurface(Collision collision)
    //{

    //}

    //处理炸弹反弹时的状态变化
    public void BombBounce(Collision collision)
    {
        if(bounceCount == 0)
        {
            return;
        }
        bounceCount--;
        if(bounceCount == 0)
        {
            Debug.Log("<color=yellow>Bursting</color>");
            if (burstHintObj != null)
            {
                burstHintObj.SetActive(true);
                //TODO: adjust rotation
                AdjustPosAndRotToSurface(collision);
            }
            ForceStop();
            if (burstRoutine == null)//撞到停下再炸(否则hintobj的显示就有点微妙
            {
                burstRoutine = StartCoroutine(burstDelay());
            }
            if(burstRange != null)
            {
                burstAffectPlayers.Clear();
                burstRange.enabled = true;
            }
        }
    }

    IEnumerator burstDelay()
    {
        yield return new WaitForSeconds(burstCountDown);
        Burst();
    }
    //处理炸弹爆炸后的状态变化
    public void Burst()
    {
        Debug.Log("<color=red>Burst</color>");
        if(burstHintObj != null)
        {
            burstHintObj.SetActive(false);
        }
        //爆炸粒子
        if(burstParticlePrefab != null)
        {
            GameObject particle = Instantiate<GameObject>(burstParticlePrefab);
            particle.GetComponent<ParticleSystem>().Play();
            Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
        }
        //TODO: 造成伤害, 处理力, 各种各种
        foreach(var player in burstAffectPlayers)
        {
            Debug.Log(player.gameObject.name + "is hurt by burst");
        }
        //reset状态
        MinusUseCount();
        ClearState();
    }

    public void ClearState()
    {
        owner = null;
        WeaponCollider.isTrigger = true;
        if (isBomb)
        {
            bounceCount = maxBounce;
            burstRoutine = null;
            if (burstRange != null)
            {
                burstRange.enabled = false;
            }
        }
        focusable = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        //只处理爆炸的触发器
        if(burstRange == null || !burstRange.enabled)
        {
            return;
        }
        //Debug.Log(other.gameObject.name);
        if (other.GetComponentInChildren<Player>() != null)
        {
            if (burstAffectPlayers.Find(
                delegate(Player player){ return player == other.GetComponentInChildren<Player>(); }) 
                == null)
            {
                burstAffectPlayers.Add(other.GetComponentInChildren<Player>());
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //只处理爆炸的触发器
        if (burstRange == null || !burstRange.enabled)
        {
            return;
        }
        if (other.GetComponentInChildren<Player>() != null)
        {
            burstAffectPlayers.Remove(other.GetComponentInChildren<Player>());
        }
    }

    //用来在砸地和开始的时候调整位置到Surface上
    public void AdjustPosAndRotToSurface(Collision collision = null)
    {
        if (taken)
        {
            return;
        }
        if(collision == null)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 dir = WeaponHead == null ? new Vector3(0, -1, 0) : (WeaponHead.transform.position - transform.position).normalized;
            if(Physics.Raycast(transform.position, dir, out hit, 100))
            {
                Vector3 posDiffer = hit.point - transform.position + 
                    (WeaponHead == null ? Vector3.zero : transform.position - WeaponHead.transform.position);
                //Debug.Log(hit.point);
                //Debug.Log(gameObject.name + " " + hit.collider.gameObject.name + (hit.point - WeaponHead.transform.position));
                transform.Translate(posDiffer, Space.World);
                //初始表面就已经出问题的时候
                Surface hitSurface = hit.collider.GetComponent<Surface>();
                if (hitSurface != null)
                {
                    bornSurface = hitSurface;
                    bornSurface.AddWeaponToList(this);
                    if (bornSurface.destroyed)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
            return;
        }
        if (isBomb)//如果是炸弹的话那就躺平
        {
            ContactPoint cp = collision.GetContact(0);
            AdjustRotation(-cp.normal);
        }
        else//否则要沿原来方向插在surface上面
        {
            Vector3 posDiffer = collision.GetContact(0).point - transform.position +
                (WeaponHead == null ? Vector3.zero : transform.position - WeaponHead.transform.position);
            transform.Translate(posDiffer, Space.World);
        }
    }

    //简易的判断是否是临时武器的函数
    public bool IsTempWeapon()
    {
        return useCount > 0;
    }

    //减少临时武器的使用次数
    public void MinusUseCount()
    {
        if(!IsTempWeapon())
        {
            return;
        }
        useCount--;
        if(useCount == 0)
        {
            //TODO: play sound, particle, etc
            Destroy(gameObject);//这里后面需要加上延迟等一等音效和粒子
        }
    }


    //重新生成(丢到不可拾取区域的时候
    public void ReGen()
    {
        if (IsTempWeapon())
        {
            return;
        }
        ClearState();
        ForceStop();
        transform.position = startPos;
        transform.rotation = startRotate;
        if(bornSurface != null)
        {
            if (bornSurface.destroyed)
            {
                bornSurface.AddWeaponToList(this);
                gameObject.SetActive(false);
            }
        }
    }
    
    public void DestroySurface(Surface surface)
    {
        if (!canDestroy)
        {
            return;
        }
        if (surface.canBeDestroyed)
        {
            surface.Broken();
            canDestroy = false;//避免一砸砸一片
            
        }
    }
}

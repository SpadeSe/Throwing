/*
 * Player
 * 放在FirstPerson AIO的下层, 角色的mesh+骨骼的父级gameobject上
 * 需要拽好MustInit下的部分.(以及UI)
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public delegate void PlayerHittenEvent();
public delegate void PlayerDeadEvent(Player dead, Player killer);

public class Player : MonoBehaviour
{
    public bool isStaticTarget = false;
    [Header("Must Init")]
    public Transform weaponSlot;
    public Camera playerCam;
    public FirstPersonAIO moveControl;
    public Transform respawnTrans;
    [Header("UI")]
    public GameObject CanvasPrefab;
    public GameObject ownedCanvas;
    //public GameObject crosshair;
    //public GameObject hintUI;
    //public GameObject fixingBar;

    [Header("Data")]
    public int maxHp = 3;
    public float normalSpeed = 2.0f;
    public float runSpeed = 4.0f;

    [Header("State")]
    public PlayerSide side;
    public int curHp = 1;
    public float speedRate = 1.0f;

    [Header("Throw")]
    public bool targeting = false;
    public bool throwing = false;
    public int attackType = 0;
    public GameObject linePrefab;
    [HideInInspector]
    public GameObject lineObj;
    public Color predictLineColor = Color.yellow;
    public float LinePredictTime = 3.0f;
    [Range(3, 100)]
    public int LineSlice = 50;

    [Header("Interact")]
    public float interactDis = 1.0f;
    public GameObject focusingObj;
    public Surface fixingSurface;
    Coroutine speedUpState;

    //Delegates
    public PlayerDeadEvent deadEvent;
    
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.unityLogger.logEnabled = true;

        //init
        curHp = maxHp;

        if (isStaticTarget)//靶子分界线
        {
            return;            
        }
        if (ownedCanvas == null)
        {
            ownedCanvas = Instantiate(CanvasPrefab);
            ownedCanvas.GetComponent<PlayerCanvas>().player = this;
        }
    }

    private void FixedUpdate()
    {
        if (isStaticTarget)
        {
            return;
        }
        #region setState
        moveControl.walkSpeed = normalSpeed * speedRate;
        moveControl.walkSpeedInternal = moveControl.walkSpeed;
        moveControl.sprintSpeed = runSpeed * speedRate;
        moveControl.walkSpeedInternal = moveControl.sprintSpeed;
        if (hasWeapon())
        {
            attackType = weaponSlot.transform.GetChild(0).GetComponent<Weapon>().type;
        }
        if(hasWeapon() && Input.GetMouseButton(1))
        {
            targeting = true;
        }
        else
        {
            if(targeting == true && hasWeapon())
            {
                throwing = true;
            }
            targeting = false;
        }
        #endregion
        if (Input.GetKeyDown(KeyCode.E))
        {
            DealWithFocusingObj();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStaticTarget)
        {
            return;
        }
        //if(crosshair != null)
        //{
        //    crosshair.SetActive(!targeting);
        //}
        //if(fixingBar != null)
        //{
        //    fixingBar.SetActive(fixingSurface != null && fixingSurface.fixing);
        //}
        if (targeting)
        {
            UpdateLine();
        }
        else
        {
            DisableLine();

            #region Detect Interactable(Weapon Or Can be fixed Deck)
            RaycastHit hit = new RaycastHit();
            Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * interactDis, Color.blue);
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, 
                out hit, interactDis))//, ~LayerMask.NameToLayer("PlayerBlock")))
            {
                if(hit.collider.transform.parent != null 
                    && hit.collider.transform.parent.GetComponent<Focusable>() != null
                    && hit.collider.transform.parent.GetComponent<Focusable>().focusable)
                {
                    if (focusingObj != null)
                    {
                        focusingObj.GetComponent<Focusable>().focused = false;
                    }
                    focusingObj = hit.collider.transform.parent.gameObject;
                    Focusable focusable = focusingObj.GetComponent<Focusable>();
                    focusable.focused = true;
                    //if(hintUI != null)
                    //{
                    //    focusable.ShowUI(hintUI);
                    //}
                }
                else
                {
                    //if (hintUI != null)
                    //{
                    //    hintUI.SetActive(false);
                    //}
                }//deprecated
            }
            else
            {
                if(focusingObj != null)
                {
                    focusingObj.GetComponent<Focusable>().focused = false;
                    focusingObj = null;
                }
                //if (hintUI != null)
                //{
                //    hintUI.SetActive(false);
                //}
            }
            #endregion
        }

    }

    //用来更新瞄准时的参考线的绘制
    public void UpdateLine()
    {
        if (hasWeapon())
        {
            Transform weapon = weaponSlot.GetChild(0);
            #region DrawDebugLine
            if (Debug.unityLogger.logEnabled)
            {
                Debug.DrawRay(playerCam.transform.position, interactDis * playerCam.transform.forward, Color.blue);
                weapon.GetComponent<Weapon>().DrawDebug(playerCam.transform.forward);
                Vector3 start = weapon.position;
                Vector3 speed = weapon.GetComponent<Weapon>().StartSpeed * playerCam.transform.forward;
                float timeStep = LinePredictTime / LineSlice;
                for (float i = 0.0f; i < LinePredictTime; i += timeStep)
                {
                    Vector3 end = start + speed * timeStep;
                    Debug.DrawLine(start, end, predictLineColor);
                    speed += Physics.gravity * weapon.GetComponent<Weapon>().gravityScale * timeStep;
                    start = end;
                }
            }
            #endregion

            #region DrawLine
            if(linePrefab != null)
            {
                if(lineObj == null)
                {
                    lineObj = Instantiate<GameObject>(linePrefab);
                }
                else
                {
                    lineObj.SetActive(true);
                }
                lineObj.transform.position = Vector3.zero;
                lineObj.transform.rotation = Quaternion.Euler(Vector3.zero);
                VolumetricLineStripBehavior stripe = lineObj.GetComponent<VolumetricLineStripBehavior>();
                if(stripe != null)
                {
                    List<Vector3> vertices = new List<Vector3>();
                    Vector3 cur = weapon.position;
                    Vector3 end = cur;
                    Vector3 speed = weapon.GetComponent<Weapon>().StartSpeed * playerCam.transform.forward;
                    float gravityScale = weapon.GetComponent<Weapon>().gravityScale;
                    float timeStep = LinePredictTime / LineSlice;
                    for(float i = 0.0f; i < LinePredictTime; i += timeStep)
                    {
                        vertices.Add(cur);
                        end = cur + speed * timeStep;
                        #region RayTest: if hit any target or surface

                        #endregion
                        speed += Physics.gravity * gravityScale * timeStep;
                        cur = end;
                    }
                    vertices.Add(end);
                    stripe.UpdateLineVertices(vertices.ToArray());
                    stripe.LineColor = predictLineColor;
                }
            }
            #endregion

        }
    }
    //用来禁用参考线
    public void DisableLine()
    {
        if(lineObj != null)
        {
            lineObj.SetActive(false);
        }
    }

    public void DealWithFocusingObj()
    {
        if(focusingObj == null)
        {
            return;
        }
        if (focusingObj.GetComponent<Weapon>() != null)
        {
            TakeWeapon(focusingObj);
        }
        else if (focusingObj.GetComponent<Surface>() != null)
        {
            fixingSurface = focusingObj.GetComponent<Surface>();
            fixingSurface.StartFixing();
        }
    }

    //拾取武器时的处理函数
    public void TakeWeapon(GameObject takeWeapon)
    {
        if(takeWeapon == null)
        {
            return;
        }
        if (hasWeapon())
        {
            Transform curWeapon = weaponSlot.transform.GetChild(0);
            //TODO: 处理takeWeapon是临时武器的状况(或者干脆让临时武器只对空手有效)
            if(takeWeapon.GetComponent<Weapon>().IsTempWeapon())
            {

            }
            else
            {
                curWeapon.GetComponent<Weapon>().Drop(takeWeapon.transform);
            }
        }
        takeWeapon.GetComponent<Weapon>().Taken(this);
    }

    //投掷武器时的处理函数
    public void Throw()
    {
        if (!hasWeapon())
        {
            Debug.Log("<color=red>No Weapon</color>");
            return;
        }
        Transform weapon = weaponSlot.GetChild(0);
        weaponSlot.DetachChildren();
        weapon.GetComponent<Weapon>().ThrowOut(playerCam.transform.forward);
        //GameObject copy = GameObject.Instantiate<GameObject>(weapon.gameObject, weaponSlot.transform);
        //copy.transform.parent = null;
        //copy.GetComponent<Weapon>().ThrowOut(playerCam.transform.forward);
    }

    //简单的用来判断是否拿了武器的函数
    public bool hasWeapon()
    {
        if(weaponSlot == null)
        {
            return false;
        }
        return weaponSlot.childCount > 0;
    }

    public void ReceiveDamage(Player resource, int dam = 0)
    {
        Debug.Log(gameObject.name + "ReceiveDamage: " + dam);
    }

    public void ReceiveHeal(int healPoint = 0)
    {
        Debug.Log(gameObject.name + "ReceiveHeal: " + healPoint);
    }

    public void MakeSpeedUp(float upRate = 0.0f, float duration = 3.0f)
    {
        if(speedUpState != null)
        {
            StopCoroutine(speedUpState);
        }
        speedUpState = StartCoroutine(speedUp(upRate, duration));
    }

    IEnumerator speedUp(float upRate, float duration)
    {
        speedRate = 1.0f + upRate;
        yield return new WaitForSeconds(duration);
        speedRate = 1.0f;
    }

    public void Killed(Player killer=null)
    {

        deadEvent(this, killer);
        //TODO: 计分, 灰屏, 等待时间之类


        Respawn();
    }

    public void Respawn()
    {
        moveControl.transform.position = respawnTrans.position;
        moveControl.transform.rotation = respawnTrans.rotation;
    }
}

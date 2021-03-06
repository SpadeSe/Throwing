﻿/*
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
using Photon.Pun;
using Photon.Realtime;

public delegate void PlayerHittenEvent();
public delegate void PlayerDeadEvent(PlayerCharacter dead, PlayerCharacter killer);

public class PlayerCharacter : MonoBehaviourPun, IPunObservable
{
    public bool isStaticTarget = false;
    [Header("Hand Init")]
    public Transform weaponSlot;
    public Camera playerCam;
    public FirstPersonAIO moveControl;
    [Header("Script Init")]
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
    public CharacterLiveState liveState = CharacterLiveState.Alive;
    public int curHp = 1;
    public float speedRate = 1.0f;
    public int kill = 0;
    public int dead = 0;
    public int score = 0;

    [Header("Throw")]
    public bool targeting = false;
    public bool throwing = false;
    public int attackType = 0;
    public HintLine hintLine;
    //public GameObject linePrefab;
    //[HideInInspector]
    //public GameObject lineObj;
    //public Color predictLineColor = Color.yellow;
    //public float LinePredictTime = 3.0f;
    //[Range(3, 100)]
    //public int LineSlice = 50;

    [Header("Interact")]
    public float interactDis = 1.0f;
    public GameObject focusingObj;
    public Surface fixingSurface;
    Coroutine speedUpState;

    //Delegates
    public PlayerDeadEvent deadEvent;

    #region MainLoopFlow
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.unityLogger.logEnabled = true;

        //init
        curHp = maxHp;

        if (isStaticTarget)//靶子分界线
        {
            if(moveControl != null)
            {
                moveControl.enableCameraMovement = false;
                moveControl.playerCanMove = false;
            }
            if(playerCam != null)
            {
                playerCam.gameObject.SetActive(false);
            }
            return;//靶子角色设置不能移动, 摄像机也无效. 然后退出            
        }
        //以上是靶子角色, 下面是非靶子角色
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            if (ownedCanvas == null && CanvasPrefab != null)
            {
                ownedCanvas = Instantiate(CanvasPrefab);
                ownedCanvas.GetComponent<PlayerInGameCanvas>().player = this;
            }
            hintLine = GetComponent<HintLine>();
        }
        else
        {
            if (moveControl != null)
            {
                //moveControl.enableCameraMovement = false;
                moveControl.playerCanMove = false;
            }
            if (playerCam != null)
            {
                playerCam.GetComponent<AudioListener>().enabled = false;
                playerCam.enabled = false;

            }
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
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (isStaticTarget)
        {
            return;
        }
        if (targeting)
        {
            if(hintLine != null)
            {
                hintLine.UpdateLine(weaponSlot.GetChild(0), playerCam.transform.forward);
            }
        }
        else
        {
            if(hintLine != null)
            {
                hintLine.DisableLine();
            }
            //Invoke("DisableLine", 1.0f);

            #region Detect Interactable(Weapon Or Can be fixed Deck)
            RaycastHit hit = new RaycastHit();
            Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * interactDis, Color.blue);
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, 
                out hit, interactDis, ~LayerMask.NameToLayer("PlayerBlock")))
            {
                Focusable hitFocusable = hit.collider.transform.GetComponentInParent<Focusable>();
                if(hitFocusable == null)
                {
                    hitFocusable = hit.collider.transform.GetComponentInChildren<Focusable>();
                }
                if (hit.collider.transform.parent != null 
                    && hitFocusable != null && hitFocusable.focusable)
                {
                    if (focusingObj != null)
                    {
                        focusingObj.GetComponent<Focusable>().focused = false;
                    }
                    focusingObj = hitFocusable.gameObject;
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
    #endregion

    #region DeprecatedHintLine
    ////用来更新瞄准时的参考线的绘制
    //public void UpdateLine()
    //{
    //    if (hasWeapon())
    //    {
    //        Transform weapon = weaponSlot.GetChild(0);
    //        #region DrawDebugLine
    //        if (Debug.unityLogger.logEnabled)
    //        {
    //            Debug.DrawRay(playerCam.transform.position, interactDis * playerCam.transform.forward, Color.blue);
    //            weapon.GetComponent<Weapon>().DrawDebug(playerCam.transform.forward);
    //            Vector3 start = weapon.position;
    //            Vector3 speed = weapon.GetComponent<Weapon>().StartSpeed * playerCam.transform.forward;
    //            float timeStep = LinePredictTime / LineSlice;
    //            for (float i = 0.0f; i < LinePredictTime; i += timeStep)
    //            {
    //                Vector3 end = start + speed * timeStep;
    //                Debug.DrawLine(start, end, predictLineColor);
    //                speed += Physics.gravity * weapon.GetComponent<Weapon>().gravityScale * timeStep;
    //                start = end;
    //            }
    //        }
    //        #endregion

    //        #region DrawLine
    //        if(linePrefab != null)
    //        {
    //            if(lineObj == null)
    //            {
    //                lineObj = Instantiate<GameObject>(linePrefab);
    //            }
    //            else
    //            {
    //                lineObj.SetActive(true);
    //            }
    //            lineObj.transform.position = Vector3.zero;
    //            lineObj.transform.rotation = Quaternion.Euler(Vector3.zero);
    //            VolumetricLineStripBehavior stripe = lineObj.GetComponent<VolumetricLineStripBehavior>();
    //            if(stripe != null)
    //            {
    //                List<Vector3> vertices = new List<Vector3>();
    //                Vector3 cur = weapon.position;
    //                Vector3 end = cur;
    //                Vector3 speed = weapon.GetComponent<Weapon>().StartSpeed * playerCam.transform.forward;
    //                float gravityScale = weapon.GetComponent<Weapon>().gravityScale;
    //                float timeStep = LinePredictTime / LineSlice;
    //                for(float i = 0.0f; i < LinePredictTime; i += timeStep)
    //                {
    //                    vertices.Add(cur);
    //                    end = cur + speed * timeStep;
    //                    #region RayTest: if hit any target or surface

    //                    #endregion
    //                    speed += Physics.gravity * gravityScale * timeStep;
    //                    cur = end;
    //                }
    //                vertices.Add(end);
    //                stripe.UpdateLineVertices(vertices.ToArray());
    //                stripe.LineColor = predictLineColor;
    //            }
    //        }
    //        #endregion
    //    }
    //}
    ////用来禁用参考线
    //public void DisableLine()
    //{
    //    if(lineObj != null)
    //    {
    //        lineObj.SetActive(false);
    //    }
    //}
    #endregion

    public void CallDealWithFocusingObj()
    {
        photonView.RPC("DealWithFocusingObj", RpcTarget.AllViaServer);
    }

    [PunRPC]
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
            fixingSurface.StartFixing(this);
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

    public void CallThrow()
    {
        if (!hasWeapon())
        {
            Debug.Log("<color=red>No Weapon</color>");
            return;
        }
        if (photonView.IsMine)
        {
            photonView.RPC("Throw", RpcTarget.All, 
                playerCam.transform.forward, weaponSlot.GetChild(0).transform.position);
        }
    }

    public void Throw()
    {
        Throw(playerCam.transform.forward, weaponSlot.GetChild(0).transform.position);
    }

    //投掷武器时的处理函数
    [PunRPC]
    public void Throw(Vector3 forward, Vector3 startPos)
    {
        Transform weapon = weaponSlot.GetChild(0);
        weaponSlot.DetachChildren();
        weapon.GetComponent<Weapon>().ThrownOut(forward, startPos);
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

    public void ReceiveDamage(Weapon damWeapon, int dam, Vector3 damDir)
    {
        curHp = Mathf.Max(0, curHp - dam);
        Debug.Log("" + gameObject.name + "ReceiveDamage: <color=red>" + dam + "</color> LeftHp: <color=green>" + curHp + "</color>");
        if(curHp == 0)//计算死亡方向
        {
            Vector3 faceDir = transform.forward;
            faceDir.y = 0;
            Vector3 rightDir = transform.right;
            rightDir.y = 0;
            damDir.y = 0;
            float faceAngle = Vector3.Angle(faceDir, damDir);
            float rightAngle = Vector3.Angle(rightDir, damDir);
            Debug.Log("<color=blue>FaceAngle: " + faceAngle +
                "RightAngle: " + rightAngle + "</color>");
            if(faceAngle < 45f)
            {
                liveState = CharacterLiveState.Dying_Forward;
            }
            else if (faceAngle > 135f)
            {
                liveState = CharacterLiveState.Dying_Backward;
            }
            else if (rightAngle < 45f)
            {
                liveState = CharacterLiveState.Dying_Right;
            }
            else
            {
                liveState = CharacterLiveState.Dying_Left;
            }
            
            deadEvent?.Invoke(this, damWeapon.owner);
        }
    }

    public void ReceiveHeal(int healPoint = 0)
    {
        Debug.Log("<color=pink>" + gameObject.name + "ReceiveHeal: " + healPoint + "</color>");
        curHp = Mathf.Min(curHp + healPoint, maxHp);
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

    public void Suicide()
    {

        //TODO: 计分, 灰屏, 等待时间之类

        PopupHint.PopupUI("<color=red>你跳入海中溺死了自己</color>");
        deadEvent?.Invoke(this, null);
        Respawn();
    }

    public void DeadEnd()
    {
        Respawn();
    }
    
    public void Respawn()
    {
        if(respawnTrans == null)
        {
            Debug.Log("<color=red>RespawnTrans Not AssignedYet</color>");
            return;
        }
        moveControl.transform.position = respawnTrans.position;
        moveControl.transform.rotation = respawnTrans.rotation;
        liveState = CharacterLiveState.Alive;
    }

    #region CallBacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }
    #endregion
}

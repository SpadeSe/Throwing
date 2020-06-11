using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public delegate void HittenEvent();
public delegate void DeadEvent();

public class Player : MonoBehaviour
{
    [Header("Must Init")]
    public Transform weaponSlot;
    public Camera playerCam;
    public FirstPersonAIO moveControl;
    [Header("UI")]
    public GameObject crosshair;
    public GameObject hintUI;

    [Header("Throw")]
    public bool targeting = false;
    public bool throwing = false;
    public int attackType = 0;
    public GameObject linePrefab;
    [HideInInspector]
    public GameObject lineObj;
    public Color predictLineColor = Color.yellow;
    public float LinePredictTime = 5.0f;
    [Range(3, 50)]
    public int LineSlice = 20;

    [Header("Interact")]
    public float interactDis = 1.0f;
    public GameObject focusingObj;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.unityLogger.logEnabled = true;
    }

    private void FixedUpdate()
    {
        #region setState
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
            if(focusingObj != null)
            {
                TakeWeapon();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(crosshair != null)
        {
            crosshair.SetActive(!targeting);
        }
        if (targeting)
        {
            UpdateLine();
        }
        else
        {
            DisableLine();

            #region Detect Interactable
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDis))
            {
                if(hit.collider.transform.parent != null && hit.collider.transform.parent.GetComponent<Weapon>() != null)
                {
                    focusingObj = hit.collider.transform.parent.gameObject;
                    focusingObj.GetComponent<Weapon>().focused = true;
                    if(hintUI != null)
                    {
                        hintUI.SetActive(true);
                    }
                }
            }
            else
            {
                if(focusingObj != null)
                {
                    focusingObj.GetComponent<Weapon>().focused = false;
                    focusingObj = null;
                }
                if (hintUI != null)
                {
                    hintUI.SetActive(false);
                }
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

    //拾取武器时的处理函数
    public void TakeWeapon()
    {
        if(focusingObj == null)
        {
            return;
        }
        if (hasWeapon())
        {
            Transform curWeapon = weaponSlot.transform.GetChild(0);
            curWeapon.parent = null;
            curWeapon.position = focusingObj.transform.position;
            curWeapon.rotation = focusingObj.transform.rotation;
            curWeapon.localScale = focusingObj.transform.localScale;
            curWeapon.GetComponent<Weapon>().taken = false;
        }
        focusingObj.transform.parent = weaponSlot;
        focusingObj.transform.localPosition = Vector3.zero;
        focusingObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        focusingObj.transform.localScale = Vector3.one;
        focusingObj.GetComponent<Weapon>().taken = true;
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
        //weaponSlot.DetachChildren();
        //weapon.GetComponent<Weapon>().ThrowOut(playerCam.transform.forward);
        GameObject copy = GameObject.Instantiate<GameObject>(weapon.gameObject, weaponSlot.transform);
        copy.transform.parent = null;
        copy.GetComponent<Weapon>().ThrowOut(playerCam.transform.forward);
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
}

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
    public float takeDis = 1.0f;
    
    
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
    }

    // Update is called once per frame
    void Update()
    {
        if (targeting)
        {
            UpdateLine();
        }
        else
        {
            DisableLine();
        }

    }

    public void UpdateLine()
    {
        if (hasWeapon())
        {
            Transform weapon = weaponSlot.GetChild(0);
            #region DrawDebugLine
            if (Debug.unityLogger.logEnabled)
            {
                Debug.DrawRay(playerCam.transform.position, 10 * playerCam.transform.forward, Color.blue);
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
    public void DisableLine()
    {
        if(lineObj != null)
        {
            lineObj.SetActive(false);
        }
    }

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

    public bool hasWeapon()
    {
        if(weaponSlot == null)
        {
            return false;
        }
        return weaponSlot.childCount > 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class HintLine : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject hitHintPrefab;
    //[HideInInspector]
    public GameObject lineObj;
    public Color predictLineColor = Color.Lerp(Color.yellow, Color.red, 0.5f);
    public float LinePredictTime = 3.0f;
    [Range(10, 100)]
    public int LineSlice = 50;
    // Start is called before the first frame update
    void Start()
    {
        if(linePrefab == null)
        {
            linePrefab = Resources.Load<GameObject>("LinePrefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLine(Transform weaponTrans, Vector3 forward)
    {
        if (linePrefab != null)
        {
            if (lineObj == null)
            {
                lineObj = Instantiate<GameObject>(linePrefab);
            }
            else
            {
                lineObj.SetActive(true);
            }
            Weapon weapon = weaponTrans.GetComponent<Weapon>();
            lineObj.transform.position = Vector3.zero;
            lineObj.transform.rotation = Quaternion.Euler(Vector3.zero);
            VolumetricLineStripBehavior stripe = lineObj.GetComponent<VolumetricLineStripBehavior>();
            if (stripe != null)
            {
                List<Vector3> vertices = new List<Vector3>();
                Vector3 cur = weapon.transform.position;
                Vector3 end = cur;
                Vector3 speed = weapon.StartSpeed * forward;
                float gravityScale = weapon.gravityScale;
                float timeStep = LinePredictTime / LineSlice;
                float endTime = LinePredictTime;
                RaycastHit hit = new RaycastHit();
                bool findHit = true;
                for (float i = 0.0f; i < endTime; i += timeStep)
                {
                    vertices.Add(cur);
                    end = cur + speed * timeStep;
                    #region RayTest: if hit any target or surface
                    if(findHit && Physics.Raycast(cur, end - cur, out hit, Vector3.Distance(cur, end)))
                    {
                        PlayerCharacter player = hit.collider.GetComponentInChildren<PlayerCharacter>();
                        if ((player != null && player != weapon.owner)
                            || hit.collider.GetComponentInChildren<ItemBase>() != null
                            || hit.collider.GetComponentInChildren<Surface>() != null)
                        {
                            endTime = i + timeStep;
                            timeStep = timeStep / 10;//最后只用10段就很够了
                            end = cur + speed * timeStep;
                            findHit = false;
                        }
                    }
                    #endregion
                    speed += Physics.gravity * gravityScale * timeStep;
                    cur = end;
                }
                vertices.Add(end);
                stripe.UpdateLineVertices(vertices.ToArray());
                stripe.LineColor = predictLineColor;
            }
        }
    }

    public void DisableLine()
    {
        if (lineObj != null)
        {
            lineObj.SetActive(false);
        }
    }
}

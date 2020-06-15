using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class HintLine : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject hitHintPrefab;
    [HideInInspector]
    public GameObject lineObj;
    public Color predictLineColor = Color.yellow;
    public float LinePredictTime = 5.0f;
    [Range(3, 50)]
    public int LineSlice = 20;
    // Start is called before the first frame update
    void Start()
    {
        linePrefab = Resources.Load<GameObject>("LinePrefab");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLine(Transform weapon, Vector3 forward)
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
            lineObj.transform.position = Vector3.zero;
            lineObj.transform.rotation = Quaternion.Euler(Vector3.zero);
            VolumetricLineStripBehavior stripe = lineObj.GetComponent<VolumetricLineStripBehavior>();
            if (stripe != null)
            {
                List<Vector3> vertices = new List<Vector3>();
                Vector3 cur = weapon.position;
                Vector3 end = cur;
                Vector3 speed = weapon.GetComponent<Weapon>().StartSpeed * forward;
                float gravityScale = weapon.GetComponent<Weapon>().gravityScale;
                float timeStep = LinePredictTime / LineSlice;
                for (float i = 0.0f; i < LinePredictTime; i += timeStep)
                {
                    vertices.Add(cur);
                    end = cur + speed * timeStep;
                    //TODO: 
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
    }

    public void DisableLine()
    {
        if (lineObj != null)
        {
            lineObj.SetActive(false);
        }
    }
}

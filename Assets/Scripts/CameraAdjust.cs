/*
 * CameraAdjust
 * 用来给摄像机调整位置, 减少跟随骨骼导致的抖动
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAdjust : MonoBehaviour
{
    public Transform refTrans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(refTrans != null)
        {
            transform.position = refTrans.position;
        }
    }
}

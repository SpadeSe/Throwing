using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Focusable : MonoBehaviour
{
    [Header("Focus")]
    public bool focusable = true;
    public bool focused = false;
    
    public Material focusMat;
    public Color focusColor = Color.yellow;
    public GameObject focusPrefab;
    public GameObject focusObj;
    public GameObject focusHintUI;
    public string focusUIHint;

    private void Awake()
    {
        //focusMat = Resources.Load<Material>("FocusMat");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual bool canShowUI()
    {
        return focusable || focused;
    }
    
    //被盯着的时候的处理函数, 每帧会调用
    public void DealWithFocus()
    {
        
        if(focusable && focused)
        {
            if (focusMat == null || focusPrefab == null)
            {
                return;
            }
            if (focusObj != null)
            {
                return;
            }
            focusObj = Instantiate(focusPrefab, transform);
            Collider[] colliders = focusObj.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            Renderer[] renderers = focusObj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                //Fixme: 这里可能有问题
                renderer.material = focusMat;
                renderer.material.SetColor("g_vOutlineColor", focusColor);
            }
            if(focusHintUI != null && focusHintUI.GetComponentInChildren<Text>() != null)
            {
                focusHintUI.GetComponentInChildren<Text>().text = focusUIHint;
            }
        }
        else
        {
            Destroy(focusObj);
            focusObj = null;
        }
    }
}

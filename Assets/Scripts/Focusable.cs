using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Focusable : MonoBehaviour
{
    [Header("Focus")]
    public bool focusable = true;
    public bool focused = false;
    public Player focusingPlayer;
    
    public Material focusMat;
    public Color focusColor = Color.yellow;
    public GameObject focusPrefab;
    public GameObject focusObj;
    public GameObject focusingUI;
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

    public virtual void ShowUI(GameObject focusHintUI)
    {
        if (!(focusable && focused))
        {
            focusHintUI.SetActive(false);
            return;
        }
        focusHintUI.SetActive(true);
        if (focusHintUI != null && focusHintUI.GetComponentInChildren<Text>() != null)
        {
            focusHintUI.GetComponentInChildren<Text>().text = focusUIHint;
        }
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
            //Debug.Log(focusObj);
            Collider[] colliders = focusObj.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            Renderer[] renderers = focusObj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
                //Fixme: 这里可能有问题
                renderer.material = focusMat;
                renderer.material.SetColor("g_vOutlineColor", focusColor);
            }
            
        }
        else
        {
            Destroy(focusObj);
            focusObj = null;
        }
    }
}

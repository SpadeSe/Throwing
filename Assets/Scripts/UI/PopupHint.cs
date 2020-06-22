using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHint : MonoBehaviour
{
    public static GameObject UIItemPrefab;
    public static GameObject CanvasPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PopupUI(string hint, RectTransform canvasTrans = null)
    {
        PopupUI(hint, canvasTrans, Color.white, Color.black);
    }

    public static void PopupUI(string hint, RectTransform canvasTrans, Color textColor, Color backColor)
    {
        if(UIItemPrefab == null)
        {
            UIItemPrefab = Resources.Load<GameObject>("PopupUI");
        }
        if(CanvasPrefab == null)
        {
            CanvasPrefab = Resources.Load<GameObject>("PopupCanvas");
        }
        GameObject popup = null;
        if(canvasTrans == null)
        {
            popup = Instantiate(CanvasPrefab, null);
        }
        else
        {
            popup = Instantiate(UIItemPrefab, canvasTrans);
        }
        Image back = popup.GetComponentInChildren<Image>();
        back.material.SetColor("_Color", backColor);
        Text text = popup.GetComponentInChildren<Text>();
        text.text = hint;
        text.material.SetColor("_Color", textColor);
        popup.GetComponentInChildren<Animation>().Play();
        Destroy(popup, popup.GetComponentInChildren<Animation>().clip.length);
        //TODO: 根据字数来调整尺寸
    }
}

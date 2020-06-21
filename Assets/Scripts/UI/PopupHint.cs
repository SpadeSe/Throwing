using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHint : MonoBehaviour
{
    public static GameObject UIPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PopupUI(string hint, RectTransform canvasTrans)
    {
        PopupUI(hint, canvasTrans, Color.white, Color.black);
    }

    public static void PopupUI(string hint, RectTransform canvasTrans, Color textColor, Color backColor)
    {
        if(UIPrefab == null)
        {
            UIPrefab = Resources.Load<GameObject>("PopupUI");
        }
        GameObject popup = Instantiate(UIPrefab, canvasTrans);
        Image back = popup.GetComponentInChildren<Image>();
        back.material.SetColor("_Color", backColor);
        Text text = popup.GetComponentInChildren<Text>();
        text.text = hint;
        text.material.SetColor("_Color", textColor);
        popup.GetComponent<Animation>().Play();
        Destroy(popup, popup.GetComponent<Animation>().clip.length);
        //TODO: 根据字数来调整尺寸
    }
}

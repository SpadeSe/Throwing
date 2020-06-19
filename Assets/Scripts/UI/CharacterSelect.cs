using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect), typeof(ToggleGroup))]
public class CharacterSelect : MonoBehaviour
{
    [Header("Settings")]
    public Transform characterTrans;
    public GameObject DisplayTogglePrefab;//需要和CharaterSelectInfo相配
    public List<CharacterSelectInfo> characters;//暂时先手动配着, 之后可能用resource文件夹的方式来配
    public List<GameObject> displayObjs;
    public GameObject curDisplaying;
    [Header("Debug")]
    public ScrollRect scroll;

    // Start is called before the first frame update
    void Start()
    {
        if (scroll == null)
        {
            scroll = GetComponent<ScrollRect>();
        }
        RectTransform contentTrans = scroll.content;
        //清空原有的
        List<GameObject> childs = new List<GameObject>();
        for(int i = 0; i < contentTrans.childCount; i++)
        {
            childs.Add(contentTrans.GetChild(i).gameObject);
        }
        contentTrans.DetachChildren();
        foreach(GameObject child in childs)
        {
            Destroy(child);
        }
        //生成选项和待使用的显示角色
        for(int i = 0; i < characters.Count; i++)
        {
            //生成角色
            GameObject character = Instantiate(characters[i].displayPrefab, characterTrans);
            //character.transform.position = Vector3.zero;
            //character.transform.rotation = Quaternion.Euler(Vector3.zero);
            character.SetActive(false);
            displayObjs.Add(character);
            //调整toggle
            GameObject toggle = Instantiate(DisplayTogglePrefab, contentTrans);
            toggle.GetComponentInChildren<Image>().sprite = characters[i].displaySprite;
            toggle.GetComponentInChildren<Text>().text = characters[i].description;
            RectTransform toggleTrans = toggle.GetComponent<RectTransform>();
            toggleTrans.anchoredPosition = new Vector2(0, -i * toggleTrans.sizeDelta.y);

            Toggle toggleComp = toggle.GetComponent<Toggle>();
            toggleComp.group = GetComponent<ToggleGroup>();
            toggleComp.isOn = false;
            toggleComp.onValueChanged.AddListener(ChangeCharacter);

        }
        //计算尺寸, 
        if(contentTrans.childCount > 0)
        {
            displayObjs[0].SetActive(true);
            curDisplaying = displayObjs[0];

            contentTrans.sizeDelta = new Vector2(contentTrans.sizeDelta.x,
                contentTrans.childCount * ((RectTransform)contentTrans.GetChild(0)).sizeDelta.y);

            contentTrans.GetChild(0).GetComponent<Toggle>().isOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCharacter(bool activated)
    {
        if (activated)
        {
            int activaIdx = -1;
            GameObject toggleObj = null;
            for(int i = 0; i < scroll.content.childCount; i++)
            {
                if (scroll.content.GetChild(i).GetComponent<Toggle>().isOn)
                {
                    activaIdx = i;
                    //toggleObj = scroll.content.GetChild(i).gameObject;
                    break;
                }
            }
            curDisplaying.SetActive(false);
            curDisplaying = displayObjs[activaIdx];
            curDisplaying.SetActive(true);
        }
    }
}

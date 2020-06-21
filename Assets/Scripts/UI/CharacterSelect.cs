using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("Hand Init")]
    public Transform characterTrans;
    public GameObject scrollObj;//需要这个scroll有scrollRect和ToggleGroup
    [Header("Settings")]
    public GameObject DisplayTogglePrefab;//需要和CharaterSelectInfo相配
    public List<CharacterSelectInfo> characters;//暂时先手动配着, 之后可能用resource文件夹的方式来配
    public List<GameObject> displayObjs;
    public GameObject curDisplaying;
    [Header("Debug")]
    public ScrollRect scroll;
    public ToggleGroup group;
    public PlayerController playerController;

    public void Init()
    {
        scroll = scrollObj.GetComponent<ScrollRect>();
        group = scrollObj.GetComponent<ToggleGroup>();
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag(Definitions.playerControllerTag).GetComponent<PlayerController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
       // CreateSelectScroll();
    }

    public void CreateSelectScroll()
    {
        RectTransform contentTrans = scroll.content;
        //清空原有的
        List<GameObject> childs = new List<GameObject>();
        for (int i = 0; i < contentTrans.childCount; i++)//UI
        {
            childs.Add(contentTrans.GetChild(i).gameObject);
        }
        if (characterTrans.childCount > 0)//角色位置
        {
            for (int i = 0; i < characterTrans.childCount; i++)
            {
                childs.Add(characterTrans.GetChild(i).gameObject);
            }
        }
        contentTrans.DetachChildren();
        characterTrans.DetachChildren();
        foreach (GameObject child in childs)
        {
            Destroy(child);
        }
        //生成选项和待使用的显示角色
        for (int i = 0; i < characters.Count; i++)
        {
            //生成角色
            GameObject character = Instantiate(characters[i].displayPrefab, characterTrans);
            character.transform.localPosition = Vector3.zero;
            character.transform.localRotation = Quaternion.Euler(Vector3.zero);
            displayObjs.Add(character);
            character.SetActive(false);
            //调整toggle
            GameObject toggle = Instantiate(DisplayTogglePrefab, contentTrans);
            toggle.GetComponentsInChildren<Image>()[1].sprite = characters[i].displaySprite;
            toggle.GetComponentInChildren<Text>().text = characters[i].description;
            //RectTransform toggleTrans = toggle.GetComponent<RectTransform>();
            //toggleTrans.anchoredPosition = new Vector2(0, -i * toggleTrans.sizeDelta.y);

            Toggle toggleComp = toggle.GetComponent<Toggle>();
            toggleComp.group = group;
            toggleComp.isOn = false;
            toggleComp.onValueChanged.AddListener(ChangeCharacter);

        }
        //计算尺寸, 
        if (contentTrans.childCount > 0)
        {
            displayObjs[0].SetActive(true);
            curDisplaying = displayObjs[0];
            playerController.selectCharacter = characters[0];

            contentTrans.GetComponent<GridLayoutGroup>().cellSize = ((RectTransform)contentTrans.GetChild(0)).sizeDelta;
            contentTrans.sizeDelta = new Vector2(contentTrans.sizeDelta.x,
                contentTrans.childCount * ((RectTransform)contentTrans.GetChild(0)).sizeDelta.y);

            contentTrans.GetChild(0).GetComponent<Toggle>().isOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //用来给选择角色的toggle的委托调用的函数
    public void ChangeCharacter(bool activated)
    {
        if (activated)
        {
            int activeIdx = -1;
            //GameObject toggleObj = null;
            for(int i = 0; i < scroll.content.childCount; i++)
            {
                if (scroll.content.GetChild(i).GetComponent<Toggle>().isOn)
                {
                    activeIdx = i;
                    //toggleObj = scroll.content.GetChild(i).gameObject;
                    break;
                }
            }
            curDisplaying.SetActive(false);
            curDisplaying = displayObjs[activeIdx];
            curDisplaying.SetActive(true);
            playerController.selectCharacter = characters[activeIdx];
        }
    }
}

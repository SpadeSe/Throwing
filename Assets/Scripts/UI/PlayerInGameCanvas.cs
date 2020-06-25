using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameCanvas : MonoBehaviour
{
    [Header("DataRefs")]
    public RoomRecorder roomRecorder;//动态获取
    public PlayerCharacter player;//必须靠其他东西来分配
    public Sprite NoWeaponSprite;

    [Header("UIRefs")]
    public Image TimeImage;
    public Text Time_Min;
    public Text Time_Sec;
    [Space(10)]
    #region Red
    public Text redKills;
    public Text redDeaths;
    public Text redScore;
    #endregion
    [Space(10)]
    #region Blue
    public Text blueKills;
    public Text blueDeaths;
    public Text blueScore;
    #endregion
    [Space(10)]
    #region Player
    public GameObject playerHearts;
    public GameObject HeartPrefab;
    public Text playerKills;
    public Text playerDeaths;
    public Text playerScore;
    public Image weaponImage;
    public Text weaponNameText;
    public Text weaponDescriptionText;
    #endregion
    [Space(10)]
    #region Other
    public GameObject crossHair;
    public GameObject interactHint;
    public Scrollbar fixBar;
    //TODO
    public GameObject popUpHint;
    public GameObject Communication;
    #endregion


    // Start is called before the first frame update
    void Start()
    {

        GameObject roomRecorderObj = GameObject.FindGameObjectWithTag(Definitions.roomRecorderTag);
        if(roomRecorderObj != null)
        {
            roomRecorder = roomRecorderObj.GetComponent<RoomRecorder>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        #region Time
        if(roomRecorder != null)
        {
            Time_Sec.text = string.Format("{0:00}", (int)roomRecorder.leftTime % 60);
            Time_Min.text = string.Format("{0:00}", (int)roomRecorder.leftTime / 60);
        }
        #endregion

        #region player
        //TODO: playerHearts
        //TODO: playerData

        //crossHair
        crossHair.SetActive(!player.targeting);
        //focusingObj
        if(player.focusingObj != null)
        {
            Focusable focusable = player.focusingObj.GetComponent<Focusable>();
            if(focusable.focusable && focusable.focused)
            {
                interactHint.SetActive(true);
                interactHint.GetComponentInChildren<Text>().text = focusable.focusUIHint;
            }
            else
            {
                interactHint.SetActive(false);
            }
        }
        else
        {
            interactHint.SetActive(false);
        }
        //fixingBar
        if(player.fixingSurface != null)
        {
            if (player.fixingSurface.fixing)
            {
                fixBar.gameObject.SetActive(true);
                fixBar.size = player.fixingSurface.fixProgress / player.fixingSurface.fixTime;
            }
        }
        else
        {
            fixBar.gameObject.SetActive(false);
        }
        if (player.hasWeapon())
        {
            Weapon weapon = player.weaponSlot.GetChild(0).GetComponent<Weapon>();
            weaponImage.sprite = weapon.UISprite;
            weaponNameText.text = weapon.weaponName;
            weaponDescriptionText.text = weapon.description;
        }
        else
        {
            weaponImage.sprite = NoWeaponSprite;
            weaponNameText.text = "(无)";
            weaponDescriptionText.text = "你还没有拿起武器喔";
        }
        //LifeBar, TODO: 优化实现心破碎和恢复的动画效果
        int curHeart = playerHearts.transform.childCount;
        for(; curHeart < player.maxHp; curHeart++)
        {
            GameObject heart = Instantiate(HeartPrefab, playerHearts.transform);
            RectTransform heartTrans = heart.GetComponent<RectTransform>();
            heartTrans.anchoredPosition = new Vector2(curHeart * heartTrans.sizeDelta.x, heartTrans.anchoredPosition.y);
        }
        for(int i = 0; i < player.curHp; i++)
        {
            playerHearts.transform.GetChild(i).gameObject.SetActive(true);
        }
        for(int i = player.curHp; i < curHeart; i++)
        {
            playerHearts.transform.GetChild(i).gameObject.SetActive(false);
        }
        //KillAndDeadAndScore
        playerKills.text = player.kill.ToString();
        playerDeaths.text = player.dead.ToString();
        playerScore.text = player.score.ToString();

        #endregion

        #region RoundInfo
        if(roomRecorder != null)
        {
            redKills.text = roomRecorder.redIngameRecords.KillCount.ToString();
            redDeaths.text = roomRecorder.redIngameRecords.DeadCount.ToString();
            redScore.text = roomRecorder.redIngameRecords.Score.ToString();
            blueKills.text = roomRecorder.blueIngameRecords.KillCount.ToString();
            blueDeaths.text = roomRecorder.blueIngameRecords.DeadCount.ToString();
            blueScore.text = roomRecorder.blueIngameRecords.Score.ToString();
        }
        #endregion
    }
}

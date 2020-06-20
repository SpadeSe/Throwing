using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameCanvas : MonoBehaviour
{
    [Header("DataRefs")]
    public RoundManager roundManager;//动态获取
    public PlayerCharacter player;//必须靠其他东西来分配

    [Header("UIRefs")]
    public Image TimeImage;
    public Text TimeText;
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
    public Text weaponText;
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
        //TODO: 获取自己的dataRefs
        if(roundManager == null)
        {
            roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        #region roundManager
        #endregion

        #region player
        //TODO: playerHearts
        //TODO: playerData

        //crossHair
        crossHair.SetActive(!player.targeting);
        //focusingObj
        #endregion
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
        #region PopUp
        #endregion
    }
}

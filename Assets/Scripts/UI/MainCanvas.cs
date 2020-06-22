using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MainCanvas : MonoBehaviourPunCallbacks
{
    [Header("UI Refs")]
    public Button startButton;
    public Button QuitButton;
    public GameObject mainPanel;
    public GameObject selectPanel;
    public GameObject RoomPanel;
    

    // Start is called before the first frame update
    void Start()
    {
        selectPanel.GetComponent<CharacterSelect>().Init();
        selectPanel.GetComponent<RoomManager>().Init();
        selectPanel.SetActive(false);
        mainPanel.SetActive(true);
        RoomPanel.SetActive(false);
        startButton.onClick.AddListener(StartGame);
        QuitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Start Game. Connect to master");
            PhotonNetwork.ConnectUsingSettings();        }
        PopupHint.PopupUI("开始游戏", (RectTransform)transform);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        mainPanel.SetActive(false);
        selectPanel.SetActive(true);
        selectPanel.GetComponent<CharacterSelect>().CreateSelectScroll();
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PopupHint.PopupUI("成功加入大厅");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

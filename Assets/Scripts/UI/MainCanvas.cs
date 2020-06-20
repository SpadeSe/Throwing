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
    

    // Start is called before the first frame update
    void Start()
    {
        selectPanel.GetComponent<CharacterSelect>().Init();
        selectPanel.GetComponent<RoomManager>().Init();
        selectPanel.SetActive(false);
        mainPanel.SetActive(true);
        startButton.onClick.AddListener(StartGame);
        QuitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        mainPanel.SetActive(false);
        selectPanel.SetActive(true);
        //selectPanel.GetComponent<CharacterSelect>().CreateSelectScroll();

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Start Game. Connect to master");
            PhotonNetwork.ConnectUsingSettings();
        }
        PopupHint.PopupUI("开始游戏", (RectTransform)transform);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
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

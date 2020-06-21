using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [Header("Hand Init")]
    public GameObject inputObj;
    [Header("For Debug")]
    public InputField input;
    public PlayerController playerController;
    // Start is called before the first frame update

    public void Init()
    {
        input = inputObj.GetComponent<InputField>();
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag(Definitions.playerControllerTag).GetComponent<PlayerController>();
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public GameObject characterPrefab;
    // Start is called before the first frame update
    private void Awake()
    {
        gameObject.tag = "PlayerController";

    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

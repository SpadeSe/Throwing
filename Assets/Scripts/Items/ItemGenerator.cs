using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用来生成item并且给初速度. 需要item有rigidbody;
public class ItemGenerator : MonoBehaviour
{
    public bool CanGen = true;
    [Header("Settings")]
    public float startGenDelay = 1.0f;
    public float GenInterval = 5.0f;
    public float startSpeed = 2.0f;
    [Header("Refs")]
    public Transform startTrans;
    public Transform dirTrans;
    public List<GameObject> ItemPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        if (CanGen)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: 根据roomrecorder的状态来调整CanGen

    }

    IEnumerator GenItem()
    {
        yield return new WaitForSeconds(startGenDelay);

    }
}

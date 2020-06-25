using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用来生成item并且给初速度. 需要item有rigidbody;
public class ItemGenerator : MonoBehaviour
{
    public bool CanGen = true;
    Coroutine genRoutine;
    [Header("Settings")]
    public float startGenDelay = 1.0f;
    public float GenInterval = 5.0f;
    //public float startSpeed = 2.0f;
    public float itemDuration = 8.0f;
    public float itemRotateSpeed = 3.0f;
    [Header("Refs")]
    public Transform startTrans;
    //public Transform dirTrans;
    public Transform endTrans;
    public List<GameObject> ItemPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(var renderer in renderers)
        {
            renderer.enabled = false;
        }
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }
        if (CanGen)
        {
            genRoutine = StartCoroutine(GenItem());
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
        while (CanGen)
        {
            GameObject item = Instantiate(ItemPrefabs[Random.Range(0, ItemPrefabs.Count)], startTrans);
            item.GetComponent<Rigidbody>().AddForce((endTrans.position - startTrans.position) / itemDuration, ForceMode.Impulse);
            item.GetComponent<Rigidbody>().AddTorque(item.transform.up * itemRotateSpeed, ForceMode.Impulse);
            Destroy(item, itemDuration);
            yield return new WaitForSeconds(GenInterval);
        }
    }
}

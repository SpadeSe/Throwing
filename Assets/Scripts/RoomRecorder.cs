/*
 * 用来记录当前房间下玩家的角色prefab选择以及阵容选择
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRecorder : MonoBehaviour
{
    public bool clearRecord = false;
    public List<RoomRecord> redRecords;
    public List<RoomRecord> blueRecords;
    public int curIdx = 0; //从0开始不断增加, 不回退.(反正溢出的可能性几乎没有)

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //玩家初次进入房间的时候要注册进这个roomRecorder, 然后recoder会返回一个int来标识
    public int RegisterToRoom(CharacterSelectInfo selectInfo, string name)
    {
        RoomRecord newRecord = new RoomRecord
        (
            curIdx,
            name,
            selectInfo,
            redRecords.Count <= blueRecords.Count ? PlayerSide.RED : PlayerSide.BLUE
        );
        if(newRecord.side == PlayerSide.RED)
        {
            redRecords.Add(newRecord);
        }
        else
        {
            blueRecords.Add(newRecord);
        }
        curIdx++;
        return curIdx - 1;
    }

    //玩家离开房间的时候需要调用, 将记录移除
    public void LeaveRoom(int id)
    {
        if(!redRecords.Remove(redRecords.Find((r) => r.id == id)))
        {
            blueRecords.Remove(blueRecords.Find((r) => r.id == id));
        }
    }


}

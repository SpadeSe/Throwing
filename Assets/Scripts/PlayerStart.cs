using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个也得同步
public class PlayerStart : MonoBehaviour
{
    public List<Transform> redStarts;
    public int redCount = 0;
    public List<Transform> blueStarts;
    List<bool> BlueStartsTaken;
    public int blueCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = Definitions.playerStartTag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform getStartTrans(PlayerSide side)
    {
        //TODO: 检测这里的溢出等各种问题
        if(side == PlayerSide.RED)
        {
            return redStarts[redCount++];
        }
        else
        {
            return blueStarts[blueCount++];
        }
    }
}

/*
 * RoundManager
 * 回合管理者, 管理每局游戏中的各种数据.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoundManager : MonoBehaviour
{

    // Start is called before the first frame update
    [Header("Settings")]
    public float RoundDuration = 180.0f;
    public int KillScore = 2;
    public int SuicideScore = 1;

    [Header("States")]
    public float RoundTime;

    public List<SideRecords> records;

    //TODO: 暂时先全部手拖, 做到关卡跳转的时候再来动态获取
    [Header("Blue")]
    public List<GameObject> bluePrefabs;
    public List<Transform> blueSpawnPoses;
    public List<PlayerCharacter> bluePlayers;
    public int blueKills;
    public int blueScore;

    [Header("Red")]
    public List<GameObject> redPrefabs;
    public List<Transform> redSpawnPoses;
    public List<PlayerCharacter> redPlayers;
    public int redKills;
    public int redScore;


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //test
        //GameInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //初始化各种状态, 生成各种东西
    public void GameInit()
    {
        
        foreach(PlayerCharacter player in redPlayers)
        {
            player.deadEvent += PlayerDead;
        }
        foreach(PlayerCharacter player in bluePlayers)
        {
            player.deadEvent += PlayerDead;
        }
        //FinishInit
        GameStart();
    }

    //开放玩家的操作和画面显示
    public void GameStart()
    {

    }

    //禁用操作, 停止游戏, 结算与显示结算结果
    public void GameEnd()
    {

    }

    public void PlayerDead(PlayerCharacter deadPlayer, PlayerCharacter killer)
    {
        int score = killer == null ? SuicideScore : KillScore;
        int kill = killer == null ? 0 : 1;
        if (deadPlayer.side == PlayerSide.RED)
        {
            blueScore += score;
            blueKills += kill;
        }
        else
        {
            redScore += score;
            redKills += kill;
        }
    }
}

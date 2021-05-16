using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    //Play.cs用来更新游戏内容，是游戏系统与玩家的桥梁。
    //在FixedUpdate中响应按键，根据按键对Game做出相应的操作，再从Game调取信息，显示在游戏界面上。



    //几个延迟参数的设定
    public const float LOCK_DELAY = 1f;
    public const float GRAVITY = 0.5f;
    public const float SARR = 0.001f;
    public const float DAS = 0.083f;//相当于60fps中的5帧
    public const float ARR = 0.0f;

    public const int MAX_NEXT = 6;

    //按键
    private bool key_left;
    private bool key_right;
    private bool key_cw;
    private bool key_ccw;
    private bool key_softdrop;
    private bool key_harddrop;
    private bool key_hold;
    private bool key_restart;

    //对于左右同时按的优化
    private bool turnback = false;


    public GameObject ParentField;
    public GameObject[] MinoTiles;
    public GameObject[] Minoes;

    public GameObject HoldArea;
    public GameObject NextArea;


    public Sprite[,] FieldSprites;

    private int lastInput;
    private bool arrTrigger = false;

    public TextMesh clearMsg;

    private float fallTimer;
    private float lockTimer;
    private float dasTimer;
    private float arrTimer;
    private float clearMsgTimer;

    private Game GAME;


    private float startTime;

    public Text TxtStats;
    public Text TxtClearMsg;


    private float GetCurrentTime()
    {
        System.TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
        float t = (float)time.TotalSeconds;//现在的时间
        return t;
    }

    private float GetGameTime()
    {
        return GetCurrentTime() - startTime;
    }

    public void UpdateStats()
    {
        int piece = GAME.statPiece;
        int atk = GAME.statAttack;
        float gameTime = GetGameTime();

        String s = String.Format("时间：{0:0.00}\n块数：{1}\n消行：{2}\n攻击：{3}\n", GetGameTime(),piece, GAME.statLine, atk);
        s += String.Format("PPS：{0:0.00}\nAPM：{1:0.00}\n", piece/gameTime, atk/gameTime*60);

        TxtStats.text = s;
    }


    private void InstChild(GameObject child,Vector3 position,GameObject parent) //实例化方块的prefab（方块）
    {
        Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);

    }

    private void InstChild(GameObject child, Vector3 position, GameObject parent,float scale)//实例化prefab，带缩放（hold/next）
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale,scale,scale);
    }

    private void InstChild(GameObject child, Vector3 position, GameObject parent, float scale,float alpha)//实例化prefab，带缩放和透明度（砖块阴影）
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale, scale, scale);
        Color tmp = t.GetComponent<SpriteRenderer>().color;
        tmp.a = alpha;
        t.GetComponent<SpriteRenderer>().color = tmp;
    }

    private void DestroyAllChild(GameObject parent) {//删除所有child object
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void UpdateHold()
    {
        int holdid = GAME.GetHoldId();
        DestroyAllChild(HoldArea);
        if (holdid != 0)
        {
            InstChild(Minoes[holdid-1], Vector3Int.zero, HoldArea);
        }
    }

    private void UpdateNext()
    {
        DestroyAllChild(NextArea);
        int[] nextSeq = GAME.GetNextSeq();
        for (int i = 0; i < MAX_NEXT; i++) {
            int id = nextSeq[i];
            Vector3 offset = Vector3.zero;
            if (id == Mino.NameToId("J"))
            {
                offset.x = -0.5f;
            }
            else if (id == Mino.NameToId("I")) {
                offset.x = 0.5f;
            }
            else if (id == Mino.NameToId("O"))
            {
                offset.y = 0.5f;
            }
            InstChild(Minoes[nextSeq[i] - 1], new Vector3(0,-2.5f*i,0)+offset, NextArea,0.8f);
        }
    }

    private void UpdateFieldSprites()
    {
        int[,] field = GAME.GetFieldArray();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                if (field[x, y] == 0)
                {
                }
                if (field[x, y] > 0 && field[x, y] < 20)//正在下落的
                {
                    InstChild(MinoTiles[field[x, y] - 1], new Vector3(x, y, 0),ParentField);
                }
                if (field[x, y] < 0)//已经锁定的
                {
                    InstChild(MinoTiles[-field[x, y] - 1], new Vector3(x, y, 0), ParentField);
                }
                if (field[x, y] > 20)//砖影
                {
                    InstChild(MinoTiles[field[x, y] - 21], new Vector3(x, y, 0), ParentField,1f,0.5f);
                }
            }
        }
    }

    private void DestroyPreviousSprites()
    {
        DestroyAllChild(ParentField);
    }


    void Start()
    {
        GAME = new Game();
        startTime = GetCurrentTime();
    }

    void Update()
    {

        key_harddrop = Input.GetKeyDown("space");//硬降
        key_hold = Input.GetKeyDown("w");//hold
        key_restart = Input.GetKeyDown("f4");

        key_cw = Input.GetKeyDown("right");
        key_ccw = Input.GetKeyDown("left");
        DestroyPreviousSprites();
        UpdateFieldSprites();
        UpdateHold();
        UpdateNext();
        UpdateStats();
        //f.SetField(game.field);
    }

    void FixedUpdate()
    {
        float TIME = GetCurrentTime();//现在的时间
        key_left = Input.GetKey("a");//左右
        key_right = Input.GetKey("d");
        key_softdrop = Input.GetKey("s");//软降
        if (key_restart)
        {
            GAME.Restart();
            startTime = TIME;
        }
        if (GAME.Gaming())//游戏进行中
        {
            int input = 0;
            if (key_left && key_right)
            {
                if (!turnback)
                {

                    arrTrigger = false;
                    arrTimer = 0;
                    dasTimer = 0;
                    input = -lastInput;
                    turnback = true;
                }
                else
                {
                    input = lastInput;
                }
            }
            else
            {

                if (key_left) input = -1;
                if (key_right) input = 1;
                turnback = false;
            }





            if (key_hold)
            {
                GAME.Hold();
                key_hold = false;
            }
            if (key_ccw)
            {
                GAME.CCWRotate();//逆时针旋转
                key_ccw = false;
            }
            if (key_cw)
            {
                GAME.CWRotate();
                //顺时针旋转

                key_cw = false;
            }

            if (key_harddrop)
            {
                GAME.HardDrop();//硬降
                key_harddrop = false;
            }

            if (input != 0)
            {
                //左右移动
                if (lastInput == 0 || input == -lastInput)//反向移动或刚开始移动则重置das
                {
                    arrTrigger = false;
                    arrTimer = 0;
                    dasTimer = 0;
                }

                if (!arrTrigger)//如果不是arr阶段
                {

                    if (dasTimer == 0)//按下按键后移动的第一下
                    {
                        GAME.Move(input);
                        dasTimer = TIME;
                    }
                    else if (TIME - dasTimer >= DAS)//经过das延迟后移动第二下
                    {
                        dasTimer = 0;
                        arrTrigger = true;
                        GAME.Move(input);
                    }
                }
                else
                {
                    if (ARR == 0) //真·0ARR
                    {
                        for (int i = 0; i < 9; i++) {
                            GAME.Move(input);
                        }
                    }
                    else
                    {
                        if (arrTimer == 0)
                            arrTimer = TIME;
                        if (TIME - arrTimer >= ARR)
                        {
                            GAME.Move(input);
                            arrTimer = TIME;
                        }
                    }

                }

            }
            else//如果本次也不移动则重置das
            {
                arrTrigger = false;
                arrTimer = 0;
                dasTimer = 0;
            }
            lastInput = input;
            //自然下落or软降
            if (TIME - fallTimer >= (key_softdrop ? SARR : GRAVITY))
            {//每0.5s降落一次

                if (GAME.Fall() == 0)//如果成功下落1格
                {
                    lockTimer = 0;
                }
                else
                { //如果已经到底了
                    if (lockTimer == 0) lockTimer = TIME;

                }


                fallTimer = TIME;
            }
            if (lockTimer != 0)
            {
                if (TIME - lockTimer >= LOCK_DELAY)
                {
                    GAME.LockMino();
                    lockTimer = 0;
                    // f.UpdateNextTilemap(game.next);

                }
            }

        }


        if (TIME - clearMsgTimer >= 2 && clearMsgTimer != 0)
        {
            clearMsg.text = "";
            clearMsgTimer = 0;
        }
    }
}


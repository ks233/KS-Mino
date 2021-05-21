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
    public const float LOCK_DELAY = 0.5f;//guideline规定
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

    private bool key_debug;


    //对于左右同时按的优化
    private bool turnback = false;


    public GameObject ParentField;
    public GameObject[] MinoTiles;
    public GameObject[] Minoes;

    public GameObject HoldArea;
    public GameObject NextArea;
    public GameObject ActiveMinoParent;

    private int lastInput;
    private bool arrTrigger = false;

    private float fallTimer;
    private float lockTimer;
    private float dasTimer;
    private float arrTimer;
    private float clearMsgTimer;

    public Game game;

    private float prevFrameTime;

    private float gameTime;

    public Text TxtStats;
    public Text TxtClearMsg;


    private bool locked = false;//避免同一个FixedUpdate里调用多次LockMino

    private bool gameover = false;

    private float GetCurrentTime()
    {
        System.TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
        float t = (float)time.TotalSeconds;//现在的时间
        return t;
    }


    public void UpdateStats()
    {
        int piece = game.statPiece;
        int atk = game.statAttack;

        String s = String.Format("时间：{0:0.00}\n块数：{1}\n消行：{2}\n攻击：{3}\n", gameTime, piece, game.statLine, atk);
        s += String.Format("PPS：{0:0.00}\nAPM：{1:0.00}\n", piece / gameTime, atk / gameTime * 60);

        TxtStats.text = s;
    }

    private void UpdateActiveMinoDisplay()
    {
        DestroyAllChild(ActiveMinoParent);
        Mino tmpMino = game.GetActiveMino();

        //方块
        List<Vector2Int> l = Field.GetAllCoordinates(game.activeMino);
        int ghostDist = tmpMino.GetPosition().y - game.GetGhostY();
        foreach (Vector2Int v in l)
        {
            DisplayUtils.InstChild(MinoTiles[tmpMino.GetIdInt() - 1], new Vector3(v.x, v.y, 0), ActiveMinoParent);
            if (game.Gaming())
            {

                DisplayUtils.InstChild(MinoTiles[tmpMino.GetIdInt() - 1], new Vector3(v.x, v.y - ghostDist, 0), ActiveMinoParent, 1, 0.5f);
            }
        }
    }


    private void DestroyAllChild(GameObject parent)
    {//删除所有child object
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void UpdateHoldDisplay()
    {
        int holdid = game.GetHoldId();
        DestroyAllChild(HoldArea);
        if (holdid != 0)
        {
            DisplayUtils.InstChild(Minoes[holdid - 1], Vector3Int.zero, HoldArea);
        }
    }

    private void UpdateNextDisplay()
    {
        DestroyAllChild(NextArea);
        int[] nextSeq = game.GetNextSeq();
        for (int i = 0; i < MAX_NEXT; i++)
        {
            int id = nextSeq[i];
            Vector3 offset = Vector3.zero;
            if (id == Mino.NameToId("J"))
            {
                offset.x = -0.5f;
            }
            else if (id == Mino.NameToId("I"))
            {
                offset.x = 0.5f;
            }
            else if (id == Mino.NameToId("O"))
            {
                offset.y = 0.5f;
            }
            DisplayUtils.InstChild(Minoes[nextSeq[i] - 1], new Vector3(0, -2.5f * i, 0) + offset, NextArea, 0.8f);
        }
    }

    public void UpdateFieldDisplay()
    {
        DestroyAllChild(ParentField);
        int[,] field = game.GetFieldArray();




        //InstChild(Minoes[activeMinoId-1], tmp, ParentField,1,0.6f);

        //场地
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                if (1 <= field[x, y] && field[x, y] <= 7)
                    DisplayUtils.InstChild(MinoTiles[field[x, y] - 1], new Vector3(x, y, 0), ParentField);
            }
        }
    }


    void Start()
    {
        game = new Game();
        Restart();
    }
    public void Restart()
    {
        gameover = false;
        game.Restart();
        gameTime = 0;
        prevFrameTime = GetCurrentTime();
        UpdateHoldDisplay();
        UpdateNextDisplay();
        UpdateFieldDisplay();
        UpdateActiveMinoDisplay();
    }

    private void ShowClearMsg(string msg)
    {
        if (msg != "")
        {
            clearMsgTimer = GetCurrentTime();

            TxtClearMsg.text = msg;
        }
    }

    void Update()
    {
        float TIME = GetCurrentTime();//现在的时间
        if (PauseMenu.GameIsPaused)
        {
            prevFrameTime = TIME;
        }
        else
        {
            gameTime += TIME - prevFrameTime;
            prevFrameTime = TIME;
        }
        key_harddrop = Input.GetKeyDown("space");//硬降
        key_hold = Input.GetKeyDown("w");//hold
        key_restart = Input.GetKeyDown("f4");

        key_cw = Input.GetKeyDown("right");
        key_ccw = Input.GetKeyDown("left");

        key_debug = Input.GetKeyDown("b");
        if (game.Gaming())
        {

            UpdateStats();
        }
        else {
            if (!gameover)
            {
                int[,] field = game.GetFieldArray();

                string s = "";


                //InstChild(Minoes[activeMinoId-1], tmp, ParentField,1,0.6f);

                //场地
                for (int y = 0; y < 23; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {

                        s += field[x, 22 - y];
                    }
                    s += "\n";
                }
                Debug.Log(s);
                gameover = true;
            }
        }

        //f.SetField(game.field);
    }


    private int Lock(out ClearType ct)
    {
        if (!locked)
        {
            return game.LockMino(out ct);
        }
        else {
            ct = new ClearType();
            return 0;
        }
    }

    private void ResetLockTimer()
    {
        lockTimer = 0;
    }

    void FixedUpdate()
    {


        float TIME = GetCurrentTime();//现在的时间

        locked = false;
        

        key_left = Input.GetKey("a");//左右
        key_right = Input.GetKey("d");
        key_softdrop = Input.GetKey("s");//软降
        if (key_restart)
        {
            Restart();
        }
        if (game.Gaming())//游戏进行中
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
                if (game.Hold() == 0)
                {
                    UpdateHoldDisplay();
                    UpdateNextDisplay();
                    UpdateActiveMinoDisplay();
                }
                key_hold = false;
            }
            if (key_ccw)
            {
                if (game.CCWRotate() == 0) {
                    UpdateActiveMinoDisplay();//逆时针旋转
                    ResetLockTimer();
                }
                key_ccw = false;
            }
            if (key_cw)
            {

                if (game.CWRotate() == 0)
                {
                    UpdateActiveMinoDisplay();
                    ResetLockTimer();
                }
                //顺时针旋转

                key_cw = false;
            }

            if (key_harddrop)
            {
                ClearType ct;
                int hdCells;
                game.Harddrop(out hdCells);//硬降
                if(Lock(out ct) > 0)
                {
                    ShowClearMsg(ct.ToString());//显示消行信息
                    if (ct.GetAttack() >= 4)
                    {
                        //特效
                    }
                }
                game.NextMino();
                UpdateNextDisplay();
                UpdateFieldDisplay();
                UpdateActiveMinoDisplay();
                key_harddrop = false;
                ResetLockTimer();
                locked = true;

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
                        if (game.Move(input) == 0)
                        {
                            dasTimer = TIME;
                            UpdateActiveMinoDisplay();

                            ResetLockTimer();
                        }
                    }
                    else if (TIME - dasTimer >= DAS)//经过das延迟后移动第二下
                    {
                        dasTimer = 0;
                        arrTrigger = true;
                        if (game.Move(input) == 0)
                        {
                            UpdateActiveMinoDisplay();
                            ResetLockTimer();
                        }
                    }
                }
                else
                {
                    if (ARR == 0) //真·0ARR
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (game.Move(input) == 0)
                            {
                                ResetLockTimer();
                            }
                        }
                        UpdateActiveMinoDisplay();
                    }
                    else
                    {
                        if (arrTimer == 0)
                            arrTimer = TIME;
                        if (TIME - arrTimer >= ARR)
                        {
                            if (game.Move(input) == 0)
                            {
                                
                                ResetLockTimer();
                                UpdateActiveMinoDisplay();
                            }
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

                if (game.Fall() == 0)//如果成功下落1格
                {
                    UpdateActiveMinoDisplay();
                    ResetLockTimer();
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
                    if (!locked)
                    {
                        ClearType ct;
                        if (Lock(out ct) > 0)
                        {
                            ShowClearMsg(ct.ToString());//显示消行信息
                        }
                        //涨垃圾行
                        game.NextMino();
                        UpdateFieldDisplay();
                        UpdateActiveMinoDisplay();
                        UpdateNextDisplay();
                    }
                    ResetLockTimer();

                }
            }

        }


        if (TIME - clearMsgTimer >= 2 && clearMsgTimer != 0)
        {
            TxtClearMsg.text = "";
            clearMsgTimer = 0;
        }

        if (key_debug)
        {


            key_debug = false;
        }
    }
}
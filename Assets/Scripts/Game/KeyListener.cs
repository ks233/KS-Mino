using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    public bool playing;
    public Play play;
    //几个延迟参数的设定
    public const float LOCK_DELAY = 0.5f;//guideline规定
    public const float GRAVITY = 0.5f;
    public const float SARR = 0.001f;
    public const float DAS = 0.083f;//相当于60fps中的5帧
    public const float ARR = 0.0f;

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


    private int lastInput;
    private bool arrTrigger = false;
    private float fallTimer;
    private float lockTimer;

    private float dasTimer;
    private bool firstMove;

    private float arrTimer;

    private bool locked = false;//避免同一个FixedUpdate里调用多次LockMino


    //对于左右同时按的优化
    private bool turnback = false;
    public bool pause = false;




    private void ResetLockTimer()
    {
        lockTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        key_harddrop = Input.GetKeyDown("space");//硬降
        key_hold = Input.GetKeyDown("w");//hold
        key_restart = Input.GetKeyDown("f4");

        key_cw = Input.GetKeyDown("right");
        key_ccw = Input.GetKeyDown("left");

        key_debug = Input.GetKeyDown("b");
        if (key_restart)
        {
            play.Restart();
        }
    }
    void FixedUpdate()
    {
        if (!pause)
        {

            locked = false;
            float TIME = play.GetCurrentTime();//现在的时间
            bool activeMinoMoved = false;
            key_left = Input.GetKey("a");//左右
            key_right = Input.GetKey("d");
            key_softdrop = Input.GetKey("s");//软降

            int input = 0;
            if (key_left && key_right)
            {
                if (!turnback)
                {

                    arrTrigger = false;
                    arrTimer = 0;
                    dasTimer = 0;
                    firstMove = true;
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
                play.Hold();
                activeMinoMoved = true;
                key_hold = false;

            }
            if (key_ccw)
            {
                if (play.OP_CCWRotate() == 0)
                {
                    activeMinoMoved = true;//逆时针旋转
                    ResetLockTimer();
                }
                key_ccw = false;
            }
            if (key_cw)
            {

                if (play.OP_CWRotate() == 0)
                {
                    activeMinoMoved = true;
                    ResetLockTimer();
                }
                //顺时针旋转

                key_cw = false;
            }
            if (key_harddrop)
            {
                play.OP_HardDrop();
                activeMinoMoved = true;
                key_harddrop = false;
                ResetLockTimer();
                locked = true;
            }

            if (input != 0)
            {
                //左右移动

                if (lastInput == 0 || input == -lastInput)//反向移动或刚开始移动则重置das
                {
                    firstMove = true;
                    arrTrigger = false;
                    arrTimer = 0;
                    dasTimer = 0;
                }

                if (!arrTrigger)//如果不是arr阶段
                {

                    if (firstMove)//按下按键后移动的第一下
                    {
                        if (play.OP_Move(input) == 0)
                        {
                            firstMove = false;
                            activeMinoMoved = true;

                            ResetLockTimer();
                        }
                    }
                    else
                    {
                        if (dasTimer >= DAS)//经过das延迟后移动第二下
                        {
                            dasTimer = 0;
                            arrTrigger = true;
                            if (play.OP_Move(input) == 0)
                            {
                                activeMinoMoved = true;
                                ResetLockTimer();
                            }
                        }
                        else
                        {
                            dasTimer += Time.deltaTime;
                        }
                    }
                }
                else//如果是ARR阶段
                {/*
                if (ARR == 0) //真·0ARR
                {
                    while (play.Move(input) == 0)
                    {
                        ResetLockTimer();
                    }
                    activeMinoMoved = true;
                }
                else
                */

                    {
                        if (arrTimer == 0)
                            arrTimer = TIME;
                        if (TIME - arrTimer >= ARR)
                        {
                            if (play.OP_Move(input) == 0)
                            {

                                ResetLockTimer();
                                activeMinoMoved = true;
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
                firstMove = true;
            }
            lastInput = input;
            //自然下落or软降
            if (fallTimer >= (key_softdrop ? SARR : GRAVITY))
            {//每0.5s降落一次

                if (play.OP_Fall() == 0)//如果成功下落1格
                {
                    activeMinoMoved = true;
                    ResetLockTimer();
                }
                else
                { //如果已经到底了
                    if (lockTimer == 0) lockTimer = TIME;

                }
                fallTimer = 0;
            }
            else
            {
                fallTimer += Time.deltaTime;
            }
            if (lockTimer != 0 && !locked)
            {
                if (TIME - lockTimer >= LOCK_DELAY)
                {
                    play.OP_Lock();
                    ResetLockTimer();
                }
            }

            if (activeMinoMoved) play.UpdateActiveMinoDisplay();

        }
    }

}


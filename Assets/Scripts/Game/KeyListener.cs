using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    public bool playing;
    public Play play;
    //�����ӳٲ������趨
    public const float LOCK_DELAY = 0.5f;//guideline�涨
    public const float GRAVITY = 0.5f;
    public const float SARR = 0.001f;
    public const float DAS = 0.083f;//�൱��60fps�е�5֡
    public const float ARR = 0.0f;

    //����
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

    private bool locked = false;//����ͬһ��FixedUpdate����ö��LockMino


    //��������ͬʱ�����Ż�
    private bool turnback = false;
    public bool pause = false;




    private void ResetLockTimer()
    {
        lockTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        key_harddrop = Input.GetKeyDown("space");//Ӳ��
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
            float TIME = play.GetCurrentTime();//���ڵ�ʱ��
            bool activeMinoMoved = false;
            key_left = Input.GetKey("a");//����
            key_right = Input.GetKey("d");
            key_softdrop = Input.GetKey("s");//��

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
                    activeMinoMoved = true;//��ʱ����ת
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
                //˳ʱ����ת

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
                //�����ƶ�

                if (lastInput == 0 || input == -lastInput)//�����ƶ���տ�ʼ�ƶ�������das
                {
                    firstMove = true;
                    arrTrigger = false;
                    arrTimer = 0;
                    dasTimer = 0;
                }

                if (!arrTrigger)//�������arr�׶�
                {

                    if (firstMove)//���°������ƶ��ĵ�һ��
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
                        if (dasTimer >= DAS)//����das�ӳٺ��ƶ��ڶ���
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
                else//�����ARR�׶�
                {/*
                if (ARR == 0) //�桤0ARR
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
            else//�������Ҳ���ƶ�������das
            {
                arrTrigger = false;
                arrTimer = 0;
                dasTimer = 0;
                firstMove = true;
            }
            lastInput = input;
            //��Ȼ����or��
            if (fallTimer >= (key_softdrop ? SARR : GRAVITY))
            {//ÿ0.5s����һ��

                if (play.OP_Fall() == 0)//����ɹ�����1��
                {
                    activeMinoMoved = true;
                    ResetLockTimer();
                }
                else
                { //����Ѿ�������
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


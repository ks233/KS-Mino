using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    //Play.cs����������Ϸ���ݣ�����Ϸϵͳ����ҵ�������
    //��FixedUpdate����Ӧ���������ݰ�����Game������Ӧ�Ĳ������ٴ�Game��ȡ��Ϣ����ʾ����Ϸ�����ϡ�



    //�����ӳٲ������趨
    public const float LOCK_DELAY = 0.5f;//guideline�涨
    public const float GRAVITY = 0.5f;
    public const float SARR = 0.001f;
    public const float DAS = 0.083f;//�൱��60fps�е�5֡
    public const float ARR = 0.0f;

    public const int MAX_NEXT = 6;

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


    //��������ͬʱ�����Ż�
    private bool turnback = false;


    //public GameObject ParentField;
    public GameObject[] MinoTiles;
    public GameObject[] Minoes;


    public FieldUIDisplay fieldUI;
    public ActiveMinoUIDisplay activeMinoUI;
    public HoldUIDisplay holdUI;
    public NextUIDisplay nextUI;
    public AttackBar attackBar;
    public GameClearBoard gameClearBoard;

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


    private bool locked = false;//����ͬһ��FixedUpdate����ö��LockMino

    private bool gameover = false;

    private float GetCurrentTime()
    {
        System.TimeSpan time = DateTime.Now.TimeOfDay;//������ȷ��������
        float t = (float)time.TotalSeconds;//���ڵ�ʱ��
        return t;
    }


    public void UpdateStats()
    {
        int piece = game.statPiece;
        int atk = game.statAttack;

        String s = String.Format("Pieces\n<size=32>{0} </size>{1:0.00}/SEC\n\nAttacks\n<size=32>{2} </size>{3:0.00}/Min\n\nTime\n<size=32>{4:0.00}</size>",
            piece, piece / gameTime, atk, atk / gameTime * 60, gameTime);
        TxtStats.text = s;
    }

    private void UpdateActiveMinoDisplay()
    {
        activeMinoUI.UpdateActiveMino(game.field, game.activeMino);

    }


    private void DestroyAllChild(GameObject parent)
    {//ɾ������child object
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void UpdateHoldDisplay()
    {
        int holdId = game.GetHoldId();
        holdUI.UpdateHold(holdId);
    }

    private void UpdateNextDisplay()
    {
        int[] nextSeq = game.GetNextSeq();
        nextUI.UpdateNext(nextSeq, MAX_NEXT);
    }

    public void UpdateFieldDisplay()
    {
        fieldUI.UpdateField(game.field, game.activeMino);
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


        float TIME = GetCurrentTime();//���ڵ�ʱ��

        if (!PauseMenu.GameIsPaused)
        {
            gameTime += Time.deltaTime;
        }
        key_harddrop = Input.GetKeyDown("space");//Ӳ��
        key_hold = Input.GetKeyDown("w");//hold
        key_restart = Input.GetKeyDown("f4");

        key_cw = Input.GetKeyDown("right");
        key_ccw = Input.GetKeyDown("left");

        key_debug = Input.GetKeyDown("b");
        if (game.Gaming())
        {

            UpdateStats();
        }
        else
        {
            if (!gameover)
            {
                int[,] field = game.GetFieldArray();

                string s = "";


                //InstChild(Minoes[activeMinoId-1], tmp, ParentField,1,0.6f);

                //����
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
        else
        {
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


        float TIME = GetCurrentTime();//���ڵ�ʱ��

        locked = false;

        bool activeMinoMoved = false;

        key_left = Input.GetKey("a");//����
        key_right = Input.GetKey("d");
        key_softdrop = Input.GetKey("s");//��
        if (key_restart)
        {
            Restart();
        }
        if (game.Gaming())//��Ϸ������
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
                    activeMinoMoved = true;
                }
                key_hold = false;
            }
            if (key_ccw)
            {
                if (game.CCWRotate() == 0)
                {

                    activeMinoMoved = true;//��ʱ����ת
                    ResetLockTimer();
                }
                key_ccw = false;
            }
            if (key_cw)
            {

                if (game.CWRotate() == 0)
                {
                    activeMinoMoved = true;
                    ResetLockTimer();
                }
                //˳ʱ����ת

                key_cw = false;
            }

            if (key_harddrop)
            {
                ClearType ct;
                int hdCells;
                game.Drop(out hdCells);//Ӳ��
                if (Lock(out ct) > 0)
                {
                    ShowClearMsg(ct.ToString());//��ʾ������Ϣ

                    attackBar.AddAttack(ct.GetAttack());

                    if (ct.GetAttack() >= 4)
                    {
                        //��Ч
                    }
                }
                game.NextMino();
                UpdateNextDisplay();
                UpdateFieldDisplay();
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
                    arrTrigger = false;
                    arrTimer = 0;
                    dasTimer = 0;
                }

                if (!arrTrigger)//�������arr�׶�
                {

                    if (dasTimer == 0)//���°������ƶ��ĵ�һ��
                    {
                        if (game.Move(input) == 0)
                        {
                            dasTimer = TIME;
                            activeMinoMoved = true;

                            ResetLockTimer();
                        }
                    }
                    else if (TIME - dasTimer >= DAS)//����das�ӳٺ��ƶ��ڶ���
                    {
                        dasTimer = 0;
                        arrTrigger = true;
                        if (game.Move(input) == 0)
                        {
                            activeMinoMoved = true;
                            ResetLockTimer();
                        }
                    }
                }
                else
                {
                    if (ARR == 0) //�桤0ARR
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (game.Move(input) == 0)
                            {
                                ResetLockTimer();
                            }
                        }
                        activeMinoMoved = true;
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
            }
            lastInput = input;
            //��Ȼ����or��
            if (TIME - fallTimer >= (key_softdrop ? SARR : GRAVITY))
            {//ÿ0.5s����һ��

                if (game.Fall() == 0)//����ɹ�����1��
                {
                    activeMinoMoved = true;
                    ResetLockTimer();
                }
                else
                { //����Ѿ�������
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
                            ShowClearMsg(ct.ToString());//��ʾ������Ϣ
                        }
                        attackBar.AddAttack(ct.GetAttack());
                        //��������
                        game.NextMino();
                        UpdateFieldDisplay();
                        activeMinoMoved = true;
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
        if (activeMinoMoved) UpdateActiveMinoDisplay();
    }
}
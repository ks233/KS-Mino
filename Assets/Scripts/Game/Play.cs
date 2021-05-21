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

        String s = String.Format("ʱ�䣺{0:0.00}\n������{1}\n���У�{2}\n������{3}\n", gameTime, piece, game.statLine, atk);
        s += String.Format("PPS��{0:0.00}\nAPM��{1:0.00}\n", piece / gameTime, atk / gameTime * 60);

        TxtStats.text = s;
    }

    private void UpdateActiveMinoDisplay()
    {
        DestroyAllChild(ActiveMinoParent);
        Mino tmpMino = game.GetActiveMino();

        //����
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
    {//ɾ������child object
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

        //����
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
        float TIME = GetCurrentTime();//���ڵ�ʱ��
        if (PauseMenu.GameIsPaused)
        {
            prevFrameTime = TIME;
        }
        else
        {
            gameTime += TIME - prevFrameTime;
            prevFrameTime = TIME;
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
        else {
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


        float TIME = GetCurrentTime();//���ڵ�ʱ��

        locked = false;
        

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
                    UpdateActiveMinoDisplay();
                }
                key_hold = false;
            }
            if (key_ccw)
            {
                if (game.CCWRotate() == 0) {
                    UpdateActiveMinoDisplay();//��ʱ����ת
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
                //˳ʱ����ת

                key_cw = false;
            }

            if (key_harddrop)
            {
                ClearType ct;
                int hdCells;
                game.Harddrop(out hdCells);//Ӳ��
                if(Lock(out ct) > 0)
                {
                    ShowClearMsg(ct.ToString());//��ʾ������Ϣ
                    if (ct.GetAttack() >= 4)
                    {
                        //��Ч
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
                            UpdateActiveMinoDisplay();

                            ResetLockTimer();
                        }
                    }
                    else if (TIME - dasTimer >= DAS)//����das�ӳٺ��ƶ��ڶ���
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
                    if (ARR == 0) //�桤0ARR
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
                    UpdateActiveMinoDisplay();
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
                        //��������
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
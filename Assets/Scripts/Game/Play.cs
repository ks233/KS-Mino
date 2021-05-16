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
    public const float LOCK_DELAY = 1f;
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

    //��������ͬʱ�����Ż�
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
        System.TimeSpan time = DateTime.Now.TimeOfDay;//������ȷ��������
        float t = (float)time.TotalSeconds;//���ڵ�ʱ��
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

        String s = String.Format("ʱ�䣺{0:0.00}\n������{1}\n���У�{2}\n������{3}\n", GetGameTime(),piece, GAME.statLine, atk);
        s += String.Format("PPS��{0:0.00}\nAPM��{1:0.00}\n", piece/gameTime, atk/gameTime*60);

        TxtStats.text = s;
    }


    private void InstChild(GameObject child,Vector3 position,GameObject parent) //ʵ���������prefab�����飩
    {
        Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);

    }

    private void InstChild(GameObject child, Vector3 position, GameObject parent,float scale)//ʵ����prefab�������ţ�hold/next��
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale,scale,scale);
    }

    private void InstChild(GameObject child, Vector3 position, GameObject parent, float scale,float alpha)//ʵ����prefab�������ź�͸���ȣ�ש����Ӱ��
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale, scale, scale);
        Color tmp = t.GetComponent<SpriteRenderer>().color;
        tmp.a = alpha;
        t.GetComponent<SpriteRenderer>().color = tmp;
    }

    private void DestroyAllChild(GameObject parent) {//ɾ������child object
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
                if (field[x, y] > 0 && field[x, y] < 20)//���������
                {
                    InstChild(MinoTiles[field[x, y] - 1], new Vector3(x, y, 0),ParentField);
                }
                if (field[x, y] < 0)//�Ѿ�������
                {
                    InstChild(MinoTiles[-field[x, y] - 1], new Vector3(x, y, 0), ParentField);
                }
                if (field[x, y] > 20)//שӰ
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

        key_harddrop = Input.GetKeyDown("space");//Ӳ��
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
        float TIME = GetCurrentTime();//���ڵ�ʱ��
        key_left = Input.GetKey("a");//����
        key_right = Input.GetKey("d");
        key_softdrop = Input.GetKey("s");//��
        if (key_restart)
        {
            GAME.Restart();
            startTime = TIME;
        }
        if (GAME.Gaming())//��Ϸ������
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
                GAME.CCWRotate();//��ʱ����ת
                key_ccw = false;
            }
            if (key_cw)
            {
                GAME.CWRotate();
                //˳ʱ����ת

                key_cw = false;
            }

            if (key_harddrop)
            {
                GAME.HardDrop();//Ӳ��
                key_harddrop = false;
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
                        GAME.Move(input);
                        dasTimer = TIME;
                    }
                    else if (TIME - dasTimer >= DAS)//����das�ӳٺ��ƶ��ڶ���
                    {
                        dasTimer = 0;
                        arrTrigger = true;
                        GAME.Move(input);
                    }
                }
                else
                {
                    if (ARR == 0) //�桤0ARR
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

                if (GAME.Fall() == 0)//����ɹ�����1��
                {
                    lockTimer = 0;
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


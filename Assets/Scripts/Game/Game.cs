using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.XR;
using UnityEngine.Assertions.Must;

public class Game : MonoBehaviour
{

    //public Text t;
    public float LOCK_DELAY = 1f;
    public float GRAVITY = 0.5f;
    public float SARR = 0.1f;
    public float DAS = 0.083f;
    public float ARR = 0.016f;
    public Vector2Int[,] OFFSET_3x3;
    public Vector2Int[,] OFFSET_I;
    private bool left, right, cw, ccw, down, harddrop, ho, restart;
    private bool turnback = false;



    public InputField nextSeq;
    public Text clearMsg;

    public FieldVisualization f;

    [HideInInspector]
    public NextManager next;

    //public int[,] field = new int[10, 40];//10*20的盘面，预留20行，避免c4w玩家被顶到溢出（
    public Field field = new Field();
    private Mino currentMino;


    ClearType ct = new ClearType();
    private bool wasB2B = false;
    private int combo = 0;

    private Mino S, Z, L, J, T, O, I;

    private float fallTimer;
    private float lockTimer;
    private float dasTimer;
    private float arrTimer;


    private float clearMsgTimer;

    private int lastInput;
    private bool arrTrigger = false;

    private int count = 0;

    [HideInInspector]
    public bool gameover = false;

    [HideInInspector]
    public int hold = 0;
    [HideInInspector]
    public bool isHeld;

    public Toggle useCustomNext;


    void RestartGame()
    {
        count = 0;
        gameover = false;

        //初始化40行10列的盘面
        field.ClearAll();
        hold = 0;
        isHeld = false;
        wasB2B = false;
        combo = 0;

        next = new NextManager();
        next.setUseCustomNext(useCustomNext.isOn);
        next.SetNextSeq(nextSeq.text);
        next.ResetNext();
        GenerateMino(next.Dequeue());

        f.UpdateNextTilemap(next);
        f.UpdateHoldTilemap(hold);
    }

    // Start is called before the first frame update
    void Start()
    {
        //t = GetComponent<Text>();


        //3*3方块的踢墙表
        OFFSET_3x3 = new Vector2Int[5, 4];
        OFFSET_3x3[0, 0] = Vector2Int.zero;
        OFFSET_3x3[0, 1] = Vector2Int.zero;
        OFFSET_3x3[0, 2] = Vector2Int.zero;
        OFFSET_3x3[0, 3] = Vector2Int.zero;

        OFFSET_3x3[1, 0] = Vector2Int.zero;
        OFFSET_3x3[1, 1] = new Vector2Int(1, 0);
        OFFSET_3x3[1, 2] = Vector2Int.zero;
        OFFSET_3x3[1, 3] = new Vector2Int(-1, 0);

        OFFSET_3x3[2, 0] = Vector2Int.zero;
        OFFSET_3x3[2, 1] = new Vector2Int(1, -1);
        OFFSET_3x3[2, 2] = Vector2Int.zero;
        OFFSET_3x3[2, 3] = new Vector2Int(-1, -1);


        OFFSET_3x3[3, 0] = Vector2Int.zero;
        OFFSET_3x3[3, 1] = new Vector2Int(0, 2);
        OFFSET_3x3[3, 2] = Vector2Int.zero;
        OFFSET_3x3[3, 3] = new Vector2Int(0, 2);

        OFFSET_3x3[4, 0] = Vector2Int.zero;
        OFFSET_3x3[4, 1] = new Vector2Int(1, 2);
        OFFSET_3x3[4, 2] = Vector2Int.zero;
        OFFSET_3x3[4, 3] = new Vector2Int(-1, 2);

        //I块的踢墙表
        OFFSET_I = new Vector2Int[5, 4];
        OFFSET_I[0, 0] = Vector2Int.zero;
        OFFSET_I[0, 1] = new Vector2Int(-1, 0);
        OFFSET_I[0, 2] = new Vector2Int(-1, 1);
        OFFSET_I[0, 3] = new Vector2Int(0, 1);

        OFFSET_I[1, 0] = new Vector2Int(-1, 0);
        OFFSET_I[1, 1] = Vector2Int.zero;
        OFFSET_I[1, 2] = new Vector2Int(1, 1);
        OFFSET_I[1, 3] = new Vector2Int(0, 1);

        OFFSET_I[2, 0] = new Vector2Int(2, 0);
        OFFSET_I[2, 1] = Vector2Int.zero;
        OFFSET_I[2, 2] = new Vector2Int(-2, 1);
        OFFSET_I[2, 3] = new Vector2Int(0, 1);


        OFFSET_I[3, 0] = new Vector2Int(-1, 0);
        OFFSET_I[3, 1] = new Vector2Int(0, 1);
        OFFSET_I[3, 2] = new Vector2Int(1, 0);
        OFFSET_I[3, 3] = new Vector2Int(0, -1);

        OFFSET_I[4, 0] = new Vector2Int(2, 0);
        OFFSET_I[4, 1] = new Vector2Int(0, -2);
        OFFSET_I[4, 2] = new Vector2Int(-2, 0);
        OFFSET_I[4, 3] = new Vector2Int(0, 2);


        RestartGame();
    }








    void GenerateMino(int minoId)
    {
        if (minoId != 0)
        {
            currentMino = new Mino(minoId);

            currentMino.position = new Vector2Int(4, 20);
            count++;
            if (!field.IsValid(currentMino, currentMino.position)) gameover = true;
        }
        else
        {
            gameover = true;
        }
    }

    public static int[,] RotateMatrix(int[,] matrix, int n)
    {
        int[,] ret = new int[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = matrix[n - j - 1, i];
            }
        }

        return ret;
    }

    Vector2Int WallKickOffset(Mino mino, int A, int B, Vector2Int coordinate)//A和B都是rotationId
    {
        int size = mino.size;

        if (size == 3)
        {
            for (int i = 0; i < 5; i++)
            {
                //Debug.Log("A:" + A + "  B:" + B);
                //Debug.Log(coordinate + OFFSET_3x3[i, A] - OFFSET_3x3[i, B] + " " + i);

                if (field.IsValid(mino, B, coordinate + OFFSET_3x3[i, A] - OFFSET_3x3[i, B]))
                    return OFFSET_3x3[i, A] - OFFSET_3x3[i, B];
            }
        }
        else if (size == 5)
        {
            for (int i = 0; i < 5; i++)
            {

                if (field.IsValid(mino, B, coordinate + OFFSET_I[i, A] - OFFSET_I[i, B]))
                    return OFFSET_I[i, A] - OFFSET_I[i, B];
            }
        }
        return new Vector2Int(0, 0);
    }

    void CCWRotate()//逆时针旋转
    {
        int size = currentMino.size;
        int newRotationId = (currentMino.rotation + 3) % 4;

        if (size == 2) lockTimer = 0;
        if (size == 3)
        {
            if (field.IsValid(currentMino, newRotationId, currentMino.position))
            {

                currentMino.rotation = newRotationId;
                currentMino.array = RotateMatrix(currentMino.array, size);
                lockTimer = 0;
            }
            else
            {
                Vector2Int o = WallKickOffset(currentMino, currentMino.rotation, newRotationId, currentMino.position);
                if (o != new Vector2Int(0, 0))
                {
                    currentMino.rotation = newRotationId;
                    currentMino.array = RotateMatrix(currentMino.array, size);
                    currentMino.position += o;
                    lockTimer = 0;
                    field.kicked = true;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(currentMino, currentMino.rotation, newRotationId, currentMino.position);
            if (o != new Vector2Int(0, 0))
            {
                currentMino.rotation = newRotationId;
                currentMino.array = RotateMatrix(currentMino.array, size);
                currentMino.position += o;
                lockTimer = 0;
            }


        }

    }

    void CWRotate()//逆时针旋转
    {
        int size = currentMino.size;
        int newRotationId = (currentMino.rotation + 1) % 4;
        if (size == 2) lockTimer = 0;
        if (size == 3)
        {
            if (field.IsValid(currentMino, newRotationId, currentMino.position))
            {
                currentMino.rotation = newRotationId;
                currentMino.array = RotateMatrix(currentMino.array, size);
                currentMino.array = RotateMatrix(currentMino.array, size);
                currentMino.array = RotateMatrix(currentMino.array, size);
                lockTimer = 0;
            }
            else
            {
                Vector2Int o = WallKickOffset(currentMino, currentMino.rotation, newRotationId, currentMino.position);
                if (o != new Vector2Int(0, 0))
                {
                    currentMino.rotation = newRotationId;
                    currentMino.array = RotateMatrix(currentMino.array, size);
                    currentMino.array = RotateMatrix(currentMino.array, size);
                    currentMino.array = RotateMatrix(currentMino.array, size);
                    currentMino.position += o;
                    lockTimer = 0;
                    field.kicked = true;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(currentMino, currentMino.rotation, newRotationId, currentMino.position);
            if (o != new Vector2Int(0, 0))
            {
                currentMino.rotation = newRotationId;
                currentMino.array = RotateMatrix(currentMino.array, size);
                currentMino.array = RotateMatrix(currentMino.array, size);
                currentMino.array = RotateMatrix(currentMino.array, size);
                currentMino.position += o;
                lockTimer = 0;
            }

        }
    }
    void Move(int x)
    {
        Vector2Int newPosition = currentMino.position;
        newPosition.x += x;
        if (field.IsValid(currentMino, currentMino.rotation, newPosition)) currentMino.Move(x);
        lockTimer = 0;
    }


    List<Vector2Int> GetAllCoordinates()//mino四个格子的坐标
    {
        List<Vector2Int> l = new List<Vector2Int>();
        int size = currentMino.size;
        if (size == 3)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (currentMino.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 1 + currentMino.position.x, j - 1 + currentMino.position.y));

        }
        else if (size == 5)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (currentMino.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 2 + currentMino.position.x, j - 2 + currentMino.position.y));

        }

        else if (size == 2)
        {

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    l.Add(new Vector2Int(i + currentMino.position.x, j + currentMino.position.y));
        }
        return l;
    }



    void LockMino()
    {

        field.LockMino(currentMino);
        ct = new ClearType();
        ct.wasB2b = wasB2B;
        //此时方块的位置和旋转都已经固定，开始消行性质检定
        int lines = field.LinesCanClear(currentMino);
        ct.lines = lines;
        ct.pc = field.IsAllClear();
        if (lines > 0)
        {

            if (currentMino.name == "T")//tspin
            {
                bool c, f, k;
                c = field.HasThreeCorners(currentMino);
                f = field.HasTwoFeets(currentMino);
                k = field.kicked;
                if (lines == 1)
                {
                    bool cannotGoUp = !field.IsValid(currentMino, new Vector2Int(currentMino.position.x, currentMino.position.y + 1));
                    Debug.Log(cannotGoUp);
                    if (c && f && cannotGoUp)//t-spin single
                    {
                        ct.tSpin = true;
                        ct.tSpinType = 1;
                    }
                    else if ((c && !f && cannotGoUp) || (c && !f && k))//t-spin mini
                    {
                        ct.tSpin = true;
                        ct.tSpinType = 0;

                    }
                }
                else if (lines == 2 && c)//t-spin double
                {
                    ct.tSpin = true;
                    ct.tSpinType = 2;
                }
                else if (lines == 3)
                {
                    ct.tSpin = true;
                    ct.tSpinType = 3;
                }
            }
            else
            {
                ct.tSpin = false;
            }
            wasB2B = ct.GetIsB2b();
            field.Clear(currentMino);
            clearMsg.text = ct.ClearMessage();

            TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
            float timeNow = (float)time.TotalSeconds;//现在的时间
            clearMsgTimer = timeNow;
        }
        dasTimer = 0;
        arrTimer = 0;
        arrTrigger = false;
        isHeld = false;
    }


    void hardDrop()
    {
        Vector2Int newPosition = currentMino.position;
        newPosition.y--;
        while (field.IsValid(currentMino, currentMino.rotation, newPosition))
        {
            currentMino.Fall();

            newPosition.y--;
        }

        LockMino();
        lockTimer = 0;
        GenerateMino(next.Dequeue());

        f.UpdateNextTilemap(next);
        //GenerateMino(6);
    }

    void Hold()
    {
        if (hold == 0)
        {
            int t = currentMino.id;
            GenerateMino(next.Dequeue());

            f.UpdateNextTilemap(next);
            hold = t;
        }
        else
        {
            int t = currentMino.id;
            GenerateMino(hold);
            hold = t;
        }
        isHeld = true;
        f.UpdateHoldTilemap(hold);
    }

    void Update()
    {

        harddrop = Input.GetKeyDown("space");//硬降
        cw = Input.GetKeyDown("right");
        ccw = Input.GetKeyDown("left");
        ho = Input.GetKeyDown("w");//hold
        restart = Input.GetKeyDown("f4");



        field.RefreshField(currentMino);
        f.SetField(field);
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
        float timeNow = (float)time.TotalSeconds;//现在的时间
        left = Input.GetKey("a");//左右
        right = Input.GetKey("d");
        down = Input.GetKey("s");//软降

        int input = 0;
        if (left && right)
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

            if (left) input = -1;
            if (right) input = 1;
            turnback = false;
        }
        if (ho && !isHeld)
        {
            Hold();

        }
        if (restart)
        {
            RestartGame();
        }


        if (!gameover)
        {
            if (ccw)
            {
                CCWRotate();//逆时针旋转
                ccw = false;
            }
            if (cw)
            {
                CWRotate();
                //顺时针旋转
                cw = false;
            }

            if (harddrop)
            {
                hardDrop();//硬降
                harddrop = false;
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
                        Move(input);
                        dasTimer = timeNow;
                    }
                    else if (timeNow - dasTimer >= DAS)//经过das延迟后移动第二下
                    {
                        dasTimer = 0;
                        arrTrigger = true;
                        Move(input);
                    }
                }
                else
                {
                    if (arrTimer == 0)
                        arrTimer = timeNow;
                    if (timeNow - arrTimer >= ARR)
                    {
                        Move(input);
                        arrTimer = timeNow;
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
            if (timeNow - fallTimer >= (down ? SARR : GRAVITY))
            {//每0.5s降落一次
                if (field.IsValid(currentMino, currentMino.rotation, new Vector2Int(currentMino.position.x, currentMino.position.y - 1)))
                {
                    currentMino.Fall();
                    lockTimer = 0;
                }
                else
                {
                    if (lockTimer == 0)
                        lockTimer = timeNow;
                    if (timeNow - lockTimer >= LOCK_DELAY)
                    {
                        LockMino();
                        lockTimer = 0;
                        GenerateMino(next.Dequeue());

                        f.UpdateNextTilemap(next);
                        //GenerateMino(6);
                    }
                }

                fallTimer = timeNow;
            }


            if (timeNow - clearMsgTimer >= 2 && clearMsgTimer!=0)
            {
                clearMsg.text = "";
                clearMsgTimer = 0;
            }
        }
        /*
        t.text = "";

        int[] nextArr = nextQueue.ToArray();
        int n = 0;
        foreach (int i in nextArr)
        {
            t.text += minoDic[i];
            n++;
            if (n > 4) break;
            t.text += ",  ";
        }
        t.text += "\r\n";
        t.text += "HOLD:";
        if (hold != 0)
            t.text += minoDic[hold];

        t.text += "\r\n";
        string str = "", s = "";
        for (int i = 19; i >= 0; i--)
        {
            for (int j = 0; j < 10; j++)
            {
                if (field[j, i] > 0 && field[j, i] < 20)
                {
                    s = "回";
                }
                else if (field[j, i] < 0)
                {
                    s = "回";
                }
                else if (field[j, i] > 20)
                {
                    s = "囚";
                }
                else
                {
                    s = "口";
                }

                str += s;

            }
            t.text += str + "\r\n";
            str = "";
        }*/
    }
}

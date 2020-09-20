using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Game : MonoBehaviour
{

    public Text t;
    public float LOCK_DELAY = 1f;
    public float GRAVITY = 0.5f;
    public float SARR = 0.1f;
    public float DAS = 0.083f;
    public float ARR = 0.016f;
    public Vector2Int[,] OFFSET_3x3;
    public Vector2Int[,] OFFSET_I;

    struct Mino
    {
        public int[,] array;
        public int size;
        public string name;
        public int rotation;
        public int id;
    };

    public int[] bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };//7bag机制

    public Queue<int> nextQueue = new Queue<int>();

    public int[,] field = new int[10, 40];//10*20的盘面，预留20行，避免c4w玩家被顶到溢出（

    private Mino minoNow;

    private Vector2Int minoCoordinate = new Vector2Int(0, 0);//旋转中心的位置


    private bool isSpin = false;
    private bool isB2B = false;
    private int combo = 0;

    private Mino S, Z, L, J, T, O, I;

    private float fallTimer;
    private float lockTimer;
    private float dasTimer;
    private float arrTimer;
    private float lastInput;
    private bool arrTrigger = false;

    private int count = 0;
    public bool gameover = false;
    private int shadowDistance;
    Dictionary<int, string> minoDic = new Dictionary<int, string>();

    public int hold = 0;
    public bool isHeld;


    void RestartGame()
    {
        count = 0;
        gameover = false;

        //初始化40行10列的盘面
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                field[i, j] = 0;
            }
        }
        hold = 0;
        isHeld = false;
        bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        Shuffle(bag);
        nextQueue = new Queue<int>();
        updateNext();
        updateNext();
        generateMino(nextQueue.Dequeue());
    }

    // Start is called before the first frame update
    void Start()
    {
        //t = GetComponent<Text>();

        S.array = new int[,] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
        S.size = 3;
        S.name = "S";
        S.rotation = 0;
        S.id = 1;

        Z.array = new int[,] { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
        Z.size = 3;
        Z.name = "Z";
        Z.rotation = 0;
        Z.id = 2;

        L.array = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 1 } };
        L.size = 3;
        L.name = "L";
        L.rotation = 0;
        L.id = 3;

        J.array = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 0, 0 } };
        J.size = 3;
        J.name = "J";
        J.rotation = 0;
        J.id = 4;

        T.array = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
        T.size = 3;
        T.name = "T";
        T.rotation = 0;
        T.id = 5;

        O.array = new int[,] { { 1, 1 }, { 1, 1 } };
        O.size = 2;
        O.name = "O";
        O.rotation = 0;
        O.id = 6;

        I.array = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
        I.size = 5;
        I.name = "I";
        I.rotation = 0;
        I.id = 7;

        minoDic.Add(1, "S");
        minoDic.Add(2, "Z");
        minoDic.Add(3, "L");
        minoDic.Add(4, "J");
        minoDic.Add(5, "T");
        minoDic.Add(6, "O");
        minoDic.Add(7, "I");

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




    void updateNext()
    {

        Shuffle(bag);
        for (int i = 0; i < 7; i++)
            nextQueue.Enqueue(bag[i]);
    }


    void Shuffle(int[] deck)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            int temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(0, deck.Length);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    bool isValid(int minoId, int rotationId, Vector2Int coordinate, int size)
    {
        int[,] testMino;
        testMino = new int[size, size];
        switch (minoId)
        {
            case 1: testMino = S.array; break;
            case 2: testMino = Z.array; break;
            case 3: testMino = L.array; break;
            case 4: testMino = J.array; break;
            case 5: testMino = T.array; break;
            case 6: testMino = O.array; break;
            case 7: testMino = I.array; break;
        }
        for (int i = 0; i < (4 - rotationId) % 4; i++)
        {
            testMino = RotateMatrix(testMino, size);
        }
        if (1 <= minoId && minoId <= 5)
        {



            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (testMino[j, i] == 1)
                    {
                        if (coordinate.x + i - 1 > 9 || coordinate.x + i - 1 < 0) return false;
                        if (coordinate.y + j - 1 < 0) return false;
                        if (field[coordinate.x + i - 1, coordinate.y + j - 1] < 0) return false;
                    }
                }
            }
        }
        if (minoId == 6)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (coordinate.x + i > 9 || coordinate.x + i < 0) return false;
                    if (coordinate.y < 0) return false;
                    if (field[coordinate.x + i, coordinate.y + j] < 0) return false;
                }
            }
        }
        if (minoId == 7)
        {
            int x, y;
            x = coordinate.x;
            y = coordinate.y;
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if (testMino[j + 2, i + 2] == 1)
                    {
                        if (x + i > 9 || x + i < 0) return false;
                        if (y + j < 0) return false;
                        if (field[x + i, y + j] < 0) return false;
                    }
                }
            }
        }
        return true;
    }

    void generateMino(int mino)
    {
        switch (mino)
        {
            case 1: minoNow = S; break;
            case 2: minoNow = Z; break;
            case 3: minoNow = L; break;
            case 4: minoNow = J; break;
            case 5: minoNow = T; break;
            case 6: minoNow = O; break;
            case 7: minoNow = I; break;
        }
        if (nextQueue.Count <= 7) updateNext();

        minoCoordinate = new Vector2Int(4, 19);
        count++;
        if (!isValid(minoNow.id, 0, minoCoordinate, minoNow.size)) gameover = true;

    }

    static int[,] RotateMatrix(int[,] matrix, int n)
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

    Vector2Int WallKickOffset(int minoId, int A, int B, Vector2Int coordinate, int size)//A和B都是rotationId
    {
        if (size == 3)
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.Log("A:" + A + "  B:" + B);
                Debug.Log(coordinate + OFFSET_3x3[i, A] - OFFSET_3x3[i, B] + " " + i);

                if (isValid(minoId, B, coordinate + OFFSET_3x3[i, A] - OFFSET_3x3[i, B], size))
                    return OFFSET_3x3[i, A] - OFFSET_3x3[i, B];
            }
        }
        else if (size == 5)
        {
            for (int i = 0; i < 5; i++)
            {

                if (isValid(minoId, B, coordinate + OFFSET_I[i, A] - OFFSET_I[i, B], size))
                    return OFFSET_I[i, A] - OFFSET_I[i, B];
            }
        }
        return new Vector2Int(0, 0);
    }

    void CCWRotate()//逆时针旋转
    {
        int size = minoNow.size;
        int newRotationId = (minoNow.rotation + 3) % 4;

        if (size == 2) lockTimer = 0;
        if (size == 3)
        {
            if (isValid(minoNow.id, newRotationId, minoCoordinate, minoNow.size))
            {

                minoNow.rotation = newRotationId;
                minoNow.array = RotateMatrix(minoNow.array, size);
                lockTimer = 0;
                isSpin = false;
            }
            else
            {
                Vector2Int o = WallKickOffset(minoNow.id, minoNow.rotation, newRotationId, minoCoordinate, minoNow.size);
                if (o != new Vector2Int(0, 0))
                {
                    minoNow.rotation = newRotationId;
                    minoNow.array = RotateMatrix(minoNow.array, size);
                    minoCoordinate += o;
                    lockTimer = 0;
                    isSpin = true;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(minoNow.id, minoNow.rotation, newRotationId, minoCoordinate, minoNow.size);
            if (o != new Vector2Int(0, 0))
            {
                minoNow.rotation = newRotationId;
                minoNow.array = RotateMatrix(minoNow.array, size);
                minoCoordinate += o;
                lockTimer = 0;
            }


        }

    }

    void CWRotate()//逆时针旋转
    {
        int size = minoNow.size;
        int newRotationId = (minoNow.rotation + 1) % 4;
        if(size == 2) lockTimer = 0;
        if (size == 3)
        {
            if (isValid(minoNow.id, newRotationId, minoCoordinate, minoNow.size))
            {
                minoNow.rotation = newRotationId;
                minoNow.array = RotateMatrix(minoNow.array, size);
                minoNow.array = RotateMatrix(minoNow.array, size);
                minoNow.array = RotateMatrix(minoNow.array, size);
                lockTimer = 0;
            }
            else
            {
                Vector2Int o = WallKickOffset(minoNow.id, minoNow.rotation, newRotationId, minoCoordinate, minoNow.size);
                if (o != new Vector2Int(0, 0))
                {
                    minoNow.rotation = newRotationId;
                    minoNow.array = RotateMatrix(minoNow.array, size);
                    minoNow.array = RotateMatrix(minoNow.array, size);
                    minoNow.array = RotateMatrix(minoNow.array, size);
                    minoCoordinate += o;
                    lockTimer = 0;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(minoNow.id, minoNow.rotation, newRotationId, minoCoordinate, minoNow.size);
            if (o != new Vector2Int(0, 0))
            {
                minoNow.rotation = newRotationId;
                minoNow.array = RotateMatrix(minoNow.array, size);
                minoNow.array = RotateMatrix(minoNow.array, size);
                minoNow.array = RotateMatrix(minoNow.array, size);
                minoCoordinate += o;
                lockTimer = 0;
            }

        }
    }

    void Fall()
    {
        minoCoordinate.y--;
    }

    void Move(int x)
    {
        if (isValid(minoNow.id, minoNow.rotation, new Vector2Int(minoCoordinate.x + x, minoCoordinate.y), minoNow.size)) minoCoordinate.x += x;
        lockTimer = 0;
    }

    bool TouchGround()//判断方块是否触地
    {

        List<Vector2Int> l = GetAllCoordinates();
        foreach (Vector2Int pos in l)
        {
            if (pos.y <= 0) return true;
            if (field[pos.x, pos.y - 1] < 0) return true;
        }
        return false;
    }

    void RefreshField()
    {//在确定新的方块坐标和方向后，刷新field中的元素

        for (int i = 0; i < 10; i++)//先清除地形以外的元素
        {
            for (int j = 0; j < 40; j++)
            {
                if (field[i, j] > 0)
                {
                    field[i, j] = 0;
                }
            }

        }
        shadowDistance = 0;//阴影距离
        int size = minoNow.size;
        bool shadowTouchGround = false;
        List<Vector2Int> l = GetAllCoordinates();
        while (!shadowTouchGround)//方块下方阴影只要有一块即将重叠，就停止增加阴影距离
        {
            foreach (Vector2Int pos in l)
            {
                if (pos.y - shadowDistance <= 0)
                {
                    shadowTouchGround = true;
                    break;
                }
                if (field[pos.x, pos.y - shadowDistance - 1] < 0)
                {
                    shadowTouchGround = true;
                    break;
                }
            }
            if (!shadowTouchGround)
                shadowDistance++;
        }
        foreach (Vector2Int pos in l)//写入方块和阴影
        {

            field[pos.x, pos.y - shadowDistance] = 20 + minoNow.id;
            field[pos.x, pos.y] = minoNow.id;
        }

    }

    List<Vector2Int> GetAllCoordinates()//mino四个格子的坐标
    {
        List<Vector2Int> l = new List<Vector2Int>();
        int size = minoNow.size;
        if (size == 3)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (minoNow.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 1 + minoCoordinate.x, j - 1 + minoCoordinate.y));

        }
        else if (size == 5)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (minoNow.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 2 + minoCoordinate.x, j - 2 + minoCoordinate.y));

        }

        else if (size == 2)
        {

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    l.Add(new Vector2Int(i + minoCoordinate.x, j + minoCoordinate.y));
        }
        return l;
    }

    void clearLine(int x)
    {
        for (int i = x; i < 39; i++)
        {
            for (int j = 0; j < 10; j++)
                field[j, i] = field[j, i + 1];
        }

    }

    void LockBlock()
    {
        List<Vector2Int> l = GetAllCoordinates();
        foreach (Vector2Int pos in l)
        {
            field[pos.x, pos.y] = -minoNow.id;
        }

        int fulllines = 0;

        for (int i = 0; i < 40; i++)
        {
            bool full = true;
            for (int j = 0; j < 10; j++)
            {
                if (field[j, i] >= 0) full = false;
            }

            if (full)
            {
                clearLine(i);
                fulllines++;
                i--;
            }
        }
        
        isHeld = false;
    }

    string ClearLineMessage(String minoName,bool isSpin,bool isB2B,int clearLineNumber)
    {
        string result = "";

        string a = "";
        switch (clearLineNumber)
        {
            case 1: a = "Single"; break;
            case 2: a = "Double"; break;
            case 3: a = "Triple"; break;
            case 4: a = "④"; break;
        }
        if (isSpin)
        {
            result += minoName + " Spin";
        }
        result += a;
        return result;
    }

    void hardDrop()
    {
        while (!TouchGround())
        {
            Fall();
        }

        LockBlock();
        lockTimer = 0;
        generateMino(nextQueue.Dequeue());
        //generateMino(6);
    }

    void Hold()
    {
        if (hold == 0)
        {
            int t = minoNow.id;
            generateMino(nextQueue.Dequeue());
            hold = t;
        }
        else
        {
            int t = minoNow.id;
            generateMino(hold);
            hold = t;
        }
        isHeld = true;
    }

    // Update is called once per frame
    void Update()
    {


        bool left = Input.GetKey("a");//左右
        bool right = Input.GetKey("d");
        bool down = Input.GetKey("s");//软降
        bool harddrop = Input.GetKeyDown("space");//硬降
        bool cw = Input.GetKeyDown("right");
        bool ccw = Input.GetKeyDown("left");
        bool ho = Input.GetKeyDown("w");//hold
        bool restart = Input.GetKeyDown("f4");
        int input = 0;
        if (left) input = -1;
        if (right) input = 1;
        if (!left && !right) input = 0;//左-1右1
        if (ho && !isHeld)
        {
            Hold();

        }
        if (restart)
        {
            RestartGame();
        }


        float inputVertical = Input.GetAxis("Vertical");

        TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
        float timeNow = (float)time.TotalSeconds;//现在的时间


        if (!gameover)
        {
            if (ccw) CCWRotate();//逆时针旋转
            if (cw)
            {
                CWRotate();
                //顺时针旋转
            }

            if (harddrop) hardDrop();//硬降

            //左右移动
            if (input == 0 || lastInput == 0)
            {
                arrTrigger = false;
                arrTimer = 0;
                dasTimer = 0;
            }
            else
            {
                if ((input == -lastInput))
                {
                    arrTrigger = false;
                    arrTimer = 0;
                    dasTimer = 0;
                }

                if (arrTrigger)
                {
                    if (arrTimer == 0)
                        arrTimer = timeNow;
                    if (timeNow - arrTimer >= ARR)
                    {
                        Move(input);
                        arrTimer = timeNow;
                    }
                }
                else
                {

                    if (dasTimer == 0)
                    {
                        Move(input);
                        dasTimer = timeNow;
                    }
                    else if (timeNow - dasTimer >= DAS)
                    {
                        dasTimer = 0;
                        arrTrigger = true;
                        Move(input);
                    }
                }
            }
            lastInput = input;


            //自然下落or软降
            if (timeNow - fallTimer >= (down ? SARR : GRAVITY))
            {//每0.5s降落一次
                if (isValid(minoNow.id, minoNow.rotation, new Vector2Int(minoCoordinate.x, minoCoordinate.y - 1), minoNow.size))
                {
                    Fall();
                    lockTimer = 0;
                }
                else
                {
                    if (lockTimer == 0)
                        lockTimer = timeNow;
                    if (timeNow - lockTimer >= LOCK_DELAY)
                    {
                        LockBlock();
                        lockTimer = 0;
                        generateMino(nextQueue.Dequeue());
                        //generateMino(6);
                    }
                }

                fallTimer = timeNow;
            }
        }

        RefreshField();
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
        }




    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Game : MonoBehaviour
{

    public Text t;
    public float LOCK_DELAY = 1f;
    public float GRAVITY = 0.5f;
    public float SARR = 0.1f;
    public float DAS = 0.083f;
    public float ARR = 0.016f;


    struct Mino
    {
        public int[,] array;
        public int size;
        public string name;
        public int rotation;
        public int id;
    };

    int[] bag = new int[] { 1, 2, 3, 4, 5, 6, 7 };//7bag机制

    public int[,] field = new int[40, 10];//10*20的盘面，预留20行，避免c4w玩家被顶到溢出（

    private Mino minoNow;

    private int[] minoPosition = new int[] { 0, 0 };//旋转中心的位置

    Mino S, Z, L, J, T, O, I;

    private float fallTimer;
    private float lockTimer;
    private float dasTimer;
    private float arrTimer;
    private float lastInput;
    private bool arrTrigger = false;
    private bool harddropTrigger = false;
    private Dictionary<int, string> MinoDic = new Dictionary<int, string>();

    private int count = 0;
    public bool gameover = false;
    private int shadowDistance;

    enum mino { S, Z, L, J, T, O, I };

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

        //初始化40行10列的盘面
        for (int i = 0; i < 40; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                field[i, j] = 0;
            }
        }

        bag = new int[]{ 6,7,2,1,3,4,5};
        //Shuffle(bag);
        generateMino(bag[0]);
        count++;
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
        //if (count % 7 == 0) Shuffle(bag);
        minoPosition = new int[] { 19, 4 };

        if (TouchGround()) gameover = true;

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

    void CCWRotate()//顺时针旋转
    {
        int size = minoNow.size;
        int[,] temp = minoNow.array;
        minoNow.array = RotateMatrix(minoNow.array, size);
    }

    void Fall()
    {
        minoPosition[0]--;
    }

    void Move(int x)
    {
        if (!touchWall(x)) minoPosition[1] += x;
        lockTimer = 0;
    }

    bool TouchGround()//判断方块是否触地
    {

        List<int[]> l = GetAllPositions();
        foreach (int[] pos in l)
        {
            if (pos[0] <= 0) return true;
            if (field[pos[0] - 1, pos[1]] < 0) return true;
        }
        return false;
    }

    bool touchWall(int LorR)
    {
        List<int[]> l = GetAllPositions();
        foreach (int[] pos in l)
        {
            if (pos[1] + LorR > 9 || pos[1] + LorR < 0) return true;
            if (field[pos[0], pos[1] + LorR] < 0) return true;
        }
        return false;
    }

    void RefreshField()
    {//在确定新的方块坐标和方向后，刷新field中的元素
        for (int i = 0; i < 40; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (field[i, j] > 0)
                {
                    field[i, j] = 0;
                }
            }

        }
        shadowDistance = 0;
        int size = minoNow.size;
        bool shadowTouchGround = false;
        List<int[]> l = GetAllPositions();
        while (!shadowTouchGround)
        {
            foreach (int[] pos in l)
            {
                if (pos[0] - shadowDistance <= 0)
                {
                    shadowTouchGround = true;
                    break;
                }
                if (field[pos[0] - shadowDistance - 1, pos[1]] < 0)
                {

                    shadowTouchGround = true;
                    break;
                }
            }
            shadowDistance++;
        }
        try
        {
            foreach (int[] pos in l)
                field[pos[0] - shadowDistance + 1, pos[1]] = 20+minoNow.id;

            foreach (int[] pos in l)
                field[pos[0], pos[1]] = minoNow.id;
        }
        catch
        {

        }
    }

    List<int[]> GetAllPositions()//mino四个格子的坐标
    {
        List<int[]> l = new List<int[]>();
        int size = minoNow.size;
        if (size == 3)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (minoNow.array[i, j] == 1)
                        l.Add(new int[] { i - 1 + minoPosition[0], j - 1 + minoPosition[1] });

        }
        else if (size == 5)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (minoNow.array[i, j] == 1)
                        l.Add(new int[] { i - 2 + minoPosition[0], j - 2 + minoPosition[1] });
        }
        else if (size == 2)
        {

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    l.Add(new int[] { i + minoPosition[0], j + minoPosition[1] });
        }
        return l;
    }

    void clearLine(int x)
    {
        for (int i = x; i < 39; i++)
        {
            for (int j = 0; j < 10; j++)
                field[i, j] = field[i + 1, j];
        }

    }

    void LockBlock()
    {
        List<int[]> l = GetAllPositions();
        foreach (int[] pos in l)
        {
            field[pos[0], pos[1]] = -minoNow.id;
        }

        int fulllines = 0;

        for (int i = 0; i < 40; i++)
        {
            bool full = true;
            for (int j = 0; j < 10; j++)
            {
                if (field[i, j] == 0) full = false;
            }

            if (full)
            {
                clearLine(i);
                i--;
            }
        }
    }

    void hardDrop()
    {
        while (!TouchGround())
        {
            Fall();
        }

        LockBlock();
        lockTimer = 0;
        generateMino(bag[count % 7]);
        count++;
        //generateMino(6);
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
        int input = 0;
        if (left) input = -1;
        if (right) input = 1;
        if (!left && !right) input = 0;//左-1右1

        float inputVertical = Input.GetAxis("Vertical");

        TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
        float timeNow = (float)time.TotalSeconds;//现在的时间


        if (!gameover)
        {
            if (ccw) CCWRotate();//逆时针旋转
            if (cw)
            {
                CCWRotate();
                CCWRotate();
                CCWRotate();
                //顺时针旋转
            }

            else {
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
            }

            //自然下落or软降
            if (timeNow - fallTimer >= (down ? SARR : GRAVITY))
            {//每0.5s降落一次
                if (!TouchGround())
                {
                    Fall();
                    lockTimer = 0;
                }
                else if (TouchGround())
                {
                    if (lockTimer == 0)
                        lockTimer = timeNow;
                    if (timeNow - lockTimer >= LOCK_DELAY)
                    {
                        LockBlock();
                        lockTimer = 0;
                        generateMino(bag[count % 7]);

                        //generateMino(6);
                    }
                }

                fallTimer = timeNow;
            }
        }

        RefreshField();
        t.text = "";
        string str = "", s = "";
        for (int i = 19; i >= 0; i--)
        {
            for (int j = 0; j < 10; j++)
            {
                if (field[i, j]>0 && field[i, j] <20)
                {
                    s = "回";
                }else if (field[i, j] <0)
                {
                    s = "回";
                }else if (field[i, j] > 20)
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

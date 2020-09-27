using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.XR;
using UnityEngine.Assertions.Must;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;

public class FumenPlayer : MonoBehaviour
{   //public Text t;
    public float LOCK_DELAY = 1f;
    public float GRAVITY = 0.5f;
    public float SARR = 0.1f;
    public float DAS = 0.083f;
    public float ARR = 0.016f;
    public Vector2Int[,] OFFSET_3x3;
    public Vector2Int[,] OFFSET_I;
    private bool left, right, cw, ccw, down, harddrop, ho, restart;
    private bool turnback = false;


    public Text clearMsg;

    public FieldVisualization f;

    [HideInInspector]
    public NextManager next;

    //public int[,] field = new int[10, 40];//10*20�����棬Ԥ��20�У�����c4w��ұ����������
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

    public int count = 0;

    [HideInInspector]
    public bool gameover = false;

    [HideInInspector]
    public int hold = 0;
    [HideInInspector]
    public bool isHeld;


    public Fumen fumen;

    public TMP_InputField missionList;
    public Toggle useCustomNext;
    public TMP_InputField tips;

    public TMP_Dropdown folder;
    public TMP_Dropdown filename;

    public void LoadFumen()
    {
        Debug.Log("\\Fumen\\" + folder.captionText.text + "\\" + filename.captionText.text);
        StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "\\Fumen\\" + folder.captionText.text + "\\" + filename.captionText.text);
        string json = reader.ReadLine();
        fumen = JsonUtility.FromJson<Fumen>(json);
        fumen.ListToField();
        reader.Close();
        Debug.Log(fumen.minoSequence);

        LoadField();
        field.RefreshField();
        tips.text = fumen.tips;
    }
    // Start is called before the first frame update
    void Start()
    {
        //t = GetComponent<Text>();


        //3*3�������ǽ��
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

        //I�����ǽ��
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

        fumen = new Fumen();
        gameover = true;
    }


    void GenerateMino(int minoId)
    {
        if (minoId != 0)
        {
            currentMino = new Mino(minoId);

            currentMino.position = new Vector2Int(4, 20);
            if (!field.IsValid(currentMino, currentMino.position)) gameover = true;
        }
        else
        {
            currentMino = new Mino(0);

            currentMino.position = new Vector2Int(4, 20);
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

    Vector2Int WallKickOffset(Mino mino, int A, int B, Vector2Int coordinate)//A��B����rotationId
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

    public void RestartGame()
    {
        count = 0;
        gameover = false;

        hold = 0;
        isHeld = false;
        wasB2B = false;
        combo = 0;
        next = new NextManager();
        next.setUseCustomNext(fumen.useCustomNext);
        next.SetNextSeq(fumen.minoSequence);
        next.ResetNext();
        GenerateMino(next.Dequeue());
        f.UpdateNextTilemap(next);
        f.UpdateHoldTilemap(hold);
        LoadField();
        field.RefreshField(currentMino);
    }

    void CCWRotate()//��ʱ����ת
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

    void CWRotate()//��ʱ����ת
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


    List<Vector2Int> GetAllCoordinates()//mino�ĸ����ӵ�����
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
        bool cannotGoUp = !field.IsValid(currentMino, new Vector2Int(currentMino.position.x, currentMino.position.y + 1));
        field.LockMino(currentMino);
        count++;
        ct = new ClearType();
        ct.wasB2b = wasB2B;
        //��ʱ�����λ�ú���ת���Ѿ��̶�����ʼ�������ʼ춨
        int lines = field.LinesCanClear(currentMino);
        ct.lines = lines;
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

                    if (c && f && cannotGoUp)//t-spin single
                    {
                        ct.tSpin = true;
                        ct.tSpinType = 1;
                    }
                    else if ((c && !f && cannotGoUp) || (c && (!f) && k))//t-spin mini
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

            ct.pc = field.IsAllClear();
            clearMsg.text = ct.ClearMessage();

            TimeSpan time = DateTime.Now.TimeOfDay;//������ȷ��������
            float timeNow = (float)time.TotalSeconds;//���ڵ�ʱ��
            clearMsgTimer = timeNow;
        }
        dasTimer = 0;
        arrTimer = 0;
        arrTrigger = false;
        isHeld = false;
    }


    void HardDrop()
    {
        Vector2Int newPosition = currentMino.position;
        newPosition.y--;
        while (field.IsValid(currentMino, newPosition))
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

    void LoadField()
    {
        field.array = fumen.customField.Clone() as int[,];

        field.RefreshField();
    }

    void Update()
    {

        harddrop = Input.GetKeyDown("space");//Ӳ��
        cw = Input.GetKeyDown("right");
        ccw = Input.GetKeyDown("left");
        ho = Input.GetKeyDown("w");//hold
        restart = Input.GetKeyDown("f4");
        if (!gameover) field.RefreshField(currentMino);
        else
        {
            field.RefreshField();
        }
        f.SetField(field);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!gameover) ResponseMovement();

        TimeSpan time = DateTime.Now.TimeOfDay;//������ȷ��������
        float timeNow = (float)time.TotalSeconds;//���ڵ�ʱ��
        if (timeNow - clearMsgTimer >= 2 && clearMsgTimer != 0)
        {
            clearMsg.text = "";
            clearMsgTimer = 0;
        }
    }
    void ResponseMovement()
    {


        TimeSpan time = DateTime.Now.TimeOfDay;//������ȷ��������
        float timeNow = (float)time.TotalSeconds;//���ڵ�ʱ��
        left = Input.GetKey("a");//����
        right = Input.GetKey("d");
        down = Input.GetKey("s");//��
        
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
                CCWRotate();//��ʱ����ת
                ccw = false;
            }
            if (cw)
            {
                CWRotate();
                //˳ʱ����ת
                cw = false;
            }

            if (harddrop)
            {
                HardDrop();//Ӳ��
                harddrop = false;
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
                        Move(input);
                        dasTimer = timeNow;
                    }
                    else if (timeNow - dasTimer >= DAS)//����das�ӳٺ��ƶ��ڶ���
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
            else//�������Ҳ���ƶ�������das
            {
                arrTrigger = false;
                arrTimer = 0;
                dasTimer = 0;
            }
            lastInput = input;
            //��Ȼ����or��
            if (timeNow - fallTimer >= (down ? SARR : GRAVITY))
            {//ÿ0.5s����һ��
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


        }
    }

}
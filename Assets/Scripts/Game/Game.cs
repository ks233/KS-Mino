using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.XR;
using UnityEngine.Assertions.Must;

public class Game
{
    //Game类包括了游戏的所有信息：场地、当前方块、Next和Hold、combo数、b2b、方块序号
    //以及所有的游戏规则：旋转、移动、踢墙、Game Over判定
    //但是它没有任何对外界输出的函数，只是通过执行游戏规则改变类的内部
    //实际游戏中的响应按键、显示画面在Play.cs中


    public Vector2Int[,] OFFSET_3x3;//SZLJT的踢墙表
    public Vector2Int[,] OFFSET_I;//I的踢墙表


    //数据
    public int statLine =0;
    public int statAttack =0;
    public int statPiece =0;
    public int statScore =0;

    public NextManager next;

    //public int[,] field = new int[10, 40];//10*20的盘面，预留20行，避免c4w玩家被顶到溢出（
    public Field field = new Field();
    public Mino currentMino;


    public ClearType clearType = new ClearType();
    private bool wasB2B = false;
    private int combo = 0;
    private bool gaming = true;
    private int hold = 0;
    private bool isHeld;



    public Game()
    {

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
        Restart();

    }



    public void Restart()
    {
        statAttack = 0;
        statLine = 0;
        statPiece = 0;
        statScore = 0;
        gaming = true;

        //初始化40行10列的盘面
        field.ClearAll();
        hold = 0;
        isHeld = false;
        wasB2B = false;
        combo = 0;

        next = new NextManager(false);
        NewMino(next.Dequeue());

        UpdateField();
    }

    public int[] GetNextSeq() {
        return next.nextQueue.ToArray();
    }
    public void NewMino(int minoId)
    {
        if (minoId != 0)
        {
            currentMino = new Mino(minoId)
            {
                position = new Vector2Int(4, 20)
            };
            UpdateField();
            if (!field.IsValid(currentMino, currentMino.GetPosition())) GameOver();

        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gaming = false;
    }

    public int GetCurrentMinoId()
    {
        return currentMino.GetIdInt();
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

    public bool Gaming()
    {
        return gaming;
    }
    private Vector2Int WallKickOffset(Mino mino, int A, int B, Vector2Int coordinate)//A和B都是rotationId
    {
        int size = mino.GetSize();

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

    public void CCWRotate()//逆时针旋转
    {
        int size = currentMino.GetSize();
        int newRotationId = (currentMino.GetRotationId() + 3) % 4;

        if (size == 3)
        {
            if (field.IsValid(currentMino, newRotationId, currentMino.GetPosition()))//如果可以直接转进去
            {
                currentMino.CCWRotate();
            }
            else//如果不能就做踢墙检定
            {
                Vector2Int o = WallKickOffset(currentMino, currentMino.GetRotationId(), newRotationId, currentMino.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    currentMino.CCWRotate();
                    currentMino.Move(o);
                    field.kicked = true;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(currentMino, currentMino.GetRotationId(), newRotationId, currentMino.GetPosition());
            if (o != new Vector2Int(0, 0))
            {
                currentMino.CCWRotate();
                currentMino.Move(o);
            }


        }

        Debug.Log("key ccw");
        UpdateField();

    }

    public void CWRotate()//逆时针旋转
    {
        int size = currentMino.GetSize();
        int newRotationId = (currentMino.GetRotationId() + 1) % 4;
        if (size == 3)
        {
            if (field.IsValid(currentMino, newRotationId, currentMino.GetPosition()))
            {
                currentMino.CWRotate();
            }
            else
            {
                Vector2Int o = WallKickOffset(currentMino, currentMino.GetRotationId(), newRotationId, currentMino.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    currentMino.CWRotate();
                    currentMino.Move(o);
                    field.kicked = true;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(currentMino, currentMino.GetRotationId(), newRotationId, currentMino.GetPosition());
            if (o != new Vector2Int(0, 0))
            {
                currentMino.CWRotate();
                currentMino.Move(o);
            }

        }
        Debug.Log("key cw");

        UpdateField();
    }
    public void Move(int x)
    {
        Vector2Int newPosition = currentMino.GetPosition();
        newPosition.x += x;
        if (field.IsValid(currentMino, currentMino.GetRotationId(), newPosition)) currentMino.Move(x);

        UpdateField();
    }


    List<Vector2Int> GetAllCoordinates()//mino四个格子的坐标
    {
        List<Vector2Int> l = new List<Vector2Int>();
        int size = currentMino.GetSize();
        if (size == 3)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (currentMino.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 1 + currentMino.GetPosition().x, j - 1 + currentMino.GetPosition().y));

        }
        else if (size == 5)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (currentMino.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 2 + currentMino.GetPosition().x, j - 2 + currentMino.GetPosition().y));

        }

        else if (size == 2)
        {

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    l.Add(new Vector2Int(i + currentMino.GetPosition().x, j + currentMino.GetPosition().y));
        }
        return l;
    }



    private void CheckClearType()
    {
        clearType = new ClearType();
        clearType.wasB2b = wasB2B;
        //此时方块的位置和旋转都已经固定，开始消行性质检定
        int lines = field.LinesCanClear(currentMino);
        clearType.lines = lines;
        clearType.pc = field.IsAllClear(lines);
        if (currentMino.id == Mino.MinoID.T)//tspin判定
        {
            bool c, f, k;
            c = field.HasThreeCorners(currentMino);
            f = field.HasTwoFeets(currentMino);
            k = field.kicked;
            if (lines == 1)
            {
                bool cannotGoUp = !field.IsValid(currentMino, new Vector2Int(currentMino.GetPosition().x, currentMino.GetPosition().y + 1));
                //Debug.Log(cannotGoUp);
                if (c && f && cannotGoUp)//t-spin single
                {
                    clearType.tSpin = true;
                    clearType.tSpinType = 1;
                }
                else if ((c && !f && cannotGoUp) || (c && !f && k))//t-spin mini
                {
                    clearType.tSpin = true;
                    clearType.tSpinType = 0;

                }
            }
            else if (lines == 2 && c)//t-spin double
            {
                clearType.tSpin = true;
                clearType.tSpinType = 2;
            }
            else if (lines == 3)
            {
                clearType.tSpin = true;
                clearType.tSpinType = 3;
            }
        }
        else
        {
            clearType.tSpin = false;
        }
        wasB2B = clearType.GetIsB2b();


        statAttack += clearType.GetAttack();

    }

    public void AfterLock() {       //子类继承时重写

    }

    public int[,] GetFieldArray() {
        return field.array.Clone() as int[,];
    }
    public void LockMino()
    {
        field.LockMino(currentMino);
        if (field.LinesCanClear(currentMino) > 0)
        {
            CheckClearType();
            statLine += field.Clear(currentMino);
        }
        isHeld = false;
        statPiece++;
        NextMino();
        UpdateField();
        AfterLock();
    }

    public int Fall()
    {

        Vector2Int newPosition = currentMino.GetPosition();
        newPosition.y -= 1;
        if (field.IsValid(currentMino, currentMino.GetRotationId(), newPosition))
        {
            currentMino.Fall();

            UpdateField();
            return 0;
        }
        else
        {
            return -1;
        }
    }


    public void HardDrop()
    {
        Vector2Int newPosition = currentMino.GetPosition();
        newPosition.y--;
        while (field.IsValid(currentMino, currentMino.GetRotationId(), newPosition))
        {
            currentMino.Fall();

            newPosition.y--;
        }
        LockMino();
    }
    public void UpdateField()
    {
        field.RefreshField(currentMino);
    }
    public void NextMino()
    {
        NewMino(next.Dequeue());
    }

    public int GetHoldId()
    {
        return hold;
    }


    public void Hold()
    {

        if (!isHeld)
        {
            if (hold == 0)
            {
                int t = currentMino.GetIdInt();
                NewMino(next.Dequeue());
                hold = t;
            }
            else
            {
                int t = currentMino.GetIdInt();
                NewMino(hold);
                hold = t;
            }
            isHeld = true;

            UpdateField();
        }
    }
}


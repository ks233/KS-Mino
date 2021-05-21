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


    public static Vector2Int[,] OFFSET_3x3;//SZLJT的踢墙表
    public static Vector2Int[,] OFFSET_I;//I的踢墙表


    //数据
    public int statLine =0;
    public int statAttack =0;
    public int statPiece =0;
    public int statScore =0;

    public NextManager next;

    //public int[,] field = new int[10, 40];//10*20的盘面，预留20行，避免c4w玩家被顶到溢出（
    public Field field = new Field();
    public Mino activeMino;


    public ClearType clearType = new ClearType();
    private bool wasB2B = false;
    private int combo = -1;
    private bool gameover = false;
    private int hold = 0;
    private bool isHeld;
    private ScoreBoard scoreBoard;



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
        gameover = false;

        //初始化40行10列的盘面
        field.ClearAll();
        hold = 0;
        isHeld = false;
        wasB2B = false;
        combo = 0;

        next = new NextManager(false);
        NewMino(next.Dequeue());

    }

    public int[] GetNextSeq() {
        return next.nextQueue.ToArray();
    }
    public void NewMino(int minoId)
    {
        if (minoId != 0)
        {
            activeMino = new Mino(minoId)
            {
                position = new Vector2Int(4, 20)
            };
            if (!field.IsValid(activeMino, activeMino.GetPosition())) GameOver();

        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameover = true;
    }

    public int GetActiveMinoId()
    {
        return activeMino.GetIdInt();
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


    public Vector2Int GetActiveMinoPos() {
        return activeMino.position;
    }
    public int GetGhostY()
    {
        return field.LandHeight(activeMino);

    }

    public bool Gaming()
    {
        return !gameover;
    }
    private Vector2Int WallKickOffset(Mino mino, int A, int B, Vector2Int coordinate)//A和B都是rotationId
    {

        return WallKickOffset(field,mino,A,B,coordinate);

    }

    public static Vector2Int WallKickOffset(Field field,Mino mino, int A, int B, Vector2Int coordinate)//A和B都是rotationId
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



    public Mino GetActiveMino()
    {
        return activeMino;
    }

    public int CCWRotate()//逆时针旋转
    {
        int size = activeMino.GetSize();
        int newRotationId = (activeMino.GetRotationId() + 3) % 4;

        if (size == 3)
        {
            if (field.IsValid(activeMino, newRotationId, activeMino.GetPosition()))//如果可以直接转进去
            {
                activeMino.CCWRotate();
                return 0;
            }
            else//如果不能就做踢墙检定
            {
                Vector2Int o = WallKickOffset(activeMino, activeMino.GetRotationId(), newRotationId, activeMino.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    activeMino.CCWRotate();
                    activeMino.Move(o);
                    field.kicked = true;
                    return 0;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(activeMino, activeMino.GetRotationId(), newRotationId, activeMino.GetPosition());
            if (o != new Vector2Int(0, 0))
            {
                activeMino.CCWRotate();
                activeMino.Move(o);
                return 0;
            }


        }
        return -1;

    }

    public int CWRotate()//逆时针旋转
    {
        int size = activeMino.GetSize();
        int newRotationId = (activeMino.GetRotationId() + 1) % 4;
        if (size == 3)
        {
            if (field.IsValid(activeMino, newRotationId, activeMino.GetPosition()))
            {
                activeMino.CWRotate();
                return 0;
            }
            else
            {
                Vector2Int o = WallKickOffset(activeMino, activeMino.GetRotationId(), newRotationId, activeMino.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    activeMino.CWRotate();
                    activeMino.Move(o);
                    field.kicked = true;

                    return 0;
                }
            }
        }
        else if (size == 5)
        {

            Vector2Int o = WallKickOffset(activeMino, activeMino.GetRotationId(), newRotationId, activeMino.GetPosition());
            if (o != new Vector2Int(0, 0))
            {
                activeMino.CWRotate();
                activeMino.Move(o);

                return 0;
            }
        }

        return -1;

    }
    public int Move(int x)
    {
        Vector2Int newPosition = activeMino.GetPosition();
        newPosition.x += x;
        if (field.IsValid(activeMino, activeMino.GetRotationId(), newPosition))
        {
            activeMino.Move(x);
            return 0;
        }
        return -1;
    }


    List<Vector2Int> GetAllCoordinates()//mino四个格子的坐标
    {
        List<Vector2Int> l = new List<Vector2Int>();
        int size = activeMino.GetSize();
        if (size == 3)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (activeMino.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 1 + activeMino.GetPosition().x, j - 1 + activeMino.GetPosition().y));

        }
        else if (size == 5)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (activeMino.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 2 + activeMino.GetPosition().x, j - 2 + activeMino.GetPosition().y));

        }

        else if (size == 2)
        {

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    l.Add(new Vector2Int(i + activeMino.GetPosition().x, j + activeMino.GetPosition().y));
        }
        return l;
    }



    private void CheckClearType()
    {

        bool tSpin = false;
        int tSpinType = 0;
        int lines = field.LinesCanClear(activeMino);

        clearType = new ClearType
        {
            combo = combo,
            wasB2b = wasB2B,
            //此时方块的位置和旋转都已经固定，开始消行性质检定
            lines = lines,
            pc = field.IsAllClear(lines)
        };

        if (activeMino.id == Mino.MinoID.T)//tspin判定
        {
            bool c, f, k;
            c = field.HasThreeCorners(activeMino);
            f = field.HasTwoFeets(activeMino);
            k = field.kicked;
            if (lines == 1)
            {
                bool cannotGoUp = !field.IsValid(activeMino, new Vector2Int(activeMino.GetPosition().x, activeMino.GetPosition().y + 1));
                //Debug.Log(cannotGoUp);
                if (c && f && cannotGoUp)//t-spin single
                {
                    tSpin = true;
                    tSpinType = 1;
                }
                else if ((c && !f && cannotGoUp) || (c && !f && k))//t-spin mini
                {
                    tSpin = true;
                    tSpinType = 0;

                }
            }
            else if (lines == 2 && c)//t-spin double
            {
                tSpin = true;
                tSpinType = 2;
            }
            else if (lines == 3)
            {
                tSpin = true;
                tSpinType = 3;
            }
        }
        else
        {
            clearType.tSpin = false;
        }
        clearType.tSpin = tSpin;
        clearType.tSpinType = tSpinType;

        wasB2B = clearType.GetIsB2b();


        statAttack += clearType.GetAttack();

    }

    public virtual void AfterLock() {       //子类继承时重写

    }

    public int[,] GetFieldArray() {
        return field.array.Clone() as int[,];
    }
    public int LockMino(out ClearType ct)
    {
        int line = 0;
        field.LockMino(activeMino);
        if (field.LinesCanClear(activeMino) > 0)
        {
            combo += 1;
            CheckClearType();
            line = field.Clear(activeMino);
            statLine += line;
        }
        else
        {
            combo = -1;
        }
        isHeld = false;
        statPiece++;
        AfterLock();
        ct = clearType;
        return line;
    }


    public int Fall()
    {

        Vector2Int newPosition = activeMino.GetPosition();
        newPosition.y -= 1;
        if (field.IsValid(activeMino, activeMino.GetRotationId(), newPosition))
        {
            activeMino.Fall();

            return 0;
        }
        else
        {
            return -1;
        }
    }


    public void Harddrop(out int hdCells)
    {
        hdCells = 0;//硬降下降的格数，用于游戏计分（每格+2分）
        int y = activeMino.position.y;
        int landHeight = field.LandHeight(activeMino);
        activeMino.position.y = landHeight;
        hdCells = y - landHeight;
        

    }
    public void NextMino()
    {
        NewMino(next.Dequeue());
    }

    public int GetHoldId()
    {
        return hold;
    }


    public int Hold()
    {

        if (!isHeld)
        {
            if (hold == 0)
            {
                int t = activeMino.GetIdInt();
                NewMino(next.Dequeue());
                hold = t;
            }
            else
            {
                int t = activeMino.GetIdInt();
                NewMino(hold);
                hold = t;
            }
            isHeld = true;

            return 0;
        }
        return -1;
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Field
{
    public int[,] array = new int[10, 40];//10*20的盘面，预留20行，避免c4w玩家被顶到溢出（

    private int ghostDist;//阴影距离

    public bool kicked = false;//判断tspin的条件之一：是否踢墙过

    public Field()
    {
        ClearAll();
    }

    void ClearLine(int x)
    {
        Debug.Log("clear");
        for (int i = x; i < 39; i++)
        {
            for (int j = 0; j < 10; j++)
                array[j, i] = array[j, i + 1];
        }
        kicked = false;

    }

    public void ClearAll()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                array[i, j] = 0;
            }
        }
    }

    public bool IsValid(Mino mino, Vector2Int coordinate)
    {
        int minoId = mino.GetIdInt();
        int x = coordinate.x;
        int y = coordinate.y;
        if (1 <= minoId && minoId <= 5)//3*3的mino
        {
            for (int i = 0; i < 3; i++)//逐个方格检查
            {
                for (int j = 0; j < 3; j++)
                {
                    if (mino.array[j, i] == 1)
                    {
                        if (coordinate.x + i - 1 > 9 || coordinate.x + i - 1 < 0) return false;
                        if (coordinate.y + j - 1 < 0) return false;
                        if (array[coordinate.x + i - 1, coordinate.y + j - 1] < 0) return false;
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
                    if (array[coordinate.x + i, coordinate.y + j] < 0) return false;
                }
            }
        }
        if (minoId == 7)
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if (mino.array[j + 2, i + 2] == 1)
                    {
                        if (x + i > 9 || x + i < 0) return false;
                        if (y + j < 0) return false;
                        if (array[x + i, y + j] < 0) return false;
                    }
                }
            }
        }
        return true;
    }

    public bool IsValid(Mino mino, int rotationId, Vector2Int coordinate)
    {
        int size = mino.GetSize();
        int minoId = mino.GetIdInt();
        int[,] testMino;
        testMino = new Mino(minoId).array;
        for (int i = 0; i < (4 - rotationId) % 4; i++)
        {
            testMino = Game.RotateMatrix(testMino, size);
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
                        if (array[coordinate.x + i - 1, coordinate.y + j - 1] < 0) return false;
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
                    if (array[coordinate.x + i, coordinate.y + j] < 0) return false;
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
                        if (array[x + i, y + j] < 0) return false;
                    }
                }
            }
        }
        return true;
    }

    public bool HasThreeCorners(Mino tMino)//判定tspin的条件之一
    {
        int x = tMino.position.x;
        int y = tMino.position.y;
        int corner = 0;
        for (int i = x - 1; i <= x + 1; i += 2)
            for (int j = y - 1; j <= y + 1; j += 2)
            {
                if (i < 0 || 9 < i || j < 0)
                {
                    corner++;
                    continue;
                }
                if (array[i, j] != 0) corner++;
            }
        if (corner >= 3) return true;
        return false;
    }


    public bool HasTwoFeets(Mino tMino)//判定tspin的条件其二，如果是，则为t1，不是则为mini
    {
        int x = tMino.position.x;
        int y = tMino.position.y;
        int[] corners = { 0, 0, 0, 0 };
        int cornerId = 0;
        Vector2Int feetId = new Vector2Int();
        for (int i = x - 1; i <= x + 1; i += 2)
            for (int j = y - 1; j <= y + 1; j += 2)
            {
                if (i < 0 || 9 < i || j < 0)
                {
                    corners[cornerId] = 1;
                    cornerId++;
                    continue;
                }
                corners[cornerId] = array[i, j];
                cornerId++;
            }
        switch (tMino.rotation)
        {
            case 0: feetId = new Vector2Int(1, 3); break;
            case 1: feetId = new Vector2Int(2, 3); break;
            case 2: feetId = new Vector2Int(0, 2); break;
            case 3: feetId = new Vector2Int(0, 1); break;
        }
        if (corners[feetId.x] != 0 && corners[feetId.y] != 0) return true;
        return false;
    }


    public List<Vector2Int> GetAllCoordinates(Mino currentMino)//mino四个格子的坐标
    {
        List<Vector2Int> l = new List<Vector2Int>();
        int size = currentMino.GetSize();
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
    public void RefreshField(Mino currentMino)
    {//在确定新的方块坐标和方向后，刷新field中的元素

        for (int i = 0; i < 10; i++)//先清除地形以外的元素
        {
            for (int j = 0; j < 40; j++)
            {
                if (array[i, j] > 0)
                {
                    array[i, j] = 0;
                }
            }

        }
        ghostDist = 0;//阴影距离
        int size = currentMino.GetSize();
        bool shadowTouchGround = false;
        List<Vector2Int> l = GetAllCoordinates(currentMino);
        while (!shadowTouchGround)//方块下方阴影只要有一块即将重叠，就停止增加阴影距离
        {
            foreach (Vector2Int pos in l)
            {
                if (pos.y - ghostDist <= 0)
                {
                    shadowTouchGround = true;
                    break;
                }
                if (array[pos.x, pos.y - ghostDist - 1] < 0)
                {
                    shadowTouchGround = true;
                    break;
                }
            }
            if (!shadowTouchGround)
                ghostDist++;
        }
        foreach (Vector2Int pos in l)//写入阴影
        {

            array[pos.x, pos.y - ghostDist] = 20 + currentMino.GetIdInt();
        }
        foreach (Vector2Int pos in l)//写入方块
        {
            array[pos.x, pos.y] = currentMino.GetIdInt();
        }

    }

    public void Clear()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                    array[i, j] = 0;
            }

        }
    }

    public bool IsAllClear()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                if (array[i, j] > 20|| array[i, j] < 0)
                {
                    int a = array[i, j];
                    return false;
                }
            }
        }
        return true;
    }

    public int LinesCanClear(Mino mino)//消行并返回消除的行数
    {
        int lines = 0;
        List<Vector2Int> l = GetAllCoordinates(mino);
        List<int> ys = new List<int>();//方块占据的所有y坐标，比如竖着的长条占了4行
        foreach (Vector2Int v in l)
        {
            if (!ys.Contains(v.y))
            {
                ys.Add(v.y);
            }
        }
        ys.Sort();
        foreach (int y in ys)
        {

            bool canClear = true;
            for (int j = 0; j < 10; j++)
            {
                if (array[j, y] >= 0) canClear = false;
            }
            if (canClear)
            {
                lines++;
            }
        }

        return lines;
    }

    public void Clear(Mino mino)
    {
        int lines = 0;
        List<Vector2Int> l = GetAllCoordinates(mino);
        List<int> ys = new List<int>();//方块占据的所有y坐标，比如竖着的长条占了4行
        foreach (Vector2Int v in l)
        {
            if (!ys.Contains(v.y))
            {
                ys.Add(v.y);
            }
        }
        ys.Sort();
        foreach (int y in ys)
        {

            bool canClear = true;
            for (int j = 0; j < 10; j++)
            {
                if (array[j, y - lines] >= 0) canClear = false;
            }
            if (canClear)
            {
                ClearLine(y - lines);
                lines++;
            }
        }
    }
    public void LockMino(Mino currentMino)
    {

        List<Vector2Int> l = GetAllCoordinates(currentMino);
        foreach (Vector2Int pos in l)
        {
            array[pos.x, pos.y] = -currentMino.GetIdInt();
            Debug.Log(-currentMino.GetIdInt());
        }
    }

}

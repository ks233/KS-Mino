using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class Field
{
    public int[,] array = new int[10, 40];//10*20的盘面，预留20行，避免c4w玩家被顶到溢出（

    public int[] top = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private int ghostDist;//阴影距离

    public int count;

    public Field Clone()
    {
        Field f = (Field)MemberwiseClone();
        f.array = array.Clone() as int[,];
        f.top = top.Clone() as int[];
        return f;
    }


    public void Count()
    {
        count = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                if (array[i, j] != 0) count++;
            }
        }
    }
    public int MaxTop()
    {
        int max = 0;
        for (int i = 0; i < 10; i++)
        {
            max = Math.Max(max, top[i]);
        }
        return max;
    }

    public int MaxDepth()
    {
        int depth = Math.Max(0, top[1] - top[0]);
        for (int i = 1; i < 9; i++)
        {
            depth = Math.Max(depth, Math.Min(top[i - 1] - top[i], top[i] - top[i + 1]));
        }
        return depth;
    }


    public static Field operator+(Field field,Mino mino)
    {
        field.LockMino(mino);
        field.Clear(mino);
        return field;
    }

    public int MinCol(out int minTop)//高度最低的列
    {

        int minCol = 9;
        minTop = top[9];
        for (int x = 9; x >= 0; x--)
        {
            if (top[x] < minTop)
            {
                minCol = x;
                minTop = top[x];
            }
        }
        return minCol;
    }
    public int CanClearFour()
    {
        UpdateTop();
        int minCol = 0;
        int minTop = top[0];
        for(int x = 0; x < 10; x++)
        {
            if (top[x] < minTop)
            {
                minCol = x;
                minTop = top[x];
            }
        }
        bool four = true;

        for(int y = minTop; y < minTop + 4; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (x == minCol) continue;
                if (array[x, y] == 0)
                {
                    return 0;
                }
            }
        }
        
        return 1;
    }

    public int SecondMinTop()
    {
        List<int> topList = top.ToList();
        topList.Sort();
        return topList[1];
        
    }

    public int GetScore()
    {

        UpdateTop();
        Count();
        int bump = Bumpiness1(out int secondMaxBump);                 //起伏程度
        int maxTop = MaxTop();                  //最高点
        int secondMinTop = SecondMinTop();
        int maxBump = MaxBump();                //最大起伏程度
        int holes = CountHole(out int holeLine);//孔洞数量、带洞行行数
        int contSurface = ContinuousSurface();  //没卵用
        int blockAboveHole = BlockAboveHole();  //没卵用2号
        int maxDepth = MaxDepth();              //没卵用3号
        int aggHeight = 0;                      //总高度
        int sz = SZ();                          //能否横放SZ
        int wellCol = Well(out int wellDepth);              //大于等于2格的深坑的数量
        int deepWellCol = DeepWell(out _);              //大于等于3格的深坑的数量
        int canClearFour = CanClearFour();
        int minCol = MinCol(out int minTop);
        foreach (int i in top)
        {
            aggHeight += i;
        }


        int score = 0;
        /*
        if (bump > 6)
        {
            score -= bump * 40;

        }
        else if (bump > 4)
        {
            score -= bump * 50;
        }
        else
        {
            score -= bump * 10;
        }
        */

        //MPH
        /*
        score = (-101 * holes//101
            - 145 * bump//145
            //- 100 * maxTop//100
            //- (maxTop >= 17 ? 100000 : 0)
            //- 33 * aggHeight//27
            - 800 * holeLine//800
            - 0 * contSurface//0
            - 0 * maxDepth//0
            - 8 * blockAboveHole//8
            + 120 * sz//120
            - 100 * wellDepth//160
            );

        */
        //7Bag

        score = ((-101 * holes//101
                - 145 * bump//145
                - 1600 * holeLine//800
                - (secondMaxBump > 3 ? 400 : 0)
                - 80 * (maxTop - secondMinTop)
                )*3//100
                                              //- (maxTop >= 17 ? 100000 : 0)
                                              //- 27 * aggHeight//27

                - 15 * contSurface//0
                - 15 * blockAboveHole//8
                - aggHeight * 40
                + 400 * sz//120
                - (deepWellCol > 1 ? 180 : 0) * wellDepth//160
                - (wellCol > 1 ? 160 : 0) * wellCol//160
                //+ (count<60?50:0)*count
                + canClearFour * 500
                + (minCol==9 ? 300:-200)
                //- (minCol == 2 || minCol == 7 ? 150 : 0)
                - (minCol == 1 || minCol == 8 || minCol == 0 ? 150 : 0)
                + (((top[0] - secondMinTop+1) * 80)
                + ((top[1] - secondMinTop+1) * 45)
                + ((top[2] - secondMinTop+1) * 30)
                + ((top[3] - secondMinTop+1) * 30)
                + ((top[4] - secondMinTop+1) * 30)
                + ((top[5] - secondMinTop+1) * 30)
                + ((top[6] - secondMinTop+1) * 45)
                + ((top[7] - secondMinTop+1) * 45)
                + ((top[8] - secondMinTop+1) * 80)
                + ((top[9]) * -50))*2
                );
     


        //score -= bump * 350+maxBump*500;



        //score -= (contSurface) * 50;
        //score -= holes * 3000;
        /*
        if (count > 120)
        {
            score -= (count - 120) * 50;
        }
        */
        //score -= blockAboveHole * 30;
        return score;

    }

    public override string ToString()
    {
        string s = "";
        for(int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                s += array[x, y].ToString();
            }
        }
        return s;
    }

    public void SetFieldByString(string s)
    {
        if (s.Length < 200) return;
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                array[x, y] = (int)char.GetNumericValue(s[20 * x + y]);
                
            }
        }
    }

    public int GetScore(Mino mino)
    {
        Field f = Clone();
        f.LockMino(mino);
        if (f.LinesCanClear(mino) > 0)
        {
            f.Clear(mino);
        }
        return f.GetScore();
    }

    public int SZ()//判断SZ是否能平放
    {
        int s = 0, z = 0;
        for (int i = 0; i < 10 - 2; i++)
        {
            if (top[i + 1] == top[i + 2] && top[i + 1] == top[i] - 1) z++;
            if (top[i] == top[i + 1] && top[i + 1] == top[i + 2] - 1) s++;
        }

        int result = 0;
        if (z > 0) result++;
        if (s > 0) result++;
        return result;
    }

    public int Well(out int wellDepth)//深度大于等于3，只能用I填的坑
    {
        int well = 0;
        wellDepth = 0;
        if (top[1] - top[0] >= 2)
        {
            wellDepth += top[1] - top[0];
            well++;
        }


        for (int i = 0; i < 10 - 2; i++)
        {
            if (top[i + 1] < top[i + 2] && top[i + 1] < top[i])
            {
                if (Math.Min(top[i + 2] - top[i + 1], top[i] - top[i + 1]) >= 2)
                {
                    wellDepth += Math.Min(top[i + 2] - top[i + 1], top[i] - top[i + 1]);
                    well++;
                }
            }
        }
        if (top[8] - top[9] >= 2)
        {
            wellDepth += top[8] - top[9];
            well++;
        }
        return well;
    }

    public int DeepWell(out int wellDepth)//深度大于等于3，只能用I填的坑
    {
        int well = 0;
        wellDepth = 0;
        if (top[1] - top[0] >= 3)
        {
            wellDepth += top[1] - top[0];
            well++;
        }


        for (int i = 0; i < 10 - 2; i++)
        {
            if (top[i + 1] < top[i + 2] && top[i + 1] < top[i])
            {
                if (Math.Min(top[i + 2] - top[i + 1], top[i] - top[i + 1]) >= 3)
                {
                    wellDepth += Math.Min(top[i + 2] - top[i + 1], top[i] - top[i + 1]);
                    well++;
                }
            }
        }
        if (top[8] - top[9] >= 3)
        {
            wellDepth += top[8] - top[9];
            well++;
        }
        return well;
    }


    public void UpdateTop(int col)
    {
        for (int i = 22; i >= 0; i--)
        {
            if (array[col, i] != 0)
            {
                top[col] = i + 1;
                return;
            }
        }
        top[col] = 0;
    }

    public int ContinuousSurface()
    {
        int count = 1;
        int tmp = top[0];
        for (int i = 1; i < 10; i++)
        {
            if (top[i] != tmp)
            {
                tmp = top[i];
                count++;
            }
        }
        return count;
    }

    public void UpdateTop()
    {
        for (int i = 0; i < 10; i++)
        {
            UpdateTop(i);
        }
    }

    public int Bumpiness(int a, int b)
    {
        if (b <= a) return 0;
        int bumpiness = 0;
        for (int i = a; i <= b - 1; i++)
        {
            bumpiness += Math.Abs(top[i] - top[i + 1]);
        }
        return bumpiness;
    }

    public int LowestCol()
    {
        int col = 0;
        for (int i = 0; i < 10; i++)
        {
            if (top[i] < top[col]) col = i;
        }
        return col;
    }

    public int Bumpiness1(out int secondMaxBump)
    {
        int lowestCol = LowestCol();
        int bumpiness = 0;
        List<int> bumpList = new List<int>();

        for (int i = 0; i < 9; i++)
        {
            if (i == lowestCol) continue;
            if(i == lowestCol - 1)
            {
                if (lowestCol == 9) continue;
                bumpiness += Math.Abs(top[i] - top[i + 2]);
                bumpList.Add(Math.Abs(top[i] - top[i + 2]));
            }
            else
            {
                bumpiness += Math.Abs(top[i] - top[i + 1]);
                bumpList.Add(Math.Abs(top[i] - top[i + 1]));
            }
        }

        bumpList.Sort();
        secondMaxBump = bumpList[7];
        return bumpiness;
    }

    public int Bumpiness()
    {
        int bumpiness = 0;
        for (int i = 0; i < 9; i++)
        {
            bumpiness += Math.Abs(top[i] - top[i + 1]);

        }
        return bumpiness;
    }


    public int MaxBump(int a, int b)
    {
        if (a >= b) return 0;
        int max = 0;
        for (int i = a; i <= b - 1; i++)
        {
            max = Math.Max(max, Math.Abs(top[i] - top[i + 1]));
        }
        return max;
    }
    public int MaxBump()
    {
        int lowestCol = LowestCol();
        return MaxBump(0, lowestCol - 1) + MaxBump(lowestCol + 1, 9);
    }


    public int CountHole(out int holeLine)
    {
        int hole = 0;
        int[] hasHole = new int[40];
        Array.Clear(hasHole, 0, 20);
        holeLine = 0;
        for (int x = 0; x < 10; x++)
        {
            for (int y = top[x] - 1; y >= 0; y--)
            {
                if (array[x, y] == 0)
                {
                    hole++;
                    hasHole[y]++;
                }
            }
        }
        for (int y = 0; y < 20; y++)
        {
            if (hasHole[y] > 0) holeLine++;
        }
        return hole;
    }

    public int BlockAboveHole()
    {
        int blockAboveHole = 0;
        for (int x = 0; x < 10; x++)
        {
            int depth = 0;
            bool hole = false;
            for (int y = top[x] - 1; y >= 0; y--)
            {
                depth++;
                if (array[x, y] == 0)
                {
                    hole = true;
                    break;
                }
            }
            if (hole == true) blockAboveHole += depth-1;
        }
        return blockAboveHole;
    }


    public Field()
    {
        ClearAll();
    }

    void ClearLine(int x)
    {
        for (int i = x; i < 39; i++)
        {
            for (int j = 0; j < 10; j++)
                array[j, i] = array[j, i + 1];
        }
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


    public bool IsValid(Mino mino)
    {
        List<Vector2Int> l = GetAllCoordinates(mino);
        foreach (Vector2Int v in l)
        {
            int i = v.x;
            int j = v.y;
            if (i > 9 || i < 0) return false;
            if (j < 0) return false;
            if (array[i, j] != 0) return false;
        }
        return true;
    }

    public bool IsValid(Mino mino, Vector2Int centerPos)
    {
        int minoId = mino.GetIdInt();
        int x = centerPos.x;
        int y = centerPos.y;
        Mino tmpMino = new Mino(mino.id);
        tmpMino.SetRotation(mino.GetRotationId());
        tmpMino.SetPosition(centerPos);

        List<Vector2Int> l = GetAllCoordinates(tmpMino);
        foreach (Vector2Int v in l)
        {
            int i = v.x;
            int j = v.y;
            if (i > 9 || i < 0) return false;
            if (j < 0) return false;
            if (array[i, j] != 0) return false;
        }
        return true;
    }

    public bool IsValid(Mino mino, int rotationId, Vector2Int centerPos)
    {

        Mino t = new Mino(mino.id);
        t.SetRotation(rotationId);
        return IsValid(t, centerPos);
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


    public static List<Vector2Int> GetAllCoordinates(Mino m)//mino四个格子的坐标
    {
        List<Vector2Int> l = new List<Vector2Int>();
        int size = m.GetSize();
        if (size == 3)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (m.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 1 + m.position.x, j - 1 + m.position.y));

        }
        else if (size == 5)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (m.array[j, i] == 1)
                        l.Add(new Vector2Int(i - 2 + m.position.x, j - 2 + m.position.y));

        }

        else if (size == 2)
        {

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    l.Add(new Vector2Int(i + m.position.x, j + m.position.y));
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


    public int LandHeight(Mino m)
    {
        Vector2Int pos = m.position;
        while (IsValid(m, pos))
        {
            pos.y--;
        }
        return pos.y + 1;
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


    public bool IsAllClear(int n)//从第n行往上，如果一块也没有，就是全消（此判定比消行要早）
    {
        if (n <= 0) return false;
        for (int i = 0; i < 10; i++)
        {
            for (int j = n; j < 17; j++)
            {
                if (array[i, j] != 0)
                {
                    //Debug.Log(String.Format("not pc because array[{0},{1}]={2}", i, j,array[i, j]));
                    return false;
                }
            }
        }
        return true;
    }

    public int LinesCanClear(Mino mino)//返回可消除的行数
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
                if (array[j, y] == 0) canClear = false;
            }
            if (canClear)
            {
                lines++;
            }
        }

        return lines;
    }

    public int Clear(Mino mino)//消行并返回行数
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
                if (array[j, y - lines] == 0) canClear = false;
            }
            if (canClear)
            {
                ClearLine(y - lines);
                lines++;
            }
        }
        return lines;
    }
    public void LockMino(Mino currentMino)
    {

        List<Vector2Int> l = GetAllCoordinates(currentMino);
        foreach (Vector2Int pos in l)
        {
            array[pos.x, pos.y] = currentMino.GetIdInt();
        }
    }

}

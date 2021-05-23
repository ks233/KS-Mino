using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Field
{
    public int[,] array = new int[10, 40];//10*20�����棬Ԥ��20�У�����c4w��ұ����������

    public int[] top = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private int ghostDist;//��Ӱ����

    public int count;

    public void Count()
    {
        count = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                if(array[i, j] != 0)count++;
            }
        }
    }

    public int GetScore()
    {

        UpdateTop();
        Count();
        int bump = Bumpiness();
        int maxBump = MaxBump();
        int holes = CountHole();
        int contSurface = ContinuousSurface();
        int blockAboveHole = BlockAboveHole();


        int score = 1000;

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
            //score -= bump * 10;
        }

        score -= maxBump * 30;



        score -= (contSurface) * 40;
        score -= holes * 300;

        if (count > 120)
        {
            score -= (count - 120) * 50;
        }

        score -= blockAboveHole * 30;
        if (maxBump == 0 && bump == 0 && (contSurface == 2 || contSurface == 3))
        {
            score = 900;
        }
        return score;

    }

    public int GetScore(Mino mino)
    {
        Field f = new Field();
        f.array = array.Clone() as int[,];
        f.LockMino(mino);
        if (f.LinesCanClear(mino) > 0)
        {
            f.Clear(mino);
        }
        return f.GetScore();
    }



    public void UpdateTop(int col)
    {
        for(int i = 22; i >= 0; i--)
        {
            if(array[col,i] != 0)
            {
                top[col] = i+1;
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

    public int Bumpiness(int a,int b)
    {
        if (b <= a) return 0;
        int bumpiness=0;
        for (int i = a; i <= b-1; i++)
        {
            bumpiness += Math.Abs(top[i]-top[i+1]);
        }
        return bumpiness;
    }

    public int LowestCol()
    {
        int col = 0;
        for(int i = 0; i < 10; i++)
        {
            if (top[i] < top[col]) col = i;
        }
        return col;
    }

    /*
    public int Bumpiness()
    {
        int lowestCol = LowestCol();
        int bumpiness = 0;
        for (int i = 0; i < 9; i++)
        {
            if (i == lowestCol) continue;
            if(i == lowestCol - 1)
            {
                if (lowestCol == 9) continue;
                bumpiness += Math.Abs(top[i] - top[i + 2]);
            }
            else
            {
                bumpiness += Math.Abs(top[i] - top[i + 1]);
            }
        }
        return bumpiness;
    }
    */
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


    public int CountHole()
    {
        int hole = 0;
        for (int x = 0; x < 10; x++)
        {
            for(int y = top[x]-1; y >= 0; y--)
            {
                if (array[x,y] == 0) hole++;
            }
        }
        return hole;
    }

    public int BlockAboveHole()
    {
        int blockAboveHole = 0;
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < top[x]; y++)
            {
                /*
                if (array[x, y] != 0)
                {
                    t++;
                }
                else
                {
                    blockAboveHole += t;
                    break;
                }*/
                if(array[x,y] == 0)
                {
                    for(int i = y+1; i < top[x]; i++)
                    {
                        blockAboveHole++;
                    }
                    break;
                }
            }
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
        foreach(Vector2Int v in l) {
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

    public bool HasThreeCorners(Mino tMino)//�ж�tspin������֮һ
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


    public bool HasTwoFeets(Mino tMino)//�ж�tspin���������������ǣ���Ϊt1��������Ϊmini
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


    public static List<Vector2Int> GetAllCoordinates(Mino m)//mino�ĸ����ӵ�����
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
    {//��ȷ���µķ�������ͷ����ˢ��field�е�Ԫ��

        for (int i = 0; i < 10; i++)//��������������Ԫ��
        {
            for (int j = 0; j < 40; j++)
            {
                if (array[i, j] > 0)
                {
                    array[i, j] = 0;
                }
            }

        }
        ghostDist = 0;//��Ӱ����
        int size = currentMino.GetSize();
        bool shadowTouchGround = false;
        List<Vector2Int> l = GetAllCoordinates(currentMino);
        while (!shadowTouchGround)//�����·���ӰֻҪ��һ�鼴���ص�����ֹͣ������Ӱ����
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
        foreach (Vector2Int pos in l)//д����Ӱ
        {

            array[pos.x, pos.y - ghostDist] = 20 + currentMino.GetIdInt();
        }
        foreach (Vector2Int pos in l)//д�뷽��
        {
            array[pos.x, pos.y] = currentMino.GetIdInt();
        }

    }


    public int LandHeight(Mino m) {
        Vector2Int pos = m.position;
        while (IsValid(m,pos)) {
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


    public bool IsAllClear(int n)//�ӵ�n�����ϣ����һ��Ҳû�У�����ȫ�������ж�������Ҫ�磩
    {
        if (n <= 0) return false;
        for (int i = 0; i < 10; i++)
        {
            for (int j = n; j < 17; j++)
            {
                if (array[i, j]!=0)
                {
                    //Debug.Log(String.Format("not pc because array[{0},{1}]={2}", i, j,array[i, j]));
                    return false;
                }
            }
        }
        return true;
    }

    public int LinesCanClear(Mino mino)//���ؿ�����������
    {
        int lines = 0;
        List<Vector2Int> l = GetAllCoordinates(mino);
        List<int> ys = new List<int>();//����ռ�ݵ�����y���꣬�������ŵĳ���ռ��4��
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

    public int Clear(Mino mino)//���в���������
    {
        int lines = 0;
        List<Vector2Int> l = GetAllCoordinates(mino);
        List<int> ys = new List<int>();//����ռ�ݵ�����y���꣬�������ŵĳ���ռ��4��
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

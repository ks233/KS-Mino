using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Search
{
    const int BEAM_NUM = 3;
    public static Mino SpawnMino(int minoId)
    {
        return new Mino(minoId, 4, 19, 0);
    }

    public static void MakeTree(SearchNode node, int depth)
    {
        if (depth == 0)
        {
            return;
        }
        node.child = GetChildren(node, true);
        for (int i = 0; i < node.child.Count; i++)
        {
            MakeTree(node.child[i], depth - 1);
        }
    }
    public static SearchNode Run(SearchNode root, int depth)
    {
        MakeTree(root, depth);
        return Iter(root);
    }
    public static SearchNode Iter(SearchNode node)
    {
        if (node == null||node.child.Count==0)return node;
        SearchNode best = node.child[0];
        float maxEval = best.score;
        for (int i = 0; i < node.child.Count; i++)
        {
            SearchNode child = Iter(node.child[i]);
            float eval = child.score;
            if (eval > maxEval)
            {
                maxEval = eval;
                best = child;
            }
        }
        return best;
    }


    public static List<SearchNode> GetChildren(SearchNode _node, bool kick)
    {
        Queue<SearchNode> bfs = new Queue<SearchNode>();//广搜队列
        List<SearchNode> visited = new List<SearchNode>();//已经访问过的
        List<SearchNode> result = new List<SearchNode>();//搜索结果
        bfs.Enqueue(_node.Clone());
        while (bfs.Count > 0)
        {
            SearchNode node = bfs.Dequeue();
            SearchNode moveRight, moveLeft, moveDown;
            SearchNode rotateCW, rotateCCW;
            //
            if (node.MoveRight(out moveRight) && !visited.Contains(moveRight))
            {
                bfs.Enqueue(moveRight);
                visited.Add(moveRight);
            }
            //
            if (node.MoveLeft(out moveLeft) && !visited.Contains(moveLeft))
            {
                bfs.Enqueue(moveLeft);
                visited.Add(moveLeft);

            }
            //
            if (node.Rotate(out rotateCCW, false) && !visited.Contains(rotateCCW))
            {
                bfs.Enqueue(rotateCCW);
                visited.Add(rotateCCW);
            }
            //
            if (node.Rotate(out rotateCW, true) && !visited.Contains(rotateCW))
            {
                bfs.Enqueue(rotateCW);
                visited.Add(rotateCW);
            }
            //
            if (node.MoveDown(out moveDown))
            {
                if (!visited.Contains(moveDown))
                {
                    bfs.Enqueue(moveDown);
                    visited.Add(moveDown);
                }
            }
            else
            {
                result.Add(moveDown);
            }
        }
        List<SearchNode> filteredResult = new List<SearchNode>();//搜索结果

        foreach (SearchNode sn in result)
        {
            if (!filteredResult.Contains(sn.OppositeMinoNode()) && !filteredResult.Contains(sn))
            {
                filteredResult.Add(sn);
            }
        }
        for (int i = 0; i < filteredResult.Count; i++)
        {
            filteredResult[i].clearType = Game.CheckClearType(filteredResult[i].field.Clone(), filteredResult[i].mino, filteredResult[i].op, 0, false);
            filteredResult[i].Eval();
            filteredResult[i].Lock();
            filteredResult[i].minoResult = filteredResult[i].mino;
            filteredResult[i].mino = new Mino(filteredResult[i].next.nextQueue.Peek());
            filteredResult[i].next = _node.childNext;
            filteredResult[i].childNext = filteredResult[i].next.CloneAndDequeue();
        }
        List<SearchNode> sortedList = filteredResult.OrderByDescending(o => o.score).Take(BEAM_NUM).ToList();
        
        return sortedList;
    }

}

public class SearchNode
{
    public Field field;
    public Mino mino, minoResult;
    public ClearType clearType;
    public Next next;
    public Next childNext;

    public List<SearchNode> child = new List<SearchNode>();//下层子节点
    public int score = 0;
    public string op = "";



    public SearchNode(SearchNode node)
    {
        field = node.field;
        mino = node.mino;
        clearType = node.clearType;
        this.next = node.next;
        childNext = next.CloneAndDequeue();
    }
    public SearchNode(Field field, Mino mino, string op, Next next)
    {
        this.field = field;
        this.mino = mino;
        this.op = op;
        this.next = next;
        mino = mino.Clone();
        childNext = next.CloneAndDequeue();
    }
    public SearchNode Clone()
    {
        return (SearchNode)this.MemberwiseClone();
    }
    public void Lock()//锁定，将方块加入地形
    {
        field = field.Clone();
        field = field + mino;
    }

    public void Eval()
    {
        //Debug.Log(clearType.lines);
        if(clearType != null)
        {
            score = field.GetScore(mino) + (clearType.lines == 4 ? 1500 : 0);
        }
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() == this.GetType())
        {
            SearchNode node = obj as SearchNode;
            return node.mino.Equals(mino);
        }
        return base.Equals(obj);
    }

    public SearchNode OppositeMinoNode()
    {
        return new SearchNode(field, mino.OppositeMino(), "", next.Clone());//
    }

    public bool MoveRight(out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        tmp.position = newPos;
        tmp.SetRotation(mino.rotation);
        bool ok = false;        //移动是否有效
        newPos.x++;
        if (field.IsValid(tmp, newPos))
        {
            tmp.Move(1);
            ok = true;
        }
        sn = new SearchNode(field, tmp, op + "r", next.Clone());
        return ok;
    }
    public bool MoveLeft(out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        tmp.position = newPos;
        tmp.SetRotation(mino.rotation);
        bool ok = false;
        newPos.x--;
        if (field.IsValid(tmp, newPos))
        {
            tmp.Move(-1);
            ok = true;
        }
        sn = new SearchNode(field, tmp, op + "l", next.Clone());
        return ok;
    }
    public bool MoveDown(out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        tmp.position = newPos;
        tmp.SetRotation(mino.rotation);
        bool ok = false;
        newPos.y--;
        if (field.IsValid(tmp, newPos))
        {
            tmp.Fall();
            ok = true;
        }
        sn = new SearchNode(field, tmp, op + "d", next.Clone());
        return ok;
    }

    public bool HardDrop(out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        tmp.position = newPos;
        tmp.SetRotation(mino.rotation);
        bool ok = false;
        newPos.y--;
        string hd = "";
        while (field.IsValid(tmp, newPos))
        {
            tmp.Fall();
            hd += "d";
            ok = true;
            newPos.y--;
        }
        sn = new SearchNode(field, tmp, op + hd, next.Clone());
        return ok;
    }

    public bool Rotate(out SearchNode sn, bool cw)
    {
        Vector2Int newPos = mino.position;
        bool ok = false;
        Mino tmp = mino.Clone();
        tmp.position = newPos;
        tmp.SetRotation(mino.rotation);
        int size = tmp.GetSize();
        string k = cw ? "c" : "z"; ;
        int newRotationId = cw ? (tmp.GetRotationId() + 1) % 4 : (tmp.GetRotationId() + 3) % 4;

        if (size == 3)
        {
            if (field.IsValid(tmp, newRotationId, tmp.GetPosition()))//如果可以直接转进去
            {
                if (cw)
                {
                    tmp.CWRotate();
                }
                else
                {
                    tmp.CCWRotate();
                }
                ok = true;
            }
            else//如果不能就做踢墙检定
            {
                Vector2Int o = Game.WallKickOffset(field, tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    if (cw)
                    {
                        tmp.CWRotate();
                    }
                    else
                    {
                        tmp.CCWRotate();
                    }
                    tmp.Move(o);
                    k = cw ? "C" : "K";
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }
        }
        else if (size == 5)
        {
            Vector2Int o = Game.WallKickOffset_I(field, tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition(), out bool iSpin);
            if (o != new Vector2Int(0, 0))
            {
                if (cw)
                {
                    tmp.CWRotate();
                }
                else
                {
                    tmp.CCWRotate();
                }
                tmp.Move(o);
                if (iSpin)
                {
                    k = cw ? "C" : "K";
                }
                ok = true;
            }
            else
            {
                ok = false;
            }
        }
        sn = new SearchNode(field, tmp, op + k, next.Clone());
        return ok;
    }
    /*
    public bool RotateCCW(Field field, out SearchNode sn)
    {
        bool ok = false;
        Mino tmp = mino.Clone();
        int size = tmp.GetSize();
        int newRotationId = (tmp.GetRotationId() + 3) % 4;
        string k = "z";


        if (size == 3)
        {
            if (field.IsValid(tmp, newRotationId, tmp.GetPosition()))//如果可以直接转进去
            {
                tmp.CCWRotate();
                ok = true;
            }
            else//如果不能就做踢墙检定
            {
                Vector2Int o = Game.WallKickOffset(field, tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    tmp.CCWRotate();
                    tmp.Move(o);
                    k = "Z";
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }
        }
        else if (size == 5)
        {
            Vector2Int o = Game.WallKickOffset_I(field, tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition(), out bool iSpin);
            if (o != new Vector2Int(0, 0))
            {
                tmp.CCWRotate();
                tmp.Move(o);
                if (iSpin)
                {
                    k = "Z";
                }
                ok = true;
            }
            else
            {
                ok = false;
            }
        }
        sn = new SearchNode(tmp,op+k);
        return ok;
    }
    */
}

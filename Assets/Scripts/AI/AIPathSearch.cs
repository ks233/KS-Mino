using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSearch
{
    public static List<SearchNode> GetLandPoints(Field field, Mino mino)
    {
        Queue<SearchNode> bfs = new Queue<SearchNode>();//广搜队列
        List<SearchNode> visited = new List<SearchNode>();//已经访问过的
        List<SearchNode> result = new List<SearchNode>();//搜索结果
        bfs.Enqueue(new SearchNode(mino));
        while (bfs.Count > 0)
        {
            SearchNode node = bfs.Dequeue();

            SearchNode moveRight,moveLeft,moveDown;
            SearchNode rotateCW, rotateCCW;
            //
            if(node.MoveRight(field,out moveRight) && !visited.Contains(moveRight))
            {
                bfs.Enqueue(moveRight);
                visited.Add(moveRight);
            }
            //
            if(node.MoveLeft(field,out moveLeft) && !visited.Contains(moveLeft))
            {
                bfs.Enqueue(moveLeft);
                visited.Add(moveLeft);

            }
            //
            if (node.RotateCCW(field, out rotateCCW) && !visited.Contains(rotateCCW))
            {
                bfs.Enqueue(rotateCCW);
                visited.Add(rotateCCW);
            }
            //
            if (node.RotateCW(field, out rotateCW) && !visited.Contains(rotateCW))
            {
                bfs.Enqueue(rotateCW);
                visited.Add(rotateCW);
            }
            //
            if (node.MoveDown(field, out moveDown) )
            {
                if(!visited.Contains(moveDown))
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
        foreach(SearchNode sn in result)
        {
            if (!filteredResult.Contains(sn.OppositeMinoNode()) && !filteredResult.Contains(sn))
            {
                filteredResult.Add(sn);
            }
        }
        return filteredResult;
    }
}

public class SearchNode
{
    // Start is called before the first frame update
    public Mino mino;
    public int mark = 0;
    public int score = 0;
    public SearchNode(Mino m) => mino = m;
    public override bool Equals(object obj)
    {
        if(obj.GetType() == this.GetType())
        {
            SearchNode node = obj as SearchNode;
            return node.mino.Equals(mino);
        }
        return base.Equals(obj);
    }

    public SearchNode OppositeMinoNode()
    {
        return new SearchNode(mino.OppositeMino());
    }

    public bool MoveRight(Field field,out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        bool ok = false;        //是否不能再往右了
        newPos.x++;
        if (field.IsValid(mino, newPos))
        {
            tmp.Move(1);
            ok = true;
        }        
        sn = new SearchNode(tmp);
        return ok;
    }
    public bool MoveLeft(Field field, out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        bool ok = false; 
        newPos.x--;
        if (field.IsValid(mino, newPos))
        {
            tmp.Move(-1);
            ok = true;
        }
        sn = new SearchNode(tmp);
        return ok;
    }


    public bool MoveDown(Field field, out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        bool ok = false;
        newPos.y--;
        if (field.IsValid(mino, newPos))
        {
            tmp.Fall();
            ok = true;
        }
        sn = new SearchNode(tmp);
        return ok;
    }

    public bool RotateCW(Field field, out SearchNode sn)
    {


        bool ok = false;
        Mino tmp = mino.Clone();
        int size = tmp.GetSize();
        int newRotationId = (tmp.GetRotationId() + 1) % 4;

        if (size == 3)
        {
            if (field.IsValid(tmp, newRotationId, tmp.GetPosition()))//如果可以直接转进去
            {
                tmp.CWRotate();
                ok = true;
            }
            else//如果不能就做踢墙检定
            {
                Vector2Int o = Game.WallKickOffset(field,tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    tmp.CWRotate();
                    tmp.Move(o);
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

            Vector2Int o = Game.WallKickOffset(field,tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition());
            if (o != new Vector2Int(0, 0))
            {
                tmp.CWRotate();
                tmp.Move(o);
                ok = true;
            }
            else
            {
                ok = false;
            }
        }
        sn = new SearchNode(tmp);
        return ok;
    }

    public bool RotateCCW(Field field, out SearchNode sn)
    {


        bool ok = false;
        Mino tmp = mino.Clone();
        int size = tmp.GetSize();
        int newRotationId = (tmp.GetRotationId() + 3) % 4;

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

            Vector2Int o = Game.WallKickOffset(field, tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition());
            if (o != new Vector2Int(0, 0))
            {
                tmp.CCWRotate();
                tmp.Move(o);
                ok = true;
            }
            else
            {
                ok = false;
            }
        }
        sn = new SearchNode(tmp);
        return ok;
    }

}

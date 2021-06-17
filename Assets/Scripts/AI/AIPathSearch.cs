using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Search
{

    const int NUM_BEAMS = 5;
    public static Mino SpawnMino(int minoId)
    {
        return new Mino(minoId, 4, 19, 0);
    }


    public static List<int> MaxScorePath(List<SearchNode> node)//�����Ѿ�����õ�beam search�������ͨ������Ҷ�ӽڵ��·��
    {
        for (int i = 0; i < node.Count; i++)
        {
            node[i].path.Add(i);
        }
        Queue<SearchNode> bfs = new Queue<SearchNode>(node);
        Stack<SearchNode> tmp = new Stack<SearchNode>();

     

        while (bfs.Count > 0)
        {
            SearchNode sn = bfs.Dequeue();
            tmp.Push(sn);
            for (int i = 0; i < sn.childCount; i++)
            {
                sn.child[i].path = sn.path;
                sn.child[i].path.Add(i);
                bfs.Enqueue(sn.child[i]);
            }
        }
        List<SearchNode> leafNodes = new List<SearchNode>();
        for(int i = 0; i < NUM_BEAMS; i++)
        {
            leafNodes.Add(tmp.Pop());
        }
        List<SearchNode> sortedList = leafNodes.OrderByDescending(o => o.score).ToList();
        return sortedList[0].path;
    }

    public static List<SearchNode> OneNextTest(Field field, int minoId,int nextId)
    {

        //TODO:�����������NUM_BEAMSʱ���������


        List<SearchNode> rootList = GetLandPoints(field.Clone(), SpawnMino(minoId)).Take(NUM_BEAMS).ToList();
        for(int i = 0; i < NUM_BEAMS; i++)//��ÿ���ڵ����NUM_BEAM���ӽڵ�
        {
            rootList[i].child = GetLandPoints(field.Clone() + rootList[i].mino, SpawnMino(nextId)).Take(NUM_BEAMS).ToList();
        }


        //���ڹ���NUM_BEAM^2���ӽڵ㣬Ҫ���ٵ�NUM_BEAM��

        int count = 0;
        while(count<NUM_BEAMS)//��ÿ���ڵ���ɸѡ��߷�
        {
            int maxIndex = 0;
            int max = rootList[0].child[0].score; 
            for (int i = 0; i < NUM_BEAMS; i++)//�����ڵ㷽�����ƹ鲢����
            {
                if (rootList[i].child[rootList[i].childCount].score > max)
                {
                    max = rootList[i].child[i].score;
                    maxIndex = i;
                }
            }
            rootList[maxIndex].childCount++;//
            count++;
        }
        return rootList;
    }


    public static List<Mino> GetSolution(SearchNode root)
    {
        //Beam Search..



        return new List<Mino>();
    }


    public static List<SearchNode> GetLandPoints(Field field, Mino mino)
    {
        return GetLandPointsKick(field, mino);
        if (field.CountHole(out _) == 0)
        {
            return GetLandPointsLite(field,mino);
        }
        else
        {
            return GetLandPointsKick(field,mino);
        }
    }


        public static List<SearchNode> GetLandPointsLite(Field field, Mino mino)
    {
        List<SearchNode> result = new List<SearchNode>();//�������

        SearchNode spawnNode = new SearchNode(mino, "");
        for (int rotation = 0; rotation < 4; rotation++)
        {
            SearchNode node = spawnNode;
            for (int i = 0; i < rotation; i++)
            {
                node.RotateCW(field,out node);
            }

            SearchNode moveRight=node, moveLeft=node;
            while (moveRight.MoveRight(field, out moveRight))
            {
                moveRight.HardDrop(field, out SearchNode hardDrop);
                result.Add(hardDrop);
            }
            while (moveLeft.MoveLeft(field, out moveLeft))
            {
                moveLeft.HardDrop(field, out SearchNode hardDrop);
                result.Add(hardDrop);
            }
        }

        List<SearchNode> filteredResult = new List<SearchNode>();//�������

        foreach (SearchNode sn in result)
        {
            if (!filteredResult.Contains(sn.OppositeMinoNode()) && !filteredResult.Contains(sn))
            {
                filteredResult.Add(sn);
            }
        }
        for (int i = 0; i < filteredResult.Count; i++)
        {
            filteredResult[i].clearType = Game.CheckClearType(field.Clone(), filteredResult[i].mino, filteredResult[i].op, 0, false);
            filteredResult[i].Eval(field);
        }
        List<SearchNode> sortedList = filteredResult.OrderByDescending(o => o.score).ToList();

        return sortedList;
    }

    public static List<SearchNode> GetLandPointsKick(Field field, Mino mino)
    {
        Queue<SearchNode> bfs = new Queue<SearchNode>();//���Ѷ���
        List<SearchNode> visited = new List<SearchNode>();//�Ѿ����ʹ���
        List<SearchNode> result = new List<SearchNode>();//�������
        bfs.Enqueue(new SearchNode(mino,""));
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
        List<SearchNode> filteredResult = new List<SearchNode>();//�������
        
        foreach(SearchNode sn in result)
        {
            if (!filteredResult.Contains(sn.OppositeMinoNode()) && !filteredResult.Contains(sn))
            {
                filteredResult.Add(sn);
            }
        }
        for(int i = 0; i < filteredResult.Count; i++)
        {
            filteredResult[i].clearType = Game.CheckClearType(field.Clone(), filteredResult[i].mino, filteredResult[i].op, 0, false);
            filteredResult[i].Eval(field);
        }
        List<SearchNode> sortedList = filteredResult.OrderByDescending(o => o.score).ToList();
        
        return sortedList;
    }

}

public class SearchNode
{
    // Start is called before the first frame update
    public Mino mino;
    public ClearType clearType;


    public List<SearchNode> child = new List<SearchNode>();//�²��ӽڵ�

    public int childCount = 0;

    public List<int> path = new List<int>();

    public int score = 0;
    public string op = "";
    




    public SearchNode()
    {

    }


    public void Eval(Field field)
    {
        //Debug.Log(clearType.lines);
        score = field.GetScore(mino) + (clearType.lines == 4 ? 1500:0);

    }

    public SearchNode(Mino m, string op)
    {
        mino = m;
        this.op = op;
    }
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
        return new SearchNode(mino.OppositeMino(),"");//
    }

    public bool MoveRight(Field field,out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        bool ok = false;        //�Ƿ�����������
        newPos.x++;
        if (field.IsValid(mino, newPos))
        {
            tmp.Move(1);
            ok = true;
        }        
        sn = new SearchNode(tmp,op+"r");
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
        sn = new SearchNode(tmp, op + "l");
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
        sn = new SearchNode(tmp, op + "d");
        return ok;
    }

    public bool HardDrop(Field field, out SearchNode sn)
    {
        Vector2Int newPos = mino.position;
        Mino tmp = mino.Clone();
        bool ok = false;
        newPos.y--;
        string hd = "";
        while (field.IsValid(mino, newPos))
        {
            tmp.Fall();
            hd += "d";
            ok = true;
            newPos.y--;
        }
        sn = new SearchNode(tmp, op + hd);
        return ok;
    }

    public bool RotateCW(Field field, out SearchNode sn)
    {


        bool ok = false;
        Mino tmp = mino.Clone();
        int size = tmp.GetSize();
        string k = "c";//����������ǽ��Ϊ��дC
        int newRotationId = (tmp.GetRotationId() + 1) % 4;

        if (size == 3)
        {
            if (field.IsValid(tmp, newRotationId, tmp.GetPosition()))//�������ֱ��ת��ȥ
            {
                tmp.CWRotate();
                ok = true;
            }
            else//������ܾ�����ǽ�춨
            {
                Vector2Int o = Game.WallKickOffset(field,tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition());
                if (o != new Vector2Int(0, 0))
                {
                    tmp.CWRotate();
                    tmp.Move(o);
                    k = "C";
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

            Vector2Int o = Game.WallKickOffset_I(field,tmp, tmp.GetRotationId(), newRotationId, tmp.GetPosition(),out bool iSpin);
            if (o != new Vector2Int(0, 0))
            {
                tmp.CWRotate();
                tmp.Move(o);
                if (iSpin)
                {
                    k = "C";
                }
                ok = true;
            }
            else
            {
                ok = false;
            }
        }
        sn = new SearchNode(tmp, op + k);
        return ok;
    }
    public bool RotateCCW(Field field, out SearchNode sn)
    {


        bool ok = false;
        Mino tmp = mino.Clone();
        int size = tmp.GetSize();
        int newRotationId = (tmp.GetRotationId() + 3) % 4;
        string k = "z";


        if (size == 3)
        {
            if (field.IsValid(tmp, newRotationId, tmp.GetPosition()))//�������ֱ��ת��ȥ
            {
                tmp.CCWRotate();
                ok = true;
            }
            else//������ܾ�����ǽ�춨
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

}

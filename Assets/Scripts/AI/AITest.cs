using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AITest : MonoBehaviour
{
    //Play.cs用来更新游戏内容，是游戏系统与玩家的桥梁。
    //在FixedUpdate中响应按键，根据按键对Game做出相应的操作，再从Game调取信息，显示在游戏界面上。



    //几个延迟参数的设定


    public const int MAX_NEXT = 6;

    public GameObject ParentField;
    public GameObject[] MinoTiles;
    public GameObject[] Minoes;
    public GameObject HoldArea;
    public GameObject NextArea;
    public GameObject ActiveMinoParent;

    //显示单块
    public GameObject MinoParent;

    public Game game;//仅用于显示

    private bool showField = false;


    public int drawColor;
    private bool eraserMode;
    private bool mouseDown;

    private List<SearchNode> nodes;
    private int nodesIndex = 0;
    public Text txtNodeIndex;
    public Text txtScore;

    public InputField inputFStr;

    private List<SearchNode> landpoints;


    private bool pause = true;
    private float aiTimer;
    private List<Mino> buffer = new List<Mino>();


    private float GetCurrentTime()
    {
        System.TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
        float t = (float)time.TotalSeconds;//现在的时间
        return t;
    }

    private void UpdateActiveMinoDisplay()
    {
        DestroyAllChild(ActiveMinoParent);
        Mino tmpMino = game.GetActiveMino();

        //方块
        List<Vector2Int> l = Field.GetAllCoordinates(game.activeMino);
        int ghostDist = tmpMino.GetPosition().y - game.GetGhostY();
        foreach (Vector2Int v in l)
        {
            DisplayUtils.InstChild(MinoTiles[tmpMino.GetIdInt() - 1], new Vector3(v.x, v.y, 0), ActiveMinoParent);
            if (game.Gaming())
            {

                DisplayUtils.InstChild(MinoTiles[tmpMino.GetIdInt() - 1], new Vector3(v.x, v.y - ghostDist, 0), ActiveMinoParent, 1, 0.5f);
            }
        }
    }


    private void DestroyAllChild(GameObject parent)
    {//删除所有child object
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void UpdateHoldDisplay()
    {
        int holdid = game.GetHoldId();
        DestroyAllChild(HoldArea);
        if (holdid != 0)
        {
            DisplayUtils.InstChild(Minoes[holdid - 1], Vector3Int.zero, HoldArea);
        }
    }

    private void UpdateNextDisplay()
    {
        DestroyAllChild(NextArea);
        int[] nextSeq = game.GetNextSeq();
        for (int i = 0; i < MAX_NEXT; i++)
        {
            int id = nextSeq[i];
            Vector3 offset = Vector3.zero;
            if (id == Mino.NameToId("J"))
            {
                offset.x = -0.5f;
            }
            else if (id == Mino.NameToId("I"))
            {
                offset.x = 0.5f;
            }
            else if (id == Mino.NameToId("O"))
            {
                offset.y = 0.5f;
            }
            DisplayUtils.InstChild(Minoes[nextSeq[i] - 1], new Vector3(0, -2.5f * i, 0) + offset, NextArea, 0.8f);
        }
    }


    private void PutMino(Mino mino)
    {
        
        game.SetActiveMino(mino);
        _ = game.LockMino(out _);
        game.NextMino();
        if (showField)
            UpdateFieldDisplay();
    }

    public void PutMino()
    {

        landpoints = Search.GetLandPoints(game.field, Search.SpawnMino(game.GetActiveMinoId()));
        /*
        for (int i = 0; i < landpoints.Count; i++)
        {
            landpoints[i].score = game.field.GetScore(landpoints[i].mino);
        }
        List<SearchNode> SortedList = landpoints.OrderByDescending(o => o.score).ToList();
       
        landpoints = SortedList;
        */
        if (landpoints.Count == 0) {
            game.SetActiveMino(new Mino(game.GetActiveMinoId(), 4, 19, 0));
        }
        else
        {
            game.SetActiveMino(landpoints[0].mino);
        }
        _ = game.LockMino(out _);
        game.NextMino();
        if(showField)
            UpdateFieldDisplay();
        //UpdateNextDisplay();
    }

    public void GetFieldString()
    {
        inputFStr.text = game.field.ToString();
        Debug.Log(game.field.GetScore());
    }

    public void SetFieldString()
    {
        game.field.SetFieldByString(inputFStr.text);
        UpdateFieldDisplay();
    }


    public void ShowMino(int minoID,int x,int y,int rotation)
    {
        Mino mino = new Mino(minoID,x,y,rotation);
        List<Vector2Int> l = Field.GetAllCoordinates(mino);
        foreach(Vector2Int v in l)
        {
            DisplayUtils.InstChild(MinoTiles[mino.GetIdInt()-1], new Vector3(v.x, v.y, 0), MinoParent);
        }
    }
    public void ShowMino(Mino mino)
    {
        List<Vector2Int> l = Field.GetAllCoordinates(mino);
        foreach (Vector2Int v in l)
        {
            DisplayUtils.InstChild(MinoTiles[mino.GetIdInt() - 1], new Vector3(v.x, v.y, 0), MinoParent);
        }
    }



    public void UpdateFieldDisplay()
    {
        DestroyAllChild(ParentField);
        int[,] field = game.GetFieldArray();



        //InstChild(Minoes[activeMinoId-1], tmp, ParentField,1,0.6f);

        //场地
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                if (1 <= field[x, y] && field[x, y] <= 7)
                    DisplayUtils.InstChild(MinoTiles[field[x, y] - 1], new Vector3(x, y, 0), ParentField);
            }
        }
    }


    void Start()
    {
        game = new Game();
        game.field = new Field();
    }

    public void PauseAndStart()
    {
        float t = GetCurrentTime();

        txtNodeIndex.text = game.statLine.ToString() + "\n" + (GetCurrentTime()-t);
        pause = !pause;
    }



    public void TestFieldFunc()
    {
        game.field.UpdateTop();
        int bump = game.field.Bumpiness();
        int maxBump = game.field.MaxBump();
        int holes = game.field.CountHole(out int holeLine);
        int contSurface = game.field.ContinuousSurface();
        int score = game.field.GetScore();
        string s = "";

        s += "综合评分：" + score + "/1000";
        s += String.Format("\n地形起伏程度{0}\n最大地形起伏程度{3}\n孔洞数量{1}\n连续地表{2}", bump, holes,contSurface,maxBump);
        /*
        for(int i = 0;i< nodes.Count; i++)
        {
            nodes[i].score = game.field.GetScore(nodes[i].mino);
        }
        */
        List<SearchNode> SortedList = nodes.OrderByDescending(o => o.score).ToList();

        nodes = SortedList;
        txtScore.text = s;
    }


    public void LandPointTest()
    {
        nodesIndex = 0;
        nodes = Search.GetLandPoints(game.field, new Mino(1, 4, 19, 0));
        foreach (SearchNode sn in nodes)
        {
            Debug.Log(sn.mino);
        }
        txtNodeIndex.text = String.Format("落点({0}/{1})",nodesIndex+1,nodes.Count);
    }


    private void OneNextTest()
    {
        List<SearchNode> beamTree = Search.OneNextTest(game.field.Clone(), game.activeMino.GetIdInt(), game.next.nextQueue.Peek());//测试下两块为ST的组合
        List<int> path = Search.MaxScorePath(beamTree);
        DestroyAllChild(MinoParent);
        PutMino(beamTree[path[0]].mino);
        //buffer.Add(beamTree[path[0]].mino);
        //buffer.Add(beamTree[path[0]].child[path[1]].mino);
    }

    public void DoSomething()//临时debug按钮对应的函数，功能不确定
    {
        /*
        game.field.UpdateTop();
        Debug.Log(game.field.BlockAboveHole());
        showField = !showField;
        */

        if (buffer.Count == 0)
        {
            List<SearchNode> beamTree = Search.OneNextTest(game.field.Clone(), game.activeMino.GetIdInt(), game.next.nextQueue.Peek());//测试下两块为ST的组合
            List<int> path = Search.MaxScorePath(beamTree);
            DestroyAllChild(MinoParent);
            buffer.Add(beamTree[path[0]].mino);
            buffer.Add(beamTree[path[0]].child[path[1]].mino);
        }
        PutMino(buffer[0]);
        buffer.RemoveAt(0);

        UpdateFieldDisplay();
        UpdateNextDisplay();
    }




    public void ShowLandPoint()
    {
        DestroyAllChild(MinoParent);
        if (nodesIndex < 5)
        {

            ShowMino(nodes[nodesIndex].mino);
            txtNodeIndex.text = String.Format("落点({0}/{1})\n{2}\n评分：{3}", nodesIndex+1, nodes.Count, nodes[nodesIndex].mino.ToString(),nodes[nodesIndex].score);
            nodesIndex++;
        }
        else
        {
            nodesIndex = 0;
        }
    }

    public void Restart()
    {
        game.Restart();
        UpdateHoldDisplay();
        UpdateNextDisplay();
        UpdateFieldDisplay();
        UpdateActiveMinoDisplay();
    }

    public void UpdateDisplay()
    {
        UpdateFieldDisplay();
    }


    void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            int x, y;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            x = (int)(worldPosition.x + 0.5);
            y = (int)(worldPosition.y + 0.5);

            bool mouseInField = 0 <= x && x <= 9 && 0 <= y && y <= 19;

            if (Input.GetMouseButtonDown(0) && mouseInField)
            {
                mouseDown = true;
                eraserMode = game.field.array[x, y] == drawColor;
            }
            if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;

                //Debug.LogFormat("({0},{1})", x, y);
            }
            if (mouseDown)
            {
                if (mouseInField)
                {
                    game.field.array[x, y] = eraserMode ? 0 : drawColor;
                    UpdateFieldDisplay();
                }
            }
            float time = GetCurrentTime();



            //UpdateFieldDisplay();
            txtNodeIndex.text = game.statLine.ToString();
        }

    }

    void FixedUpdate()
    {
        float time = GetCurrentTime();
        if (!pause && !game.gameover)
        {
            if (time - aiTimer > 0.3f)
            {

                aiTimer = time;
                //for (int i = 0;i<5 && !game.gameover;i++)
                {

                    DoSomething();
                    UpdateFieldDisplay();
                    UpdateNextDisplay();

                }

                //UpdateFieldDisplay();
                txtNodeIndex.text = game.statLine.ToString();
            }
        }
    }

}
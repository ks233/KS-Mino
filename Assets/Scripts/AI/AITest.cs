using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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




    public int drawColor;
    private bool eraserMode;
    private bool mouseDown;

    private List<SearchNode> nodes;
    private int nodesIndex = 0;
    public Text txtNodeIndex;


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
        //Restart();

    }



    public void LandPointTest()
    {
        nodesIndex = 0;
        nodes = PathSearch.GetLandPoints(game.field, new Mino(7, 4, 19, 0));
        foreach (SearchNode sn in nodes)
        {
            Debug.Log(sn.mino);
        }
        txtNodeIndex.text = String.Format("落点({0}/{1})",nodesIndex,nodes.Count);
    }

    public void ShowLandPoint()
    {
        DestroyAllChild(MinoParent);
        if (nodesIndex < nodes.Count)
        {

            ShowMino(nodes[nodesIndex].mino);
            nodesIndex++;
        }
        else
        {
            nodesIndex = 0;
        }
        txtNodeIndex.text = String.Format("落点({0}/{1})", nodesIndex, nodes.Count);
    }

    public void Restart()
    {
        game.Restart();
        UpdateHoldDisplay();
        UpdateNextDisplay();
        UpdateFieldDisplay();
        UpdateActiveMinoDisplay();
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

                Debug.LogFormat("({0},{1})", x, y);
            }
            if (mouseDown)
            {
                if (mouseInField)
                {
                    game.field.array[x, y] = eraserMode ? 0 : drawColor;
                    UpdateFieldDisplay();
                }
            }
        }
    }




    void FixedUpdate()
    {

    }
}
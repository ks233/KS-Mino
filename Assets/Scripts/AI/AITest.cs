using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AITest : MonoBehaviour
{
    //Play.cs����������Ϸ���ݣ�����Ϸϵͳ����ҵ�������
    //��FixedUpdate����Ӧ���������ݰ�����Game������Ӧ�Ĳ������ٴ�Game��ȡ��Ϣ����ʾ����Ϸ�����ϡ�



    //�����ӳٲ������趨


    public const int MAX_NEXT = 6;

    public GameObject ParentField;
    public GameObject[] MinoTiles;
    public GameObject[] Minoes;
    public GameObject HoldArea;
    public GameObject NextArea;
    public GameObject ActiveMinoParent;

    //��ʾ����
    public GameObject MinoParent;

    public Game game;//��������ʾ




    public int drawColor;
    private bool eraserMode;
    private bool mouseDown;

    private List<SearchNode> nodes;
    private int nodesIndex = 0;
    public Text txtNodeIndex;
    public Text txtScore;


    private bool pause = true;
    private float aiTimer;

    private float GetCurrentTime()
    {
        System.TimeSpan time = DateTime.Now.TimeOfDay;//������ȷ��������
        float t = (float)time.TotalSeconds;//���ڵ�ʱ��
        return t;
    }

    private void UpdateActiveMinoDisplay()
    {
        DestroyAllChild(ActiveMinoParent);
        Mino tmpMino = game.GetActiveMino();

        //����
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
    {//ɾ������child object
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

    public void PutMino()
    {

        List<SearchNode> landpoints = PathSearch.GetLandPoints(game.field, new Mino(game.GetActiveMinoId(), 4, 19, 0));

        for (int i = 0; i < landpoints.Count; i++)
        {
            landpoints[i].score = game.field.GetScore(landpoints[i].mino);
        }
        List<SearchNode> SortedList = landpoints.OrderByDescending(o => o.score).ToList();

        landpoints = SortedList;

        game.SetActiveMino(landpoints[0].mino);
        ClearType ct;
        game.LockMino(out ct);
        game.NextMino();

        UpdateFieldDisplay();
        UpdateNextDisplay();
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

        //����
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
        pause = !pause;
    }



    public void TestFieldFunc()
    {
        game.field.UpdateTop();
        int bump = game.field.Bumpiness();
        int maxBump = game.field.MaxBump();
        int holes = game.field.CountHole();
        int contSurface = game.field.ContinuousSurface();
        int score = game.field.GetScore();
        string s = "";

        s += "�ۺ����֣�" + score + "/1000";
        s += String.Format("\n��������̶�{0}\n����������̶�{3}\n�׶�����{1}\n�����ر�{2}", bump, holes,contSurface,maxBump);

        for(int i = 0;i< nodes.Count; i++)
        {
            nodes[i].score = game.field.GetScore(nodes[i].mino);
        }
        List<SearchNode> SortedList = nodes.OrderByDescending(o => o.score).ToList();

        nodes = SortedList;
        txtScore.text = s;
    }


    public void LandPointTest()
    {
        nodesIndex = 0;
        nodes = PathSearch.GetLandPoints(game.field, new Mino(1, 4, 19, 0));
        foreach (SearchNode sn in nodes)
        {
            Debug.Log(sn.mino);
        }
        txtNodeIndex.text = String.Format("���({0}/{1})",nodesIndex+1,nodes.Count);
    }

    public void ShowLandPoint()
    {
        DestroyAllChild(MinoParent);
        if (nodesIndex < 5)
        {

            ShowMino(nodes[nodesIndex].mino);
            txtNodeIndex.text = String.Format("���({0}/{1})\n{2}\n���֣�{3}", nodesIndex+1, nodes.Count, nodes[nodesIndex].mino.ToString(),nodes[nodesIndex].score);
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
            if (!pause)
            {
                if (time - aiTimer > 0.05f)
                {
                    aiTimer = time;
                    PutMino();

                    txtNodeIndex.text = game.statLine.ToString();
                }
            }

        }

    }

    void FixedUpdate()
    {
        
    }

}
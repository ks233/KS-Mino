using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    //Play.cs用来更新游戏内容，是游戏系统与玩家的桥梁。
    //在FixedUpdate中响应按键，根据按键对Game做出相应的操作，再从Game调取信息，显示在游戏界面上。

    public const int MAX_NEXT = 6;//最大NEXT数量

    //public GameObject ParentField;
    public GameObject[] MinoTiles;
    public GameObject[] Minoes;


    public KeyListener keyListener;

    public FieldUIDisplay fieldUI;
    public ActiveMinoUIDisplay activeMinoUI;
    public HoldUIDisplay holdUI;
    public NextUIDisplay nextUI;
    public AttackBar attackBar;

    public Image gameClearBoard;
    public GameObject gameClearBoardOBJ;

    private float clearMsgTimer;

    public Game game;
    private float gameTime;

    public Text TxtStats;
    public Text TxtClearMsg;
    public Text TxtGoal;

    public RectTransform goalLine;

    public int gameMode=0;

    public AIHintDisplay aIHintDisplay;
    public bool aiOn=false;

    public float aiTimer = 0;

    private int cellSize = 44;

    /*
     * 1=40L
     * 2=限高10格
     * 3=20TSD
     *
     *
     *
     *
     */



    public void Hold()
    {
        if (game.Hold() == 0)
        {
            UpdateHoldDisplay();
            UpdateNextDisplay();
        }
    }


    public void ShowGoalLine(int height)
    {
        if (height < 0)
        {
            goalLine.anchoredPosition = new Vector2Int(0, 0 + 2);
        }
        else if(height<=20)
        {
            goalLine.anchoredPosition = new Vector2Int(0, height * cellSize + 2);
        }
        else
        {
            goalLine.anchoredPosition = new Vector2Int(0, 20 * cellSize + 2);
        }
    }
    
    //OP前缀都是给KeyListener调用的

    public int OP_CWRotate()
    {
        return game.CWRotate();
    }
    public int OP_CCWRotate()
    {
        return game.CCWRotate();
    }
    public int OP_Move(int x)
    {
        return game.Move(x);
    }
    public int OP_Fall()
    {

        return game.Fall();
    }
    public void OP_HardDrop()
    {
        ClearType ct;
        int hdCells;
        game.Drop(out hdCells);//硬降
        if (Lock(out ct) > 0)
        {
            ShowClearMsg(ct.ToString());//显示消行信息

            attackBar.AddAttack(ct.GetAttack());

            if (ct.GetAttack() >= 4)
            {
                //特效
            }
        }
        game.NextMino();
        UpdateNextDisplay();
        UpdateActiveMinoDisplay();
        UpdateFieldDisplay();
        UpdateStats();
        AfterLock(ct);
    }
    public void OP_Lock()
    {
        ClearType ct;
        if (Lock(out ct) > 0)
        {
            ShowClearMsg(ct.ToString());//显示消行信息
            attackBar.AddAttack(ct.GetAttack());
        }
        //涨垃圾行
        game.NextMino();
        UpdateFieldDisplay();
        //activeMinoMoved = true;
        UpdateNextDisplay();
        AfterLock(ct);

    }

    public void AfterLock(ClearType ct)
    {
        UpdateGoalText();
        if (gameMode == 1)//40L
        {
            if (game.statLine >= 40)
            {
                game.GameClear();
                GameClear(gameTime.ToString("0.00"),false);
            }
        }
        UpdateAIHint();
    }

    public void UpdateAIHint()
    {
        if (aiOn)
        {
            /*
            SearchNode result = Search.GetLandPointsKick(game.field, game.activeMino)[0];
            SearchNode hold;
            if (game.GetHoldId() != 0)
            {
                hold = Search.GetLandPointsKick(game.field, new Mino(game.GetHoldId()))[0];
            }
            else
            {
                hold = Search.GetLandPointsKick(game.field, new Mino(game.GetNextSeq()[0]))[0];
            }


            if (result.score < hold.score)
            {
                aIHintDisplay.UpdateMino(hold.mino);
            }
            else
            {
                aIHintDisplay.UpdateMino(result.mino);
            }
            */


            aIHintDisplay.UpdateMino(zzztoj.GetMino(game.field, game.GetActiveMinoId(), game.GetNextSeq(), game.GetHoldId(),
                (game.wasB2B ? 1 : 0), game.combo, true, 0, 8));


        }
        else
        {
            aIHintDisplay.Clear();
        }
    }

    public void UpdateGoalLine()
    {
        if (gameMode == 1)//40L
        {
            ShowGoalLine(40 - game.statLine);
        }
    }

    public void UpdateGoalText()
    {
        if (gameMode == 1)//40L
        {
            TxtGoal.text = String.Format("<size=100>{0}</size>\n<size=40>Lines</size>", Math.Max(40 - game.statLine,0));
            ShowGoalLine(Math.Max(Math.Min(40 - game.statLine, 20),0));
        }
    }

    private void GameClear(string s,bool isGameOver)
    {
        if (keyListener.pause == false)
        {
            keyListener.pause = true;
            gameClearBoardOBJ.SetActive(true);
            if (isGameOver)
            {
                gameClearBoardOBJ.GetComponent<GameClearBoard>().SetGameOverText();
            }
            else
            {
                gameClearBoardOBJ.GetComponent<GameClearBoard>().SetText(s);
            }
        }
    }


    public float GetCurrentTime()
    {
        System.TimeSpan time = DateTime.Now.TimeOfDay;//秒数精确到浮点数
        float t = (float)time.TotalSeconds;//现在的时间
        return t;
    }


    public void UpdateStats()
    {
        int piece = game.statPiece;
        int atk = game.statAttack;

        String s = String.Format("Pieces\n<size=32>{0} </size>{1:0.00}/SEC\n\nAttacks\n<size=32>{2} </size>{3:0.00}/Min\n\nTime\n<size=32>{4:0.00}</size>",
            piece, piece / gameTime, atk, atk / gameTime * 60, gameTime);
        TxtStats.text = s;
    }

    public void UpdateActiveMinoDisplay()
    {
        activeMinoUI.UpdateActiveMino(game.field, game.activeMino);

    }


    public void UpdateHoldDisplay()
    {
        int holdId = game.GetHoldId();
        holdUI.UpdateHold(holdId);
    }

    public void UpdateNextDisplay()
    {
        int[] nextSeq = game.GetNextSeq();
        nextUI.UpdateNext(nextSeq, MAX_NEXT);
    }

    public void UpdateFieldDisplay()
    {
        fieldUI.UpdateField(game.field, game.activeMino);
    }


    void Start()
    {
        game = new Game();
        Restart();
    }
    public void Restart()
    {
        keyListener.pause = false;
        game.Restart();
        gameClearBoardOBJ.SetActive(false);
        gameTime = 0;
        UpdateGoalLine();
        UpdateGoalText();
        UpdateHoldDisplay();
        UpdateNextDisplay();
        UpdateFieldDisplay();
        UpdateActiveMinoDisplay();

        UpdateAIHint();

    }



    private void ShowClearMsg(string msg)
    {
        if (msg != "")
        {
            clearMsgTimer = GetCurrentTime();

            TxtClearMsg.text = msg;
        }
    }

    void Update()
    {
        if (!PauseMenu.GameIsPaused) gameTime += Time.deltaTime;



        if (game.Playing())
        {
            UpdateStats();
        }
        else if(game.gameState == Game.State.GameOver)
        {
            GameClear("",true);
        }

        if (GetCurrentTime() - clearMsgTimer >= 2 && clearMsgTimer != 0)
        {
            TxtClearMsg.text = "";
            clearMsgTimer = 0;
        }
        if (Input.GetKeyDown("h"))
        {
            aiOn = !aiOn;
            UpdateAIHint();
        }
        
        aiTimer += Time.deltaTime;
        if (aiTimer > 0.1f&& game.Playing())
        {
            zzztest();
            aiTimer = 0;
        }
        //f.SetField(game.field);
    }


    private string path="";
    private int pathindex=0;
    public void zzztest()
    {
        if (pathindex >= path.Length)
        {
            path = zzztoj.GetPath(game.field, game.GetActiveMinoId(), game.GetNextSeq(), game.GetHoldId(),
            (game.wasB2B ? 1 : 0), game.combo, true, 0, 4);
            pathindex = 0;
        }
        switch (path[pathindex++])
        {
            case 'l': game.Move(-1); break;
            case 'r': game.Move(1); break;
            case 'd': game.Fall(); break;
            case 'L': while (game.Move(-1) == 0) { } break;
            case 'R': while (game.Move(1) == 0) { } break;
            case 'D': while (game.Fall() == 0) { } break;
            case 'V': OP_HardDrop(); break;
            case 'v':
                {
                    Hold();
                    break;

                }
            case 'z': game.CCWRotate(); break;
            case 'c': game.CWRotate(); break;
        }
        UpdateActiveMinoDisplay();


        /*
        path = zzztoj.GetPath(game.field, game.GetActiveMinoId(), game.GetNextSeq(), game.GetHoldId(),
                (game.wasB2B ? 1 : 0), game.combo, true,0, 8);
        Debug.Log(path);
        foreach (char c in path)
        {
            switch (c)
            {
                case 'l': game.Move(-1); break;
                case 'r': game.Move(1); break;
                case 'd': game.Fall(); break;
                case 'L': while (game.Move(-1) == 0) { } break;
                case 'R': while (game.Move(1) == 0) { } break;
                case 'D': while (game.Fall() == 0) { } break;
                case 'V': OP_HardDrop(); break;
                case 'v':
                    {
                        Hold();
                        break;

                    }
                case 'z': game.CCWRotate(); break;
                case 'c': game.CWRotate(); break;
            }
        }
        */
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "zzztest"))
        {
            
            zzztest();
        }
    }

    private int Lock(out ClearType ct)
    {
        return game.LockMino(out ct);

    }

}
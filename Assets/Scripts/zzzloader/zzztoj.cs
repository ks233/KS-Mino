using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class zzztoj
{


    public static string GetAIName()
    {
        return Marshal.PtrToStringAnsi(zzzdll.AIName(8));
    }
    /*
    public static string GetPath()
    {
        int[] overfield = { 0, 0, 0, 0, 0, 0, 0, 0 };//场地的第23~30行
        int[] field = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0b1110000000, 0b1111000110, 0b1111110110 };//场地0~22行，从后往前
        int field_w = 10;            //宽度
        int field_h = 22;           //高度
        int b2b = 0;                //b2b数
        int combo = 0;              //连击数
        char[] next = { 'O', 'T' };  //Next序列
        char hold = 'T';            //Hold
        bool curCanHold = true;     //当前hold
        char active = 'T';          //当前块
        int x = 5;                  //x
        int y = 20;                 //y
        int spin = 1;               //朝向
        bool canhold = true;        //允许hold
        bool can180spin = false;    //允许180
        int upcomeAtt = 0;          //攻击等待条
        int[] comboTable = { 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 4, 4, 5, -1 };//连击表，以-1结尾
        int maxDepth = 2;           //next数组的长度
        int level = 1;              //这是啥
        int player = 1;             //玩家数量？

        IntPtr AIPath = zzzdll.TetrisAI(overfield, field, field_w, field_h, b2b, combo, next, hold, curCanHold, active, x, y, spin, canhold, can180spin, upcomeAtt, comboTable, maxDepth, level, player);

        string result = Marshal.PtrToStringAnsi(AIPath);
        return result;
    }*/
    public static string GetPath(Field _field,int _minoId,int[] _next,int _hold,int _b2b,int _combo,bool _curCanHold,int _upcomeAtt,int depth)
    {
        int[] overfield = new int[8];//场地的第23~30行
        int[] field = new int[24];//【场地】0~22行，从后往前
        int[] binArr = _field.BinaryArray();
        for (int i = 0; i < 23; i++)//不这么写unity会炸
        {
            field[i] = binArr[i];
        }
        int field_w = 10;            //宽度
        int field_h = 22;           //高度
        int b2b = _b2b;                //【b2b数】
        int combo = _combo;              //【连击数】
        string next = "";  //【Next序列】
        for(int i = 0; i < 8; i++)
        {
            next += Mino.IdToName(_next[i]);
        }
        char hold = Mino.IdToName(_hold);           //【Hold】
        bool curCanHold = _curCanHold;                   //当前hold
        char active = Mino.IdToName(_minoId);       //当前块
        int x = 3;                  //x
        int y = 1;                 //y
        int spin = 0;               //朝向
        bool canhold = true;        //允许hold
        bool can180spin = false;    //允许180
        int upcomeAtt = _upcomeAtt;          //【攻击等待条】
        int[] comboTable = { 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 4, 4, 5, -1 };//连击表，以-1结尾
        //int[] comboTable =   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1 };//连击表，以-1结尾
        //int[] comboTable = { 0, -1 };//连击表，以-1结尾
        int maxDepth = 8;           //【AI最大深度，next数组的长度】
        int level = depth;              //AI深度
        int player = 0;             //bot id
        IntPtr AIPath = zzzdll.TetrisAI(overfield, field, field_w, field_h, b2b, combo,
            next, hold, curCanHold, active, x, y, spin, canhold, can180spin, upcomeAtt, comboTable, maxDepth, level, player);

        string result = Marshal.PtrToStringAnsi(AIPath);
        Debug.Log(result);
        return result;
    }



    public static Mino GetMino(Field _field, int _minoId, int[] _next, int _hold, int _b2b, int _combo, bool _curCanHold, int _upcomeAtt, int depth)
    {
        Mino mino = new Mino(_minoId);
        mino.position = new Vector2Int(4,19);
        string path = GetPath(_field,_minoId,_next,_hold,_b2b,_combo,_curCanHold,_upcomeAtt,depth);

        /*
        *      'l': 左移一格
        *      'r': 右移一格
        *      'd': 下移一格
        *      'L': 左移到头
        *      'R': 右移到头
        *      'D': 下移到底（但不粘上，可继续移动）
        *      'z': 逆时针旋转
        *      'c': 顺时针旋转
        * 字符串末尾要加'\0'，表示落地操作（或硬降落）
        */
        Game game = new Game();
        game.field = _field.Clone();
        game.activeMino = mino;
        game.SetHoldID(_hold);
        Debug.Log(_hold);
        
        /*
        int[] arr = _field.BinaryArray();
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i] + ",";
        }
        Debug.Log(s + "\n" + path);

        */

        foreach (char c in path)
        {
            switch(c)
            {
                case 'l': game.Move(-1); break;
                case 'r': game.Move(1); break;
                case 'd': game.Fall(); break;
                case 'L': while (game.Move(-1) == 0) { } break;
                case 'R': while (game.Move(1) == 0) { } break;
                case 'D': while (game.Fall() == 0) { } break;
                case 'V': while (game.Fall() == 0) { } break;
                case 'v': {
                        game.Hold();
                        break;
                    
                    }
                case 'z': game.CCWRotate(); break;
                case 'c': game.CWRotate(); break;
            }
        }
        return game.activeMino;
    }


}

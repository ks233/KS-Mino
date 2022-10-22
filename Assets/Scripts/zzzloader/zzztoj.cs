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
        int[] overfield = { 0, 0, 0, 0, 0, 0, 0, 0 };//���صĵ�23~30��
        int[] field = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            0b1110000000, 0b1111000110, 0b1111110110 };//����0~22�У��Ӻ���ǰ
        int field_w = 10;            //���
        int field_h = 22;           //�߶�
        int b2b = 0;                //b2b��
        int combo = 0;              //������
        char[] next = { 'O', 'T' };  //Next����
        char hold = 'T';            //Hold
        bool curCanHold = true;     //��ǰhold
        char active = 'T';          //��ǰ��
        int x = 5;                  //x
        int y = 20;                 //y
        int spin = 1;               //����
        bool canhold = true;        //����hold
        bool can180spin = false;    //����180
        int upcomeAtt = 0;          //�����ȴ���
        int[] comboTable = { 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 4, 4, 5, -1 };//��������-1��β
        int maxDepth = 2;           //next����ĳ���
        int level = 1;              //����ɶ
        int player = 1;             //���������

        IntPtr AIPath = zzzdll.TetrisAI(overfield, field, field_w, field_h, b2b, combo, next, hold, curCanHold, active, x, y, spin, canhold, can180spin, upcomeAtt, comboTable, maxDepth, level, player);

        string result = Marshal.PtrToStringAnsi(AIPath);
        return result;
    }*/
    public static string GetPath(Field _field,int _minoId,int[] _next,int _hold,int _b2b,int _combo,bool _curCanHold,int _upcomeAtt,int depth)
    {
        int[] overfield = new int[8];//���صĵ�23~30��
        int[] field = new int[24];//�����ء�0~22�У��Ӻ���ǰ
        int[] binArr = _field.BinaryArray();
        for (int i = 0; i < 23; i++)//����ôдunity��ը
        {
            field[i] = binArr[i];
        }
        int field_w = 10;            //���
        int field_h = 22;           //�߶�
        int b2b = _b2b;                //��b2b����
        int combo = _combo;              //����������
        string next = "";  //��Next���С�
        for(int i = 0; i < 8; i++)
        {
            next += Mino.IdToName(_next[i]);
        }
        char hold = Mino.IdToName(_hold);           //��Hold��
        bool curCanHold = _curCanHold;                   //��ǰhold
        char active = Mino.IdToName(_minoId);       //��ǰ��
        int x = 3;                  //x
        int y = 1;                 //y
        int spin = 0;               //����
        bool canhold = true;        //����hold
        bool can180spin = false;    //����180
        int upcomeAtt = _upcomeAtt;          //�������ȴ�����
        int[] comboTable = { 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 4, 4, 5, -1 };//��������-1��β
        //int[] comboTable =   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1 };//��������-1��β
        //int[] comboTable = { 0, -1 };//��������-1��β
        int maxDepth = 8;           //��AI�����ȣ�next����ĳ��ȡ�
        int level = depth;              //AI���
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
        *      'l': ����һ��
        *      'r': ����һ��
        *      'd': ����һ��
        *      'L': ���Ƶ�ͷ
        *      'R': ���Ƶ�ͷ
        *      'D': ���Ƶ��ף�����ճ�ϣ��ɼ����ƶ���
        *      'z': ��ʱ����ת
        *      'c': ˳ʱ����ת
        * �ַ���ĩβҪ��'\0'����ʾ��ز�������Ӳ���䣩
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

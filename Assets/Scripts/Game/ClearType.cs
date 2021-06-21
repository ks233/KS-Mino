using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ClearType
{
    public int minoId;
    public int lines;
    public bool tSpin;
    public int tSpinType;//0mini 1single 2double 3triple
    public bool pc;
    public bool wasB2b;
    public int combo;


    public static int[] comboList = { 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 4, 4, 5 };
    private const int COMBO_LIST_LEN = 12;
    private int[] clearList = { 0, 0, 1, 2, 4 };

    private static int[] tspinList = { 0, 2, 4, 6 };//mini single double triple


    public ClearType(int minoId,int clearLines, bool isTSpin, int tType,  bool isPc,int clearCombo, bool isB2b)
    {
        this.minoId = minoId;
        lines = clearLines;
        tSpin = isTSpin;
        tSpinType = tType;
        pc = isPc;
        combo = clearCombo;
        wasB2b = isB2b;
    }

    public ClearType()
    {
        lines = 0;
        combo = 0;
        wasB2b = false;
        tSpin = false;
        tSpinType = -1;
        pc = false;

    }

    public ClearType Clone()
    {
        return (ClearType)this.MemberwiseClone();
    }




    public int GetEvalScore()//AI评估用分数
    {
        if (lines == 0) return 0;
        return lines * 76;
        int score = 0;
        bool isB2b = GetIsB2b();
        if (tSpin)
        {
            switch (tSpinType)
            {
                case 0: score += 400; break;
                case 1: score += 600; break;
                case 2: score += 1000; break;
                case 3: score += 1200; break;
            }
        }
        switch (lines)
        {
            //case 0: score += 0; break;
            case 1: score += 500; break;
            case 2: score += 600; break;
            case 3: score += 800; break;
            case 4: score += 1600; break;
        }
        if (wasB2b)
        {
            if (isB2b) score += 500;
        }
        if (combo > 0)
        {
            score += combo * 200;
        }

        
        return score;
    }


    public float GetScore(int level) {//游戏分数
        if (lines == 0) return 0;

        float score = 0;
        bool isB2b = (lines == 4 || tSpin);
        if (tSpin)
        {
            switch (tSpinType)
            {
                case 0: score += 200; break;        //mini
                case 1: score += 800; break;        //t1
                case 2: score += 1200; break;       //t2
                case 3: score += 1600; break;       //t3
            }
        }
        else
        {
            switch (lines)
            {
                case 1: score = 100; break;         //1
                case 2: score = 300; break;         //2
                case 3: score = 500; break;         //3
                case 4: score = 800; break;         //tetris
            }
        }
        if (isB2b && wasB2b) score *= 1.5f;         //b2b
        if (combo > 0) score += 50 * combo;         //combo
        if (pc)                                     //pc
        {
            score += 3000;
        }
        return score * level;
    }

    public override string ToString()
    {
        bool isB2b = (lines == 4 || tSpin);
        string msg = "";
        if (combo > 0) msg += combo + " Ren\n";
        if (isB2b && wasB2b) msg += "Back To Back\n";
        if (tSpin)
        {
            msg += "T-Spin";
            switch (tSpinType)
            {
                case 0: msg += " Mini"; break;
                case 1: msg += " Single"; break;
                case 2: msg += " Double"; break;
                case 3: msg += " Triple"; break;
            }
        }
        else
        {
            switch (lines)
            {
                case 1: msg += " Single"; break;
                case 2: msg += " Double"; break;
                case 3: msg += " Triple"; break;
                case 4: msg += " Tetris"; break;
            }
        }
        if (pc)
        {
            msg += "\nPerfect Clear";
        }

        return msg;
    }

    public string ToMissionString()
    {
        string s = "";
        if (lines == 4) s = "Tetris";
        if (tSpin)
        {
            s = "T-Spin";
            switch (tSpinType)
            {
                case 0: s += " Mini"; break;
                case 1: s += " Single"; break;
                case 2: s += " Double"; break;
                case 3: s += " Triple"; break;
            }
            return s;
        }
        if(combo > 0)
        {
            s = combo + "Combo";
        }
        if (pc)
        {
            s = "Perfect Clear";
        }

        return s;
    }

    public bool GetIsB2b()
    {
        return lines == 4 || tSpin;
     
    }
    public int GetAttack()
    {
        
        int atk = 0;
        if(combo>0)
            atk += combo > COMBO_LIST_LEN ? comboList[COMBO_LIST_LEN] :comboList[combo];
        if (GetIsB2b() && wasB2b)
            atk++;
        if (!tSpin)
        {
            atk += clearList[lines];
        }
        else
        {
            atk += tspinList[tSpinType];
        }
        if (pc) atk += 10;

        return atk;

    }


}

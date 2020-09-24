using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearType
{
    public int lines;
    public int combo;
    public bool wasB2b;
    public bool tSpin;
    public int tSpinType;//默认-1，0mini 1single 2double 3triple
    public bool pc;

    private bool isB2b;
    private int[] comboList = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5 };
    private int[] clearList = { 0, 0, 1, 2, 4 };

    private int[] tspinList = { 0, 2, 4, 6 };


    public ClearType(int clearLines, int clearCombo, bool isB2b, bool isTSpin, int tType, bool isPc)
    {
        lines = clearLines;
        combo = clearCombo;
        wasB2b = isB2b;
        tSpinType = tType;
        isB2b = false;
        tSpin = isTSpin;
        pc = isPc;
    }

    public ClearType()
    {
        lines = 0;
        combo = 0;
        wasB2b = false;
        isB2b = false;
        tSpin = false;
        tSpinType = -1;
        pc = false;

    }

    public string ClearMessage()
    {
        isB2b = (lines == 4 || tSpin);
        string msg = "";
        if (combo > 0) msg += combo + "Ren";
        if (isB2b && wasB2b) msg += "Back To Back";
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
                case 4: msg += " 很有精神！！！"; break;
            }
        }
        if (pc)
        {
            msg += "\nPerfect Clear";
        }

        return msg;
    }

    public bool GetIsB2b()
    {
        isB2b = (lines == 4 || tSpin);
        return isB2b;
    }
    public int GetAttack()
    {
        isB2b = (lines == 4 || tSpin);
        int atk = 0;
        atk += comboList[combo];
        if (isB2b && wasB2b) atk++;
        if (!tSpin)
        {
            atk += clearList[lines];

        }
        else
        {
            atk += tspinList[lines];
        }
        if (pc) atk += 6;

        return atk;

    }


}

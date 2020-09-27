using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Fumen
{
    public int[,] customField;
    public string minoSequence;
    public bool useCustomNext;
    public bool isHoldAllowed;
    public List<Mino> guideList;
    public List<ClearType> missionList;
    public string tips;
    public List<int> field;

    public Fumen()
    {
        customField = new int[10, 40];
        minoSequence = "";
        useCustomNext = true;
        isHoldAllowed = true;
        guideList = new List<Mino>();
        missionList = new List<ClearType>();
        tips = "";
        field = new List<int>();
    }

    public void SetField(Field f)
    {
        customField = f.array.Clone() as int[,];
        field = customField.Cast<int>().ToList();
    }

    public void ListToField()
    {
        for(int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                customField[i, j] = field[40 * i + j];
            }
        }
    }
}

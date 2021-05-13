using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mino
{

    //ö�٣�����id
    public enum MinoID { S, Z, L, J, T, O, I }

    //��������ԣ����顢����id����תid��λ��
    public int[,] array;
    public MinoID id;
    public int rotation; //0~3��ʾ�ĸ�����
    public Vector2Int position;

    //7�ֿ������
    private static readonly int[,] arrayS = new int[,] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
    private static readonly int[,] arrayZ = new int[,] { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
    private static readonly int[,] arrayL = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 1 } };
    private static readonly int[,] arrayJ = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 0, 0 } };
    private static readonly int[,] arrayT = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
    private static readonly int[,] arrayO = new int[,] { { 1, 1 }, { 1, 1 } };
    private static readonly int[,] arrayI = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };

    public Mino()               //���������Ĺ��캯��,Ĭ�ϳ�ʼ��һ��S��
    {
        id = MinoID.S;
        array = new int[,] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
        position = new Vector2Int(0, 0);

    }
    public Mino(MinoID _id)     //����ָ��ID�Ŀ�
    {
        id = _id;
        rotation = 0;
        position = new Vector2Int(0, 0);
        SetArray(_id, 0);
    }

    public Mino(int _id)     //����ָ��ID�Ŀ�
    {
        id = IntToMinoID(_id);
        rotation = 0;
        position = new Vector2Int(0, 0);
        SetArray(id, 0);
    }


    public int GetRotationId()
    {
        return rotation;
    }

    public void SetRotationId(int r)
    {
        rotation = r;
    }

    public void Move(Vector2Int delta) {
        position += delta;
    
    }

    public Vector2Int GetPosition() {
        return position;
    }
    public void SetArray(MinoID id, int rotationID)         //��ʼ��ID��Ӧ������
    {

        switch (id)
        {
            case MinoID.S: array = arrayS.Clone() as int[,]; break;
            case MinoID.Z: array = arrayZ.Clone() as int[,]; break;
            case MinoID.L: array = arrayL.Clone() as int[,]; break;
            case MinoID.J: array = arrayJ.Clone() as int[,]; break;
            case MinoID.T: array = arrayT.Clone() as int[,]; break;
            case MinoID.O: array = arrayO.Clone() as int[,]; break;
            case MinoID.I: array = arrayI.Clone() as int[,]; break;
        }
        for (int i = 0; i < (4 - rotationID) % 4; i++)
        {
            array = Game.RotateMatrix(array, GetSize());
        }
    }

    public void CWRotate()      //˳ʱ����ת
    {
        rotation = (rotation + 1) % 4;

        int size = GetSize();
        array = Game.RotateMatrix(array, size);
        array = Game.RotateMatrix(array, size);
        array = Game.RotateMatrix(array, size);
    }

    public void CCWRotate()     //��ʱ����ת
    {
        rotation = (rotation + 3) % 4;

        array = Game.RotateMatrix(array, GetSize());
    }

    public int GetSize()       //��ȡ�������ߴ�
    {
        if (id == MinoID.I) { return 5; }
        else if (id == MinoID.O)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    public string GetName() {
        return IdToName(MinoIDToInt(id));
    }
    public int GetIdInt()
    {
        return MinoIDToInt(id);
    }
    public static MinoID IntToMinoID(int i) {
        return (MinoID)(i - 1);
    }
    public static int MinoIDToInt(MinoID i)
    {
        return (int)i + 1;

    }

    public override String ToString() {
        return GetName() + " " + GetPosition().ToString() + " " + GetRotationId();
    }

    public static string IdToName(int id)
    {
        string name = "";
        switch (id)
        {
            case 1: name = "S"; break;
            case 2: name = "Z"; break;
            case 3: name = "L"; break;
            case 4: name = "J"; break;
            case 5: name = "T"; break;
            case 6: name = "O"; break;
            case 7: name = "I"; break;
        }
        return name;
    }
    public static int NameToId(string name)
    {
        int id = -1;
        switch (name)
        {
            case "S": id = 1; break;
            case "Z": id = 2; break;
            case "L": id = 3; break;
            case "J": id = 4; break;
            case "T": id = 5; break;
            case "O": id = 6; break;
            case "I": id = 7; break;
        }
        return id;
    }


    public void Fall()
    {
        position.y--;
    }

    public void Move(int x)
    {
        position.x += x;
    }

}

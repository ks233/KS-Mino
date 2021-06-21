using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mino
{

    //枚举：方块id
    public enum MinoID { S, Z, L, J, T, O, I }

    //对象的属性：数组、方块id、旋转id、位置
    public int[,] array;
    public MinoID id;
    public int rotation; //0~3表示四个方向
    public Vector2Int position;

    //7种块的数组
    private static readonly int[,] arrayS = new int[,] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
    private static readonly int[,] arrayZ = new int[,] { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
    private static readonly int[,] arrayL = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 1 } };
    private static readonly int[,] arrayJ = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 0, 0 } };
    private static readonly int[,] arrayT = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
    private static readonly int[,] arrayO = new int[,] { { 1, 1 }, { 1, 1 } };
    private static readonly int[,] arrayI = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };

    public override bool Equals(object obj)
    {
        if (obj.GetType() == this.GetType())
        {
            Mino m = obj as Mino;
            return m.id == id && m.position.x == position.x && m.position.y == position.y && m.rotation == rotation;
        }

        return base.Equals(obj);
    }

    public Mino Clone()
    {
        return (Mino)MemberwiseClone();
    }

    public Mino()               //不带参数的构造函数,默认初始化一个S块
    {
        id = MinoID.S;
        array = new int[,] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
        position = new Vector2Int(0, 0);

    }
    public Mino(MinoID _id)     //构造指定ID的块
    {
        id = _id;
        rotation = 0;
        position = new Vector2Int(0, 0);
        SetArray(_id, 0);
    }

    public Mino(int _id)     //构造指定ID的块
    {
        id = IntToMinoID(_id);
        rotation = 0;
        position = new Vector2Int(4,19);
        SetArray(id, 0);
    }


    public Mino(int _id,int x,int y,int _rotation)     //构造指定ID的块
    {
        id = IntToMinoID(_id);
        rotation = _rotation;
        position = new Vector2Int(x, y);
        SetArray(id, rotation);
    }

    //反向但是重叠的方块，比如向上的S块和向下并且Y+1的S块
    public Mino OppositeMino()
    {
        if(id == MinoID.S || id == MinoID.Z)
        {
            switch (rotation)
            {
                case 0: return new Mino(GetIdInt(), GetPosition().x, GetPosition().y + 1, 2);
                case 2: return new Mino(GetIdInt(), GetPosition().x, GetPosition().y - 1, 0);
                case 1: return new Mino(GetIdInt(), GetPosition().x+1, GetPosition().y, 3);
                case 3: return new Mino(GetIdInt(), GetPosition().x-1, GetPosition().y, 1);
            }
        }else if(id == MinoID.I)
        {
            switch (rotation)
            {
                case 0: return new Mino(GetIdInt(), GetPosition().x + 1, GetPosition().y, 2);
                case 2: return new Mino(GetIdInt(), GetPosition().x - 1, GetPosition().y, 0);
                case 1: return new Mino(GetIdInt(), GetPosition().x, GetPosition().y - 1, 3);
                case 3: return new Mino(GetIdInt(), GetPosition().x, GetPosition().y + 1, 1);
            }
        }
        return this;
    }
    public int GetRotationId()
    {
        return rotation;
    }

    public void SetPosition(int x, int y) {
        position.x = x;
        position.y = y;
    }

    public void SetPosition(Vector2Int v) 
    {
        position = v;
    }

    public void SetRotation(int r)
    {
        rotation = r;
        SetArray(id, rotation);
    }

    public void Move(Vector2Int delta) {
        position += delta;
    
    }

    public Vector2Int GetPosition() {
        return position;
    }

    public Vector3 GetPosition3()
    {
        return new Vector3(position.x,position.y,0);
    }
    public void SetArray(MinoID id, int rotationID)         //初始化ID对应的数组
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

    public void CWRotate()      //顺时针旋转
    {
        rotation = (rotation + 1) % 4;

        int size = GetSize();
        array = Game.RotateMatrix(array, size);
        array = Game.RotateMatrix(array, size);
        array = Game.RotateMatrix(array, size);
    }

    public void CCWRotate()     //逆时针旋转
    {
        rotation = (rotation + 3) % 4;

        array = Game.RotateMatrix(array, GetSize());
    }

    public int GetSize()       //获取块的数组尺寸
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

    public char GetName() {
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

    public static char IdToName(int id)
    {
        char name = ' ';
        switch (id)
        {
            case 0: name = ' '; break;
            case 1: name = 'S'; break;
            case 2: name = 'Z'; break;
            case 3: name = 'L'; break;
            case 4: name = 'J'; break;
            case 5: name = 'T'; break;
            case 6: name = 'O'; break;
            case 7: name = 'I'; break;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino
{

    public int[,] array;
    public int size;
    public string name;
    public int rotation;
    public int id;
    public Vector2Int position;


    public Mino()
    {

    }

    public Mino(int minoId)
    {


        Mino S = new Mino();
        Mino Z = new Mino();
        Mino L = new Mino();
        Mino J = new Mino();
        Mino T = new Mino();
        Mino O = new Mino();
        Mino I = new Mino();
        S.array = new int[,] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
        S.size = 3;
        S.name = "S";
        S.rotation = 0;
        S.id = 1;

        Z.array = new int[,] { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
        Z.size = 3;
        Z.name = "Z";
        Z.rotation = 0;
        Z.id = 2;

        L.array = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 1 } };
        L.size = 3;
        L.name = "L";
        L.rotation = 0;
        L.id = 3;

        J.array = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 0, 0 } };
        J.size = 3;
        J.name = "J";
        J.rotation = 0;
        J.id = 4;

        T.array = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
        T.size = 3;
        T.name = "T";
        T.rotation = 0;
        T.id = 5;

        O.array = new int[,] { { 1, 1 }, { 1, 1 } };
        O.size = 2;
        O.name = "O";
        O.rotation = 0;
        O.id = 6;

        I.array = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
        I.size = 5;
        I.name = "I";
        I.rotation = 0;
        I.id = 7;
        switch (minoId)
        {
            case 1: SetMino(S); break;
            case 2: SetMino(Z); break;
            case 3: SetMino(L); break;
            case 4: SetMino(J); break;
            case 5: SetMino(T); break;
            case 6: SetMino(O); break;
            case 7: SetMino(I); break;
        }

    }

    private void SetMino(Mino m)
    {
        this.array = m.array;
        this.size = m.size;
        this.name = m.name;
        this.rotation = m.rotation;
        this.id = m.id;
        //Debug.LogFormat("·½¿éid{0}", this.id);
    }

    public string IdToName(int id)
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
    public int NameToId(string name)
    {
        int id = -1;
        switch (name)
        {
            case "S":id = 1; break;
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

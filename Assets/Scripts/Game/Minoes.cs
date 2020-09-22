using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minoes
{


    public struct Mino
    {
        public int[,] array;
        public int size;
        public string name;
        public int rotation;
        public int id;
        public Vector2Int position;
    };

    private static Mino S, Z, L, J, T, O, I;

    static Minoes()
    {
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
    }
    public Mino GetMino(int minoId)
    {
        

        Mino m = new Mino();
        switch (minoId)
        {
            case 1: m = S; break;
            case 2: m = Z; break;
            case 3: m = L; break;
            case 4: m = J; break;
            case 5: m = T; break;
            case 6: m = O; break;
            case 7: m = I; break;
        }

        return m;
    }

}

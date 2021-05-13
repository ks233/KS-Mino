using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldVisualize : MonoBehaviour
{
    public Grid grid;
    public Tile[] minoTile;
    public Tile black;
    public Tilemap fieldTileMap;
    public Tilemap nextTileMap;
    public Tilemap holdTileMap;
    public Camera c;
    public int MAX_NEXT = 5;

    [HideInInspector]
    public Field f;

    // Start is called before the first frame update
    void Start()
    {
        f = new Field();
        Debug.Log(fieldTileMap.WorldToLocal(new Vector3Int(0, 0, 0)));
    }

    public void SetField(Field field)
    {
        f = field;
    }
    // Update is called once per frame
    void Update()
    {
        //if (!gameScript.gameover)


        UpdateField();

    }
    void UpdateField()
    {
        int[,] field = f.array;
        fieldTileMap.ClearAllTiles();
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector3Int pos = new Vector3Int(j, i, 0);
                if (field[j, i] == 0)
                {
                    //fieldTileMap.SetTile(pos, black);
                }
                if (field[j, i] > 0 && field[j, i] < 20)
                {
                    fieldTileMap.SetTile(pos, minoTile[field[j, i] - 1]);
                }
                if (field[j, i] < 0)
                {
                    fieldTileMap.SetTile(pos, minoTile[-field[j, i] - 1]);
                }
                if (field[j, i] > 20)
                {

                    fieldTileMap.SetTile(pos, minoTile[field[j, i] - 21]);
                    fieldTileMap.SetTileFlags(pos, TileFlags.None);

                    Color tmp = fieldTileMap.GetColor(pos);
                    tmp.a = 0.4f;
                    fieldTileMap.SetColor(pos, tmp);
                }

            }
        }
    }
    public void UpdateNextTilemap(NextManager n)
    {

        nextTileMap.ClearAllTiles();
        Vector3Int nextPos = new Vector3Int(0, 0, 0);
        Queue<int> next = n.nextQueue;
        int count = 0;
        foreach (int id in next)
        {
            if (count >= MAX_NEXT) break;
            Mino mi = new Mino();
            Mino m = new Mino((Mino.MinoID)id);

            if (m.GetSize() == 2) nextPos.y += 1;
            if (m.GetSize() == 5) nextPos.x -= 1;

            for (int i = 0; i < m.GetSize(); i++)
            {
                for (int j = 0; j < m.GetSize(); j++)
                {
                    if (m.array[i, j] == 1) nextTileMap.SetTile(nextPos + new Vector3Int(j, i, 0), minoTile[(int)m.id - 1]);
                }
            }
            if (m.GetSize() == 5)
            {
                nextPos.x += 1;
                nextPos.y -= 3;
            }
            else if (m.GetSize() == 2) nextPos.y -= 5;
            else nextPos.y -= 4;
            count++;
        }
    }
    public void UpdateHoldTilemap(int holdId)
    {

        Vector3Int holdPos = new Vector3Int(0, 0, 0);
        Mino mi = new Mino();
        holdTileMap.ClearAllTiles();
        if (holdId != 0)
        {
            if (holdId == 7) holdPos.x--;
            else if (holdId == 6) holdPos.y++;
            Mino m = new Mino((Mino.MinoID)holdId);
            for (int i = 0; i < m.GetSize(); i++)
            {
                for (int j = 0; j < m.GetSize(); j++)
                {
                    if (m.array[i, j] == 1) holdTileMap.SetTile(new Vector3Int(j, i, 0) + holdPos, minoTile[(int)m.id - 1]);
                }
            }
        }


    }

}

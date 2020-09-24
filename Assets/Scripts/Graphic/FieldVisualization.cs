using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldVisualization : MonoBehaviour
{
    public Game gameScript;
    public NextManager nextManager;
    public GameObject fieldColour;
    public Grid grid;
    public Tile[] minoTile;
    public Tile black;
    public Tilemap fieldTileMap;
    public Tilemap nextTileMap;
    public Tilemap holdTileMap;
    public Camera c;
    public int MAX_NEXT = 5;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(fieldTileMap.WorldToLocal(new Vector3Int(0, 0, 0)));
    }

    // Update is called once per frame
    void Update()
    {
        //if (!gameScript.gameover)
        {
            int[,] field = gameScript.field.array;
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


    }

    public void UpdateNextTilemap()
    {

        nextTileMap.ClearAllTiles();
        Vector3Int nextPos = new Vector3Int(0, 0, 0);
        Queue<int> next = gameScript.next.nextQueue;
        string s = "";
        int count = 0;
        foreach (int id in next)
        {
            if (count >= MAX_NEXT) break;
            Mino mi = new Mino();
            Mino m = new Mino(id);
            s += m.name;

            if (m.size == 2) nextPos.y += 1;
            if (m.size == 5) nextPos.x -= 1;

            for (int i = 0; i < m.size; i++)
            {
                for (int j = 0; j < m.size; j++)
                {
                    if (m.array[i, j] == 1) nextTileMap.SetTile(nextPos + new Vector3Int(j, i, 0), minoTile[m.id - 1]);
                }
            }
            if (m.size == 5)
            {
                nextPos.x += 1;
                nextPos.y -= 3;
            }
            else if (m.size == 2) nextPos.y -= 5;
            else nextPos.y -= 4;
            count++;
        }
        //Debug.Log(s);
    }
    public void UpdateHoldTilemap()
    {
        
        Vector3Int holdPos = new Vector3Int(0, 0, 0);
        Mino mi = new Mino();
        int holdId = gameScript.hold;
        holdTileMap.ClearAllTiles();
        if (holdId != 0)
        {
            if (holdId == 7) holdPos.x--;
            else if (holdId == 6) holdPos.y++;
            Mino m = new Mino(holdId);
            for (int i = 0; i < m.size; i++)
            {
                for (int j = 0; j < m.size; j++)
                {
                    if (m.array[i, j] == 1) holdTileMap.SetTile(new Vector3Int(j, i, 0)+holdPos, minoTile[m.id - 1]);
                }
            }
        }

        
    }
 
}

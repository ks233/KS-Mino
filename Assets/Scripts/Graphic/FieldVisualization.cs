using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldVisualization : MonoBehaviour
{
    public Game gameScript;
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
    void FixedUpdate()
    {
        if (!gameScript.gameover)
        {
            fieldTileMap.ClearAllTiles();
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector3Int pos = new Vector3Int(j, i, 0);
                    if (gameScript.field[j, i] == 0)
                    {
                        //fieldTileMap.SetTile(pos, black);
                    }
                    if (gameScript.field[j, i] > 0 && gameScript.field[j, i] < 20)
                    {
                        fieldTileMap.SetTile(pos, minoTile[gameScript.field[j, i] - 1]);
                    }
                    if (gameScript.field[j, i] < 0)
                    {
                        fieldTileMap.SetTile(pos, minoTile[-gameScript.field[j, i] - 1]);
                    }
                    if (gameScript.field[j, i] > 20)
                    {

                        fieldTileMap.SetTile(pos, minoTile[gameScript.field[j, i] - 21]);
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
        Queue<int> next = gameScript.nextQueue;
        string s = "";
        int count = 0;
        foreach (int id in next)
        {
            if (count >= MAX_NEXT) break;
            Minoes mi = new Minoes();
            Minoes.Mino m = mi.GetMino(id);
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
        Debug.Log(s);
    }
    public void UpdateHoldTilemap()
    {
        
        Vector3Int holdPos = new Vector3Int(0, 0, 0);
        Minoes mi = new Minoes();
        int holdId = gameScript.hold;
        holdTileMap.ClearAllTiles();
        if (holdId != 0)
        {
            if (holdId == 7) holdPos.x--;
            else if (holdId == 6) holdPos.y++;
            Minoes.Mino m = mi.GetMino(holdId);
            for (int i = 0; i < m.size; i++)
            {
                for (int j = 0; j < m.size; j++)
                {
                    if (m.array[i, j] == 1) holdTileMap.SetTile(new Vector3Int(j, i, 0)+holdPos, minoTile[m.id - 1]);
                }
            }
        }

        
    }
    void MouseMove()
    {

        var mousePos = Input.mousePosition;
        Vector3 pos = c.ScreenToWorldPoint(mousePos - c.transform.position);
        pos.y += 0.35f;
        Vector3Int posInt = grid.WorldToCell(new Vector3(pos.x, pos.y, 0));
        //Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {1}]", posInt.x, posInt.y));
        TileBase tile = fieldTileMap.GetTile(posInt);
        //if (tile != null) Debug.Log(string.Format("Tile is: {0}", tile.name));
        fieldTileMap.SetTile(fieldTileMap.WorldToCell(posInt), minoTile[0]);

    }
}

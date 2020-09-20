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
    public Camera c;
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
                    Vector3Int pos = fieldTileMap.WorldToCell(new Vector3Int(j, i, 0));
                    if (gameScript.field[j, i] == 0)
                    {
                        fieldTileMap.SetTile(pos, black);
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
                    }

                }
            }
        }
    }
    void Update()
    {

        var mousePos = Input.mousePosition;
        Vector3 pos =c.ScreenToWorldPoint(mousePos - c.transform.position);
        pos.y += 0.35f;
        Vector3Int posInt = grid.WorldToCell(new Vector3(pos.x, pos.y , 0));
        Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {1}]", posInt.x, posInt.y));
        TileBase tile = fieldTileMap.GetTile(posInt);
        if (tile != null) Debug.Log(string.Format("Tile is: {0}", tile.name));
        fieldTileMap.SetTile(fieldTileMap.WorldToCell(posInt), minoTile[0]);

    }
}

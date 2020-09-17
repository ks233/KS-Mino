using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldVisualization : MonoBehaviour
{
    public Game gameScript;
    public GameObject fieldColour;
    public Tile[] minoTile;
    public Tile black;
    public Tilemap fieldTileMap;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(fieldTileMap.WorldToLocal(new Vector3Int(0, 0, 0)));
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!gameScript.gameover)
        {
            fieldTileMap.ClearAllTiles();
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (gameScript.field[j, i] == 0)
                    {
                        fieldTileMap.SetTile(fieldTileMap.WorldToCell(new Vector3Int(j+2, i, 0)), black);
                    }
                    if (gameScript.field[j, i] > 0 && gameScript.field[j, i] < 20)
                    {
                        fieldTileMap.SetTile(fieldTileMap.WorldToCell(new Vector3Int(j+2, i, 0)), minoTile[gameScript.field[j, i] - 1]);
                    }
                    if (gameScript.field[j, i] < 0)
                    {
                        fieldTileMap.SetTile(fieldTileMap.WorldToCell(new Vector3Int(j+2, i, 0)), minoTile[-gameScript.field[j, i] - 1]);
                    }
                    if (gameScript.field[j, i] > 20)
                    {
                        fieldTileMap.SetTile(fieldTileMap.WorldToCell(new Vector3Int(j+2, i, 0)), minoTile[gameScript.field[j, i] - 21]);
                    }

                }
            }
        }
    }
}

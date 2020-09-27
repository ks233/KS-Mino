using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawField : MonoBehaviour
{


    public Tilemap fieldTileMap;
    public Camera c;
    public Grid grid;
    public Tile[] minoTile;
    public FumenEditor gameScript;

    public int selecting = 8;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameScript.MODE == "EDIT")
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = c.transform.position.z;

            Vector3 pos = c.ScreenToWorldPoint(mousePos);
            Vector3Int posInt = new Vector3Int((int)(pos.x), (int)(pos.y), 0);
            //Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {1}]", posInt.x, posInt.y));
            TileBase tile = fieldTileMap.GetTile(posInt);
            //if (tile != null) Debug.Log(string.Format("Tile is: {0}", tile.name));
            if (posInt.x < 10 && posInt.y < 20 && 0 <= posInt.x && 0 <= posInt.y)
            {

                if (Input.GetMouseButton(0))
                {
                    gameScript.field.array[posInt.x, posInt.y] = -selecting;
                }
                else if (Input.GetMouseButton(1))
                {
                    gameScript.field.array[posInt.x, posInt.y] = 0;

                }

            }
        }
    }
}

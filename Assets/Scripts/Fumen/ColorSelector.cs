using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorSelector : MonoBehaviour
{
    public Tilemap colorSelector;
    public Camera c;
    public Grid grid;
    public FumenEditor gameScript;
    public DrawField df;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            //mousePos += c.transform.position;
            Vector3 pos = c.ScreenToWorldPoint(mousePos);
            Vector3Int posInt = colorSelector.WorldToCell(new Vector3(pos.x, pos.y, 0));
            //Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {1}]", posInt.x, posInt.y));
            TileBase tile = colorSelector.GetTile(posInt);
            //if (tile != null) Debug.Log(string.Format("Tile is: {0}", tile.name));

            if (tile != null)
            {
                Debug.Log(string.Format("Tile is: {0}", tile.name));
                switch (tile.name)
                {
                    case "mino_colors_0": df.selecting = 1; break;
                    case "mino_colors_1": df.selecting = 2; break;
                    case "mino_colors_2": df.selecting = 3; break;
                    case "mino_colors_3": df.selecting = 4; break;
                    case "mino_colors_4": df.selecting = 6; break;
                    case "mino_colors_5": df.selecting = 5; break;
                    case "mino_colors_6": df.selecting = 7; break;
                    case "garbage_tile": df.selecting = 8; break;

                }
            }
        }
    }
}

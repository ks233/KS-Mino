using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GuideDisplayer : MonoBehaviour
{
    public Toggle showGuide;
    public Tilemap guideTilemap;
    public FumenPlayer gameScript;
    public TileBase[] minoTile;
    // Update is called once per frame
    void Update()
    {
        guideTilemap.ClearAllTiles();
        int count = gameScript.count;
        int len = gameScript.fumen.guideList.Count;
        if (showGuide.isOn && 0 < len && count < len)
        {
            Mino mino = gameScript.fumen.guideList[gameScript.count];
            Debug.Log(gameScript.count);
            mino.SetArray();


            List<Vector2Int> list = gameScript.field.GetAllCoordinates(mino);
            foreach(Vector2Int l in list)
            {
                Vector3Int pos = new Vector3Int(l.x, l.y, 0);
                guideTilemap.SetTile(pos, minoTile[mino.id - 1]);
            }
        }
        else
        {

        }
    }
}

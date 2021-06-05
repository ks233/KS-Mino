using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldUIDisplay : MonoBehaviour
{
    public GameObject[] MinoImgs;//0�ն���1~7��ɫ���飬8��ɫ������




    public void UpdateField(Field field, Mino activeMino)
    {
        int cellSize = (int)gameObject.GetComponent<RectTransform>().rect.width / 10;

        foreach (Transform child in this.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int[,] display = field.array.Clone() as int[,];
        int minoId = activeMino.GetIdInt();
        int ghostHeight = field.LandHeight(activeMino); 

        foreach (Vector2Int v in Field.GetAllCoordinates(activeMino))
        {
            display[v.x, v.y] = minoId;
            display[v.x, v.y - activeMino.position.y + ghostHeight] = -minoId;
            
        }
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                if (display[x, y] > 0) AddImage(display[x, y],x,y,cellSize);

            }
        }
    }
    public GameObject AddImage(int minoId,int x, int y,int cellSize) //ʵ���������prefab�����飩
    {
        

        GameObject t = Instantiate(MinoImgs[minoId], Vector3.zero, Quaternion.identity, gameObject.transform);
        t.transform.SetParent(gameObject.transform, false);
        t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(x,y)*cellSize;

        t.GetComponent<RectTransform>().sizeDelta = new Vector2Int(cellSize, cellSize);
        return t;
    }
}

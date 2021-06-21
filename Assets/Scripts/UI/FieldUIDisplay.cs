using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldUIDisplay : MonoBehaviour
{
    public GameObject[] MinoImgs;//0空对象，1~7彩色方块，8灰色垃圾块




    public void UpdateField(Field field, Mino activeMino)
    {
        int cellSize = (int)gameObject.GetComponent<RectTransform>().rect.width / 10;
        foreach (Transform child in this.gameObject.transform)//清空所有子物体
        {
            GameObject.Destroy(child.gameObject);
        }
        int[,] display = field.array.Clone() as int[,];
        for (int x = 0; x < 10; x++)//将10*20的场地显示出来
        {
            for (int y = 0; y < 20; y++)
            {
                if (display[x, y] > 0) AddImage(display[x, y],x,y,cellSize);
            }
        }
    }

    public GameObject AddImage(int minoId,int x, int y,int cellSize) //实例化方块的prefab（方块）
    {
        

        GameObject t = Instantiate(MinoImgs[minoId], Vector3.zero, Quaternion.identity, gameObject.transform);
        t.transform.SetParent(gameObject.transform, false);
        t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(x,y)*cellSize;

        t.GetComponent<RectTransform>().sizeDelta = new Vector2Int(cellSize, cellSize);
        return t;
    }
}

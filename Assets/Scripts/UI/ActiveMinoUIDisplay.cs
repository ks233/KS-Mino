using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveMinoUIDisplay : MonoBehaviour
{
    public GameObject[] MinoImgs;
    public void UpdateActiveMino(Field field,Mino activeMino)
    {
        foreach (Transform child in this.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int cellSize = (int)gameObject.GetComponent<RectTransform>().rect.width / 10;
        int minoId = activeMino.GetIdInt();
        int ghostHeight = field.LandHeight(activeMino);
        foreach (Vector2Int v in Field.GetAllCoordinates(activeMino))
        {
            AddImage(minoId, v.x, v.y, cellSize);
            AddGhostImage(minoId, v.x, v.y - activeMino.position.y + ghostHeight, cellSize);

        }
    }

    public GameObject AddImage(int minoId, int x, int y, int cellSize) //实例化方块的prefab（方块）
    {


        GameObject t = Instantiate(MinoImgs[minoId-1], Vector3.zero, Quaternion.identity, gameObject.transform);
        t.transform.SetParent(gameObject.transform, false);
        t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(x, y) * cellSize;
        t.GetComponent<RectTransform>().sizeDelta = new Vector2Int(cellSize, cellSize);
        return t;
    }
    public GameObject AddGhostImage(int minoId, int x, int y, int cellSize) //砖块阴影
    {
        GameObject t = Instantiate(MinoImgs[minoId-1], Vector3.zero, Quaternion.identity, gameObject.transform);
        t.transform.SetParent(gameObject.transform, false);
        t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(x, y) * cellSize;
        t.GetComponent<RectTransform>().sizeDelta = new Vector2Int(cellSize, cellSize);
        Color tmpColor = t.GetComponent<Image>().color;
        tmpColor.a = 0.5f;
        t.GetComponent<Image>().color = tmpColor;
        return t;
    }

}

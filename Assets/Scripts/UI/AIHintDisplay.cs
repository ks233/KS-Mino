using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIHintDisplay : MonoBehaviour
{
    public GameObject[] MinoImgs;
    public void UpdateMino(Mino activeMino)
    {
        foreach (Transform child in this.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int cellSize = (int)gameObject.GetComponent<RectTransform>().rect.width / 10;
        int minoId = activeMino.GetIdInt();
        foreach (Vector2Int v in Field.GetAllCoordinates(activeMino))
        {
            AddImage(minoId, v.x, v.y, cellSize);
        }
    }

    public void Clear()
    {
        foreach (Transform child in this.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public GameObject AddImage(int minoId, int x, int y, int cellSize) //×©¿éÒõÓ°
    {
        GameObject t = Instantiate(MinoImgs[8], Vector3.zero, Quaternion.identity, gameObject.transform);
        t.transform.SetParent(gameObject.transform, false);
        t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(x, y) * cellSize;
        t.GetComponent<RectTransform>().sizeDelta = new Vector2Int(cellSize, cellSize);
        Color tmpColor = t.GetComponent<Image>().color;
        tmpColor.a = 0.8f;
        t.GetComponent<Image>().color = tmpColor;
        return t;
    }
}

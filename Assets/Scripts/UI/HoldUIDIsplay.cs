using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldUIDisplay : MonoBehaviour
{

    public GameObject[] mino;

    public void UpdateHold(int minoId)
    {

        int cellSize = 60;
        foreach (Transform child in this.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        if (minoId == 0) return;
        GameObject t = Instantiate(mino[minoId - 1], Vector3.zero, Quaternion.identity, gameObject.transform);
        t.transform.SetParent(gameObject.transform, false);
        if (minoId <= 5)
        {
            t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, 0 - cellSize / 4);
        }
        else
        {
            t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, 0);
        }
        t.GetComponent<RectTransform>().localScale = new Vector3(0.8f,0.8f,1);
        //t.GetComponent<RectTransform>().sizeDelta = new Vector2Int(cellSize, cellSize);
    }
}

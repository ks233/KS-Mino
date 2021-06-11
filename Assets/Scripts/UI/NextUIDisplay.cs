using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextUIDisplay : MonoBehaviour
{
    public GameObject[] mino;

    public void UpdateNext(int[] nextSeq,int maxNext)
    {
        int cellSize = 60;
        foreach (Transform child in this.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        int count = 1;
        foreach (int minoId in nextSeq)
        {
            if (count > maxNext )break;
            GameObject t = Instantiate(mino[minoId - 1], Vector3.zero, Quaternion.identity, gameObject.transform);
            t.transform.SetParent(gameObject.transform, false);
            t.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
            count++;
        }
        //t.GetComponent<RectTransform>().sizeDelta = new Vector2Int(cellSize, cellSize);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{

    public GameObject lineH,lineV;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 45; i < 440; i+=44)
        {
            GameObject t = Instantiate(lineV, Vector3.zero, Quaternion.identity, gameObject.transform);
            t.transform.SetParent(gameObject.transform, false);
            t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(i, 0);
        }

        for (int i = 45; i < 880; i += 44)
        {
            GameObject t = Instantiate(lineH, Vector3.zero, Quaternion.identity, gameObject.transform);
            t.transform.SetParent(gameObject.transform, false);
            t.GetComponent<RectTransform>().anchoredPosition = new Vector2Int(0, i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

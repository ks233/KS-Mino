using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUtils : MonoBehaviour
{
    public static GameObject InstChild(GameObject child, Vector3 position, GameObject parent) //实例化方块的prefab（方块）
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        return t;
    }

    public static GameObject InstChild(GameObject child, Vector3 position, GameObject parent, float scale)//实例化prefab，带缩放（hold/next）
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale, scale, scale);
        return t;
    }

    public static GameObject InstChild(GameObject child, Vector3 position, GameObject parent, float scale, float alpha)//实例化prefab，带缩放和透明度（砖块阴影）
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale, scale, scale);
        Color tmp = t.GetComponent<SpriteRenderer>().color;
        tmp.a = alpha;
        t.GetComponent<SpriteRenderer>().color = tmp;
        return t;
    }
}

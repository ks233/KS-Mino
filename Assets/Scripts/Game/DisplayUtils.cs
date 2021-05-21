using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUtils : MonoBehaviour
{
    public static GameObject InstChild(GameObject child, Vector3 position, GameObject parent) //ʵ���������prefab�����飩
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        return t;
    }

    public static GameObject InstChild(GameObject child, Vector3 position, GameObject parent, float scale)//ʵ����prefab�������ţ�hold/next��
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale, scale, scale);
        return t;
    }

    public static GameObject InstChild(GameObject child, Vector3 position, GameObject parent, float scale, float alpha)//ʵ����prefab�������ź�͸���ȣ�ש����Ӱ��
    {
        GameObject t = Instantiate(child, position + parent.transform.position, Quaternion.identity, parent.transform);
        t.transform.localScale = new Vector3(scale, scale, scale);
        Color tmp = t.GetComponent<SpriteRenderer>().color;
        tmp.a = alpha;
        t.GetComponent<SpriteRenderer>().color = tmp;
        return t;
    }
}

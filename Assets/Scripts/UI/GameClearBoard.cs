using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClearBoard : MonoBehaviour
{
    public Text txtGameClear;
    public void SetText(string text)//��ʲô����ʾʲô������������������TSD����PC��
    {
        txtGameClear.text = string.Format("<color=YELLOW><size=60>{0}</size></color>\nGAME CLEAR",text);
    }
    public void SetGameOverText()//��ʲô����ʾʲô������������������TSD����PC��
    {
        txtGameClear.text = "GAME OVER";
    }

    public void Hide()
    {

    }

    void Start()
    {
    }
}

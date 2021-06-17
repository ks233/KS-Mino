using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClearBoard : MonoBehaviour
{
    public Text txtGameClear;
    public void SetText(string text)//给什么就显示什么，比如秒数、分数、TSD数、PC数
    {
        txtGameClear.text = string.Format("<color=YELLOW><size=60>{0}</size></color>\nGAME CLEAR",text);
    }
    public void SetGameOverText()//给什么就显示什么，比如秒数、分数、TSD数、PC数
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

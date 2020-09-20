using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuStart : MonoBehaviour
{
    public void ChangeMenuScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}

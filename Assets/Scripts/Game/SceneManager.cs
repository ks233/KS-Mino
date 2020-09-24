using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}

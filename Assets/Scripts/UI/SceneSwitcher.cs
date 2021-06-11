using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneSwitcher : MonoBehaviour
{
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SwitchScene);
    }
    public void SwitchScene()
    {
        SceneManager sceneManager = new SceneManager();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

}

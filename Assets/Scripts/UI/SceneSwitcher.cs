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
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
        PauseMenu.GameIsPaused = false;
    }

}

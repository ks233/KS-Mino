using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private bool started;//false��ʾanykey֮ǰ��true��ʾ֮��
    public Animator startAnim;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        started = false;

        startAnim.SetBool("started", started);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (started == false)
            {
                started = true;
                //animation in...
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && started)
            {
                started = false;
                //animation out...
            }
            startAnim.SetBool("started", started);
        }

    }
}

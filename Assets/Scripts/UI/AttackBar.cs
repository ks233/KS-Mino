using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour
{
    public bool isSinglePlayer = true;
    private float attack;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void AddAttack(int atk)
    {
        attack += atk;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSinglePlayer)
        {
            if (attack > 0)
            {
                attack -= Time.deltaTime * 10;
                if (attack < 0) attack = 0;
            }
        }
        if (attack > 0) gameObject.GetComponent<Image>().fillAmount = attack / 20;
    }
}

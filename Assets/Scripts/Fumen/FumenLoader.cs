using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FumenLoader : MonoBehaviour
{
    public void LoadFumen()
    {
        StreamReader reader = new StreamReader("save1.ksfumen");
        string json = reader.ReadLine();
        Fumen f = JsonUtility.FromJson<Fumen>(json);

        
    }





}

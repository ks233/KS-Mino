using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class FumenFileBrowser : MonoBehaviour
{

    public TMP_Dropdown foldersDropdown;
    public TMP_Dropdown fumensDropdown;
    private List<string> dirs = new List<string>();
    private List<string> folders = new List<string>();

    private void Awake()
    {
        UpdateFolders();
        UpdateFilePaths();
    }

    public void UpdateFolders()
    {
        foldersDropdown.ClearOptions();
        folders.Clear();

        foreach (string path in Directory.GetDirectories(Directory.GetCurrentDirectory()+"\\Fumen"))
        {
            folders.Add(path.Substring(path.LastIndexOf("\\")+1));
        }

        foldersDropdown.AddOptions(folders);
    }


    public void UpdateFilePaths()
    {
        if (foldersDropdown.captionText.text != "")
        {

            fumensDropdown.ClearOptions();
            dirs.Clear();
            foreach (string path in Directory.GetFiles(Directory.GetCurrentDirectory()+"\\Fumen\\" + foldersDropdown.captionText.text))
            {
                if (System.IO.Path.GetExtension(path) == ".ksfumen")
                {
                    dirs.Add(path.Substring(path.LastIndexOf("\\") + 1));
                }
            }
            fumensDropdown.AddOptions(dirs);
        }
    }
}

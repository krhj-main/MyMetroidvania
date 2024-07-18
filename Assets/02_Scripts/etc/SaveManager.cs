using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SaveManager
{
    public static SaveManager Instance;


    public HashSet<string> sceneNames;

    public void Init()
    {
        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }
}

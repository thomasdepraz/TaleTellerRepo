using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CoreManager : Singleton<CoreManager>
{
    public SaveFile saveFile;

    //Core date
    [HideInInspector] public bool completeTutorial;
    [HideInInspector] public bool playTutorial;

    public void Awake()
    {
        CreateSingleton(true);
    }

    public void Start()
    {
        saveFile = SaveManager.Load();

        completeTutorial = saveFile.completeTutorial;
        //... Load other data if necessary
    }

}

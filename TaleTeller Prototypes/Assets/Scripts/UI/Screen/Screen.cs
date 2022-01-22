using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameScreen
{
    public bool open;
    public abstract void Open(Action onComplete);


    public abstract void Close(Action onComplete);


    public abstract void InitializeContent(Action onComplete);
}

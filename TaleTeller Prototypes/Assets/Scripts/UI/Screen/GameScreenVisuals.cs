using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameScreenVisuals
{
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public Image backgroundPanel;
    public RectTransform contentTransform;
    public ScreenButton confirmButton;

    /// <summary>
    /// Initialize the visuals using the gamescreen data
    /// </summary>
    public abstract void Initialize(GameScreen gameScreen);
    public abstract void Open(Action onComplete);
    public abstract void Close(Action onComplete);
    public abstract void Reset();


}

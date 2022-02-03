using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : GameScreen
{
    public string instructionText;
    public string tutorialText;
    public Sprite illustration;
    TutorialScreenVisuals visuals;

    public TutorialScreen(string tutorialText,string instructionText, Sprite illustration)
    {
        ScreenManager.Instance.currentScreen = this;
        this.tutorialText = tutorialText;
        this.instructionText = instructionText;
        this.illustration = illustration;
        visuals = ScreenManager.Instance.tutorialScreenVisuals;

        visuals.Initialize(this);
    }

    public override void Open(Action onComplete)
    {
        open = true;
        visuals.Open(onComplete);
    }
    public override void Close(Action onComplete)
    {
        visuals.Close(onComplete);
    }

    public void Confirm()
    {
        open = false;
    }

}

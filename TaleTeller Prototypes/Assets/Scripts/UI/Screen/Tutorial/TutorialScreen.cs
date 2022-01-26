using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : GameScreen
{
    public string instructionText;
    public string tutorialText;
    TutorialScreenVisuals visuals;

    public TutorialScreen(string tutorialText,string instructionText)
    {
        ScreenManager.Instance.currentScreen = this;
        this.tutorialText = tutorialText;
        this.instructionText = instructionText;
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

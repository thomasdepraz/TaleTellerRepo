using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : GameScreen
{
    public string title;
    public string content;
    public CardData card;
    public GameOverScreenVisuals visuals;

    public GameOverScreen(string title, string content, CardData card)
    {
        ScreenManager.Instance.currentScreen = this;
        this.title = title;
        this.content = content;
        visuals = ScreenManager.Instance.gameOverScreenVisuals;
        visuals.Initialize(this);
    }

    public void ReturnToMenu()
    {
        open = false;
        SceneManager.LoadScene(0);
    }


    public override void Close(Action onComplete)
    {
        throw new NotImplementedException();
    }

    public override void Open(Action onComplete)
    {
        open = true;
        visuals.Open(onComplete);
    }
}

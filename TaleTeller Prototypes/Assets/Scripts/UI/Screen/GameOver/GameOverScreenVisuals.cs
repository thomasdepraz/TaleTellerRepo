using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class GameOverScreenVisuals : GameScreenVisuals
{
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI contentText;
    public PlaceholderCard card;

    public override void Initialize(GameScreen gameScreen)
    {
        GameOverScreen screen = gameScreen as GameOverScreen;

        instructionText.text = screen.title;
        contentText.text = screen.content;
        card.container.InitializeContainer(screen.card, true);

        confirmButton.onClick = screen.ReturnToMenu;
        confirmButton.interactable = true;
        
    }

    public override void Open(Action onComplete)
    {
        Color panelColor = backgroundPanel.color;
        backgroundPanel.color = Color.clear;

        contentTransform.localPosition = new Vector3(-canvasScaler.referenceResolution.x, 0, 0);
        canvas.gameObject.SetActive(true);
        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero + Vector3.right * 10, 1f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();
        });

        LeanTween.color(backgroundPanel.gameObject, panelColor, 0.5f).setDelay(1f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(onComplete);
    }

    public override void Close(Action onComplete)
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}

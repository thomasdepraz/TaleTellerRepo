using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TutorialScreenVisuals : GameScreenVisuals
{
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI tutorialText;

    public Image illustration;

    public override void Initialize(GameScreen gameScreen)
    {
        TutorialScreen screen = gameScreen as TutorialScreen;
        instructionsText.text = screen.instructionText;
        tutorialText.text = screen.tutorialText;


        confirmButton.onClick = screen.Confirm;
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
        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero - Vector3.right * 10, 0.3f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(contentTransform.gameObject, new Vector3(canvasScaler.referenceResolution.x, 0, 0), 1f).setEaseInQuint();
        });

        Color panelColor = backgroundPanel.color;
        LeanTween.value(backgroundPanel.gameObject, panelColor, Color.clear, 0.5f).setDelay(1f).setEaseInQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(() =>
        {
            canvas.gameObject.SetActive(false);
            backgroundPanel.color = panelColor;
            onComplete?.Invoke();

        });

        backgroundPanel.color = panelColor;
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}

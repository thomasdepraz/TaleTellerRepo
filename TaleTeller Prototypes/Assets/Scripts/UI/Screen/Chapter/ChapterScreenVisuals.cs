using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChapterScreenVisuals 
{
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public Image panel;
    public RectTransform bookTransform;
    public RectTransform instructionTransform;
    public RectTransform titleTransform_A;
    public RectTransform titleTransform_B;
    public RectTransform chapterTransform_A;
    public RectTransform chapterTransform_B;
    public RectTransform rewardTransform_A;
    public RectTransform rewardTransform_B;
    [Space]
    public ScreenButton confirmButton;
    public ScreenButton illustrationButton_A;
    public ScreenButton illustrationButton_B;
    public CardContainer card_A;
    public CardContainer card_B;
    public PlaceholderCard placeholder_A;
    public PlaceholderCard placeholder_B;
    [Space]
    public Image illustrationImage_A;
    public Image illustrationImage_B;
    public Image rewardIcon_A;
    public Image rewardIcon_B;
    [Space]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI titleText_A;
    public TextMeshProUGUI titleText_B;
    public TextMeshProUGUI chapterText_A;
    public TextMeshProUGUI chapterText_B;
    public TextMeshProUGUI rewardButtonsText_A;
    public TextMeshProUGUI rewardButtonsText_B;
    public TextMeshProUGUI confirmButtonText;

    public void SetMode(ChapterScreenMode mode, int choice)
    {
        titleTransform_A.gameObject.SetActive(true);
        titleTransform_B.gameObject.SetActive(true);
        chapterTransform_A.gameObject.SetActive(true);
        chapterTransform_B.gameObject.SetActive(true);
        rewardTransform_A.gameObject.SetActive(true);
        rewardTransform_B.gameObject.SetActive(true);

        switch (mode)
        {
            case ChapterScreenMode.CARD:
                placeholder_A.selected = false;
                placeholder_B.selected = false;
                illustrationButton_A.gameObject.SetActive(false);
                illustrationButton_B.gameObject.SetActive(false);
                card_A.gameObject.SetActive(true);
                card_B.gameObject.SetActive(true);
                rewardTransform_A.gameObject.SetActive(true);
                rewardTransform_B.gameObject.SetActive(true);
                if (choice == 1)
                {
                    titleTransform_B.gameObject.SetActive(false);
                    chapterTransform_B.gameObject.SetActive(false);
                    rewardTransform_B.gameObject.SetActive(false);
                    card_B.gameObject.SetActive(false);
                }
                break;
            case ChapterScreenMode.PLOT:
                illustrationButton_A.selected = false;
                illustrationButton_B.selected = false;
                illustrationButton_A.gameObject.SetActive(true);
                illustrationButton_B.gameObject.SetActive(true);
                card_A.gameObject.SetActive(false);
                card_B.gameObject.SetActive(false);
                rewardTransform_A.gameObject.SetActive(false);
                rewardTransform_B.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void OpenTween(Action onComplete)
    {
        bookTransform.localPosition = new Vector3(0, -canvasScaler.referenceResolution.y, 0);
        canvas.gameObject.SetActive(true);
        Color panelColor = panel.color;
        panel.color = Color.clear;

        LeanTween.moveLocal(bookTransform.gameObject, Vector3.zero + Vector3.up *10, 0.5f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(bookTransform.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();

        });

        LeanTween.color(panel.gameObject, panelColor, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            panel.color = col;
        }).setOnComplete(onComplete);
    }

    public void CloseTween(Action onComplete)
    {
        LeanTween.moveLocal(bookTransform.gameObject, Vector3.zero + Vector3.down * 10, 0.3f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(bookTransform.gameObject, new Vector3(0, canvasScaler.referenceResolution.y, 0), 0.5f).setEaseInQuint();
        });


        Color panelColor = panel.color;
        LeanTween.value(panel.gameObject, panelColor, Color.clear, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            panel.color = col;
        }).setOnComplete(() =>
        {
            canvas.gameObject.SetActive(false);
            panel.color = panelColor;
            bookTransform.localPosition = Vector3.zero; 
            onComplete?.Invoke();

        });
    }

}

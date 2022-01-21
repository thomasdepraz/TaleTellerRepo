using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChapterScreenVisuals 
{
    public Canvas canvas;
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

    public void SetMode(ChapterScreenMode mode)
    {
        switch (mode)
        {
            case ChapterScreenMode.CARD:
                illustrationButton_A.gameObject.SetActive(false);
                illustrationButton_B.gameObject.SetActive(false);
                card_A.gameObject.SetActive(true);
                card_B.gameObject.SetActive(true);
                break;
            case ChapterScreenMode.PLOT:
                illustrationButton_A.gameObject.SetActive(true);
                illustrationButton_B.gameObject.SetActive(true);
                card_A.gameObject.SetActive(false);
                card_B.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

}

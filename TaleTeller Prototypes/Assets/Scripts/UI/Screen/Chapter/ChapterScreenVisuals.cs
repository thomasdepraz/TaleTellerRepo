using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChapterScreenVisuals : GameScreenVisuals
{
    public List<ScreenButton> illustrationButtons;
    public List<PlaceholderCard> cardsPlacholders;

    [Space]
    public List<Image> illustrations;
    public List<Image> rewardIcons;
    public List<GameObject> titles;
    public List<GameObject> chapters;
    public List<Image> rewards;
    public List<RewardTooltip> rewardsTooltip;

    [Space]
    public TextMeshProUGUI instructionText;
    public List<TextMeshProUGUI> titleTexts;
    public List<TextMeshProUGUI> chapterTexts;
    public List<TextMeshProUGUI> rewardTexts;
    public TextMeshProUGUI confirmButtonText;

    void SetMode(ChapterScreenMode mode, ChapterScreen screen)
    {
        switch (mode)
        {
            case ChapterScreenMode.CARD:
                for (int i = 0; i < screen.currentStep.stepOptions.Count; i++)
                {
                    titles[i].SetActive(true);
                    chapters[i].SetActive(true);
                    rewards[i].gameObject.SetActive(true);
                    cardsPlacholders[i].gameObject.SetActive(true);
                }
                break;
            case ChapterScreenMode.PLOT:
                for (int i = 0; i < screen.schemesToChooseFrom.Count; i++)
                {
                    titles[i].SetActive(true);
                    chapters[i].SetActive(true);
                    illustrationButtons[i].gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
    }
    void LoadData(ChapterScreenMode mode, ChapterScreen screen)
    {
        switch (mode)
        {
            case ChapterScreenMode.PLOT:
                instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseSchemeInstruction);

                for (int i = 0; i < screen.schemesToChooseFrom.Count; i++)
                {
                    int j = i;
                    illustrationButtons[i].buttonImage.sprite = screen.schemesToChooseFrom[i].plotIllustration;
                    titleTexts[i].text = screen.schemesToChooseFrom[i].plotName;
                    chapterTexts[i].text = screen.schemesToChooseFrom[i].plotDescription;

                    illustrationButtons[j].onClick = () => screen.ClickIllustration(illustrationButtons[j], screen.schemesToChooseFrom[j]);
                }
                break;

            case ChapterScreenMode.CARD:
                instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseSchemeStepInstruction);

                for (int i = 0; i < screen.currentStep.stepOptions.Count; i++)
                {
                    int j = i;
                    titleTexts[j].text = screen.currentStep.stepOptions[j].cardName;
                    chapterTexts[i].text = (screen.currentStep.stepOptions[j] as PlotCard).plotChoiceDescription;
                    rewardIcons[i].sprite = screen.GetRewardIcon(screen.rewardInfos[i].type);
                    rewards[i].color = screen.GetRewardsColor(screen.rewardInfos[i].rarity);
                    rewardsTooltip[i].rewardInfo = screen.rewardInfos[i];
                    cardsPlacholders[j].onClick = () => screen.ClickCard(screen.rewardInfos[j], cardsPlacholders[j]);
                    cardsPlacholders[j].container.InitializeContainer(screen.currentStep.stepOptions[j], true);
                }
                break;
            default:
                break;
        }
    }

    public override void Initialize(GameScreen gameScreen)
    {
        Reset();

        ChapterScreen screen = gameScreen as ChapterScreen;
        SetMode(screen.screenMode, screen);
        LoadData(screen.screenMode, screen);

        confirmButtonText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CONFIRM");
        confirmButton.onClick = screen.Confirm;
        confirmButton.interactable = false;
    }

    public override void Open(Action onComplete)
    {
        contentTransform.localPosition = new Vector3(0, -canvasScaler.referenceResolution.y, 0);
        canvas.gameObject.SetActive(true);
        Color panelColor = backgroundPanel.color;
        backgroundPanel.color = Color.clear;

        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero + Vector3.up * 10, 0.5f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();

        });

        LeanTween.color(backgroundPanel.gameObject, panelColor, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(onComplete);
    }

    public override void Close(Action onComplete)
    {
        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero + Vector3.down * 10, 0.3f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(contentTransform.gameObject, new Vector3(0, canvasScaler.referenceResolution.y, 0), 0.5f).setEaseInQuint();
        });


        Color panelColor = backgroundPanel.color;
        LeanTween.value(backgroundPanel.gameObject, panelColor, Color.clear, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(() =>
        {
            canvas.gameObject.SetActive(false);
            backgroundPanel.color = panelColor;
            contentTransform.localPosition = Vector3.zero;
            onComplete?.Invoke();

        });
    }

    public override void Reset()
    {
        for (int i = 0; i < illustrationButtons.Count; i++)
        {
            illustrationButtons[i].gameObject.SetActive(false);
            illustrationButtons[i].selected = false;
        }
        for (int i = 0; i < cardsPlacholders.Count; i++)
        {
            cardsPlacholders[i].gameObject.SetActive(false);
            cardsPlacholders[i].selected = false;
        }

        for (int i = 0; i < titles.Count; i++)
        {
            titles[i].SetActive(false);
        }
        for (int i = 0; i < chapters.Count; i++)
        {
            chapters[i].SetActive(false);
        }
        for (int i = 0; i < rewards.Count; i++)
        {
            rewards[i].gameObject.SetActive(false);
        }
    }
}

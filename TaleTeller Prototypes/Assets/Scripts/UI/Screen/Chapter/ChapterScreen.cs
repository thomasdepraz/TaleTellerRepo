using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChapterScreenMode
{
    CARD,
    PLOT
}
public class ChapterScreen : GameScreen
{

    public ChapterScreenVisuals visuals;
    public ChapterScreenMode screenMode;
    public MainPlotScheme chosenScheme;
    public CardContainer chosenCard;
    public RewardInfo selectedRewardInfo;

    Action illustrationClick_A;
    Action illustrationClick_B;
    Action placeHolderClick_A;
    Action placeHolderClick_B;
    Action confirmClick;
    RewardInfo rewardInfo_A;
    RewardInfo rewardInfo_B;
    ScreenButton selectedIllustration;
    PlaceholderCard selectedPlaceholder;

    public ChapterScreen(List<MainPlotScheme>schemesToChooseFrom)
    {
        screenMode = ChapterScreenMode.PLOT;
        open = true;

        visuals = ScreenManager.Instance.chapterScreenVisuals;
        visuals.SetMode(ChapterScreenMode.PLOT, 0);

        //Load data on visuals
        visuals.instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseSchemeInstruction);

        visuals.illustrationButton_A.buttonImage.sprite = schemesToChooseFrom[0].plotIllustration;
        visuals.illustrationButton_B.buttonImage.sprite = schemesToChooseFrom[1].plotIllustration;

        visuals.titleText_A.text = schemesToChooseFrom[0].plotName;
        visuals.titleText_B.text = schemesToChooseFrom[1].plotName;

        visuals.chapterText_A.text = schemesToChooseFrom[0].plotDescription;
        visuals.chapterText_B.text = schemesToChooseFrom[1].plotDescription;

        illustrationClick_A = () => ClickIllustration(visuals.illustrationButton_A, schemesToChooseFrom[0]);
        illustrationClick_B = () => ClickIllustration(visuals.illustrationButton_B, schemesToChooseFrom[1]);

        visuals.illustrationButton_A.onClick = illustrationClick_A;
        visuals.illustrationButton_B.onClick = illustrationClick_B;

        confirmClick = () => Confirm();
        visuals.confirmButton.onClick = confirmClick;
        visuals.confirmButton.interactable = false;
    }

    public ChapterScreen(SchemeStep currentStep)
    {
        screenMode = ChapterScreenMode.CARD;
        open = true;

        visuals = ScreenManager.Instance.chapterScreenVisuals;
        visuals.SetMode(ChapterScreenMode.CARD, currentStep.stepOptions.Count);

        //Load data on visuals
        visuals.instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseSchemeStepInstruction);

        visuals.titleText_A.text = currentStep.stepOptions[0].cardName;
        visuals.chapterText_A.text = (currentStep.stepOptions[0] as PlotCard).plotChoiceDescription;
        visuals.card_A.InitializeContainer(currentStep.stepOptions[0], true);
        rewardInfo_A = new RewardInfo(StoryManager.Instance.actCount);
        visuals.rewardIcon_A.sprite = GetRewardIcon(rewardInfo_A.type);
        placeHolderClick_A = () => ClickCard(rewardInfo_A, visuals.placeholder_A, visuals.card_A);
        visuals.placeholder_A.onClick = placeHolderClick_A;
        
        if(currentStep.stepOptions.Count > 1)
        {
            visuals.titleText_B.text = currentStep.stepOptions[1].cardName;
            visuals.chapterText_B.text = (currentStep.stepOptions[1] as PlotCard).plotChoiceDescription;
            visuals.card_B.InitializeContainer(currentStep.stepOptions[1], true);
            rewardInfo_B = new RewardInfo(rewardInfo_A.rarity);
            visuals.rewardIcon_B.sprite = GetRewardIcon(rewardInfo_B.type);
            placeHolderClick_B = () => ClickCard(rewardInfo_B, visuals.placeholder_B, visuals.card_B);
            visuals.placeholder_B.onClick = placeHolderClick_B;
        }

        confirmClick = () => Confirm();
        visuals.confirmButton.onClick = confirmClick;
        visuals.confirmButton.interactable = false;
    }

    public override void Open(Action onComplete)
    {
        visuals.OpenTween(() => { onComplete?.Invoke(); });
    }

    public override void Close(Action onComplete)
    {
        visuals.CloseTween(() => { onComplete?.Invoke(); });
    }

    public override void InitializeContent(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public void ClickIllustration(ScreenButton button, MainPlotScheme scheme)
    {
        if(button == selectedIllustration)
        {
            selectedIllustration = null;
        }
        else
        {
            if (selectedIllustration != null) selectedIllustration.selected = false;
            selectedIllustration = button;
        }

        if (chosenScheme != scheme)
            chosenScheme = scheme;

        if (selectedIllustration == null)
            visuals.confirmButton.interactable = false;
        else 
            visuals.confirmButton.interactable = true;

    }

    public void ClickCard(RewardInfo rewardInfo, PlaceholderCard placeholder, CardContainer chosenCard)
    {
        if(placeholder == selectedPlaceholder)
        {
            selectedPlaceholder = null;
        }
        else
        {
            if (selectedPlaceholder != null) selectedPlaceholder.selected = false;
            selectedPlaceholder = placeholder;
        }

        if (selectedRewardInfo != rewardInfo)
        {
            selectedRewardInfo = rewardInfo;
            this.chosenCard = chosenCard;
        }


        if (selectedPlaceholder == null)
            visuals.confirmButton.interactable = false;
        else
            visuals.confirmButton.interactable = true;
    }

    public void Confirm()
    {
        open = false;
        if (screenMode == ChapterScreenMode.CARD)
            RewardManager.Instance.currentRewardInfo = selectedRewardInfo;
    }

    public Sprite GetRewardIcon(RewardType type)
    {
        switch (type)
        {
            case RewardType.MAX_STATS:
                return null;
            case RewardType.TEMP_STATS:
                return null;
            case RewardType.INSPIRE:
                return null;
            default:
                return null;
        }
    }

    ~ChapterScreen()
    {
        visuals.confirmButton.onClick -= confirmClick;
        switch (screenMode)
        {
            case ChapterScreenMode.CARD:
                visuals.card_A.ResetContainer(true);
                visuals.card_B.ResetContainer(true);
                visuals.placeholder_A.onClick -= placeHolderClick_A;
                visuals.placeholder_B.onClick -= placeHolderClick_B;
                break;
            case ChapterScreenMode.PLOT:
                visuals.illustrationButton_A.onClick -= illustrationClick_A;
                visuals.illustrationButton_A.onClick -= illustrationClick_A;
                break;
            default:
                break;
        }
    }
}

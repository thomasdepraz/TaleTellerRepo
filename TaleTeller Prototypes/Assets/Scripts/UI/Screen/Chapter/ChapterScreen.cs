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
    public List<MainPlotScheme> schemesToChooseFrom;
    public SchemeStep currentStep;
    public List<RewardInfo> rewardInfos;

    RewardInfo rewardInfo_A;
    RewardInfo rewardInfo_B;
    ScreenButton selectedIllustration;
    PlaceholderCard selectedPlaceholder;

    public ChapterScreen(List<MainPlotScheme>schemesToChooseFrom)
    {
        ScreenManager.Instance.currentScreen = this;
        screenMode = ChapterScreenMode.PLOT;
        visuals = ScreenManager.Instance.chapterScreenVisuals;
        this.schemesToChooseFrom = schemesToChooseFrom;

        visuals.Initialize(this);
    }

    public ChapterScreen(SchemeStep currentStep)
    {
        ScreenManager.Instance.currentScreen = this;
        screenMode = ChapterScreenMode.CARD;
        open = true;
        this.currentStep = currentStep;
        visuals = ScreenManager.Instance.chapterScreenVisuals;

        rewardInfos = GetRewardInfos();

        visuals.Initialize(this);
    }

    List<RewardInfo> GetRewardInfos()
    {
        List<RewardInfo> result = new List<RewardInfo>();

        RewardInfo baseInfo = new RewardInfo(StoryManager.Instance.actCount);
        result.Add(baseInfo);
        for (int i = 1; i < currentStep.stepOptions.Count; i++)
        {
            result.Add(new RewardInfo(baseInfo.rarity));
        }

        return result;
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

    public void ClickCard(RewardInfo rewardInfo, PlaceholderCard placeholder)
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
            this.chosenCard = placeholder.container;
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
                return RewardManager.Instance.rewardProfile.maxStatsIcon;
            case RewardType.TEMP_STATS:
                return RewardManager.Instance.rewardProfile.tempStatsIcon;
            case RewardType.INSPIRE:
                return RewardManager.Instance.rewardProfile.inspireIcon;
            default:
                return null;
        }
    }

    public Color GetRewardsColor(RewardRarity rarity)
    {
        switch (rarity)
        {
            case RewardRarity.NONE:
                return Color.white;
            case RewardRarity.COMMON:
                return RewardManager.Instance.rewardProfile.commonColor;
            case RewardRarity.RARE:
                return RewardManager.Instance.rewardProfile.rareColor;
            case RewardRarity.EPIC:
                return RewardManager.Instance.rewardProfile.epicColor;
            default:
                return Color.black;
        }
    }
}

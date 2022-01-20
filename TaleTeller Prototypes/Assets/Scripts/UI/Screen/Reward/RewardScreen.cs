using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : GameScreen
{
    public string questEndingText;
    public string accomplishementText;
    public string chooseInstruction;
    public string upgradeInstruction;
    public string confirmText;

    public Sprite accomplishementIllustration;

    public List<Reward> chosenRewards;
    public List<Reward> heroRewards;

    public AddCardReward addCardReward;
    public RemoveCardReward removeCardReward;

    public RewardScreenVisuals visuals;

    public RewardScreen(RewardInfo currentRewardInfo, MainPlotScheme currentScheme, RewardScreenVisuals visuals)
    {
        questEndingText = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$QUEST_ENDING");
        chooseInstruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$CHOOSE_INSTRUCTION");
        upgradeInstruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$UPGRADE_INSTRUCTION");
        confirmText = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$CONFIRM");

        accomplishementText = currentScheme.schemeSteps[currentScheme.currentStep].chapterDescription;
        accomplishementIllustration = null;//TODO

        addCardReward = new AddCardReward(2);
        removeCardReward = new RemoveCardReward(3);//TODO EXPOSE MAGIC NUMBERS

        chosenRewards = new List<Reward>();

        heroRewards = GenerateRewards(currentRewardInfo.type);
    }
    public List<Reward> GenerateRewards(RewardType type)
    {
        List<Reward> result = new List<Reward>();
        switch (type)
        {
            case RewardType.MAX_STATS:
                MaxHeroStats rewardTypeA = MaxStatsReward.GetStatType();
                MaxHeroStats rewardTypeB = MaxStatsReward.GetStatType(rewardTypeA);

                result.Add(new MaxStatsReward(rewardTypeA));
                result.Add(new MaxStatsReward(rewardTypeB));
                break;

            case RewardType.TEMP_STATS:
                TempHeroStats rewardTypeC = TempStatsReward.GetStatType();
                TempHeroStats rewardTypeD = TempStatsReward.GetStatType(rewardTypeC);

                result.Add(new TempStatsReward(rewardTypeC));
                result.Add(new TempStatsReward(rewardTypeD));
                break;

            case RewardType.INSPIRE:
                result.Add(new InspireTokenReward(1));
                break;
            default:
                break;
        }
        return result;
    }

    public override void Open(Action onComplete)
    {
        

    }
    public override void Close(Action onComplete)
    {

    }

    public override void InitializeContent(Action onComplete)
    {

    }
}

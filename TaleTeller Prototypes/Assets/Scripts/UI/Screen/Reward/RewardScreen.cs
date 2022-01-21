using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : GameScreen
{
    public List<Reward> chosenRewards;
    public List<Reward> heroRewards;

    public AddCardReward addCardReward;
    public RemoveCardReward removeCardReward;

    public RewardScreenVisuals visuals;

    public RewardScreen(RewardInfo currentRewardInfo, MainPlotScheme currentScheme)
    {
        visuals = ScreenManager.Instance.rewardScreenVisuals; ;
        visuals.completeText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$QUEST_ENDING");
        visuals.chooseInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$CHOOSE_INSTRUCTION"); 
        visuals.upgradeInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$UPGRADE_INSTRUCTION");
        //visuals.confirmButton. = LocalizationManager.Instance.GetString(LocalizationManager.Instance.rewardDictionary, "$CONFIRM");

        visuals.questText.text = currentScheme.schemeSteps[currentScheme.currentStep].chapterDescription;
        visuals.illustration = null;//TODO

        addCardReward = new AddCardReward(2);
        removeCardReward = new RemoveCardReward(3);//TODO EXPOSE MAGIC NUMBERS

        chosenRewards = new List<Reward>();

        heroRewards = GenerateRewards(currentRewardInfo.type);
        for (int i = 0; i < heroRewards.Count; i++)
        {
            visuals.heroRewardsButton[i].gameObject.SetActive(true);
            visuals.heroRewardsButton[i].onClick += () => SelectReward(heroRewards[i], visuals.heroRewardsButton[i]);
        }

        visuals.addButton.onClick += ()=> SelectReward(addCardReward, visuals.addButton);
        visuals.removeButton.onClick += () => SelectReward(removeCardReward, visuals.removeButton);

        visuals.confirmButton.onClick += Confirm;
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
        visuals.canvas.gameObject.SetActive(true);
        onComplete?.Invoke();
    }
    public override void Close(Action onComplete)
    {
        visuals.canvas.gameObject.SetActive(true);
        onComplete?.Invoke();
    }

    public override void InitializeContent(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public void Confirm()
    {
        open = false;
    }

    public void SelectReward(Reward reward, ScreenButton button)
    {
        if(!button.selected)
        {
            chosenRewards.Add(reward);
            if (reward == addCardReward)
            {
                if (chosenRewards.Contains(removeCardReward))
                {
                    chosenRewards.Remove(removeCardReward);
                    visuals.addButton.selected = false;
                }
            }
            if(reward == removeCardReward)
            {
                if (chosenRewards.Contains(removeCardReward))
                {
                    chosenRewards.Remove(removeCardReward);
                    visuals.removeButton.selected = false;
                }
            }

            if(heroRewards.Contains(reward))
            {
                for (int i = 0; i < heroRewards.Count; i++)
                {
                    if(reward != heroRewards[i] && chosenRewards.Contains(heroRewards[i]))
                    {
                        chosenRewards.Remove(heroRewards[i]);
                        visuals.heroRewardsButton[i].selected = false;
                    }
                }
            }
        }
        else
        {
            chosenRewards.Remove(reward);
        }
    }

    ~RewardScreen()
    {
        visuals.confirmButton.onClick -= Confirm;
    }
}

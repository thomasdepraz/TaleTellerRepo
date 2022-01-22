using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : GameScreen
{
    public Reward chosenHeroReward;
    public Reward chosenCardReward;
    public List<Reward> heroRewards;

    public AddCardReward addCardReward;
    public RemoveCardReward removeCardReward;

    public RewardScreenVisuals visuals;

    public RewardScreen(RewardInfo currentRewardInfo, MainPlotScheme currentScheme)
    {
        visuals = ScreenManager.Instance.rewardScreenVisuals; ;
        visuals.completeText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$QUEST_ENDING");
        visuals.chooseInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CHOOSE_INSTRUCTION"); 
        visuals.upgradeInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$UPGRADE_INSTRUCTION");
        visuals.confirmButton.SetText(LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CONFIRM"));

        visuals.questText.text = currentScheme.schemeSteps[currentScheme.currentStep].chapterDescription;
        visuals.illustration = null;//TODO

        addCardReward = new AddCardReward(2);
        removeCardReward = new RemoveCardReward(3);//TODO EXPOSE MAGIC NUMBERS

        heroRewards = GenerateRewards(currentRewardInfo.type);

        for (int i = 0; i < visuals.heroRewardsButton.Count; i++)
        {
            visuals.heroRewardsButton[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < heroRewards.Count; i++)
        {
            int index = i;

            visuals.heroRewardsButton[index].gameObject.SetActive(true);
            visuals.heroRewardsButton[index].onClick = () => SelectHeroReward(heroRewards[index], visuals.heroRewardsButton[index]);
            visuals.heroRewardsButton[index].SetText(heroRewards[index].GetString());
        }

        visuals.addButton.onClick = () => SelectCardReward(addCardReward, visuals.addButton);
        visuals.addButton.SetText(addCardReward.GetString());
        visuals.removeButton.onClick = () => SelectCardReward(removeCardReward, visuals.removeButton);
        visuals.removeButton.SetText(removeCardReward.GetString());

        visuals.confirmButton.onClick = Confirm;
        visuals.confirmButton.interactable = false;

        LayoutRebuilder.ForceRebuildLayoutImmediate(visuals.layoutRoot);
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
        open = true;
        visuals.canvas.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(visuals.layoutRoot);
        onComplete?.Invoke();
    }
    public override void Close(Action onComplete)
    {
        visuals.canvas.gameObject.SetActive(false);
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

    public void SelectCardReward(Reward reward, ScreenButton button)
    {
        if(button.selected)
        {
            if(chosenCardReward != null)
            {
                if(button == visuals.addButton)
                {
                    visuals.removeButton.selected = false;
                }
                else
                {
                    visuals.addButton.selected = false;
                }
            }

            chosenCardReward = reward;
        }
        else
        {
            chosenCardReward = null;
        }

        visuals.confirmButton.interactable = CheckValid();

    }
    public void SelectHeroReward(Reward reward, ScreenButton button)
    {
        if(button.selected)
        {
            if(chosenHeroReward != null)
            {
                for (int i = 0; i < heroRewards.Count; i++)
                {
                    if (visuals.heroRewardsButton[i] != button)
                        visuals.heroRewardsButton[i].selected = false;
                }
            }

            chosenHeroReward = reward;
        }
        else
        {

            chosenHeroReward = null;
        }

        visuals.confirmButton.interactable = CheckValid();
    }

    public bool CheckValid()
    {
        return chosenHeroReward != null && chosenCardReward!= null;
    }

  

    ~RewardScreen()
    {
        visuals.confirmButton.onClick -= Confirm;
    }
}
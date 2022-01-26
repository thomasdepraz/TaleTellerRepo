using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : GameScreen
{
    public MainPlotScheme currentScheme;
    public Reward chosenHeroReward;
    public Reward chosenCardReward;
    public List<Reward> heroRewards;

    public AddCardReward addCardReward;
    public RemoveCardReward removeCardReward;

    public RewardScreenVisuals visuals;

    public Action<Reward, ScreenButton> selectCard;
    public Action<Reward, ScreenButton> selectHero;
    public List<CardData> addRewardData;

    public RewardScreen(RewardInfo currentRewardInfo, MainPlotScheme currentScheme, List<CardData> addRewardData = null)
    {
        ScreenManager.Instance.currentScreen = this;
        visuals = ScreenManager.Instance.rewardScreenVisuals; ;

        this.currentScheme = currentScheme;
        selectCard = SelectCardReward;
        selectHero = SelectHeroReward;

        #region Reward
        if (addRewardData == null) addCardReward = new AddCardReward(2);
        else addCardReward = new AddCardReward(addRewardData, 2);

        removeCardReward = new RemoveCardReward(3);//TODO EXPOSE MAGIC NUMBERS

        heroRewards = GenerateRewards(currentRewardInfo.type);
        #endregion

        visuals.Initialize(this);
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
        visuals.Open(onComplete);
        LayoutRebuilder.ForceRebuildLayoutImmediate(visuals.layoutRoot);
    }
    public override void Close(Action onComplete)
    {
        visuals.Close(onComplete);
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
}

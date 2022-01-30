using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTooltip : UITooltip
{
    public RewardInfo rewardInfo;

    public override string GetTooltipDescription()
    {
        string baseString = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$REWARD");
        string rewardType = string.Empty;
        string rewardRarity = string.Empty;
        string result = string.Empty;

        //TODO Localize strings
        switch (rewardInfo.rarity)
        {
            case RewardRarity.NONE:
                rewardRarity = "NONE";
                break;
            case RewardRarity.COMMON:
                rewardRarity = "COMMON";
                break;
            case RewardRarity.RARE:
                rewardRarity = "RARE";
                break;
            case RewardRarity.EPIC:
                rewardRarity = "EPIC";
                break;
            default:
                break;
        }

        switch (rewardInfo.type)
        {
            case RewardType.MAX_STATS:
                rewardType = "MAX_STATS";
                break;
            case RewardType.TEMP_STATS:
                rewardType = "TEMP_STATS";
                break;
            case RewardType.INSPIRE:
                rewardType = "INSPIRE";
                break;
            default:
                break;
        }
        result = string.Format(baseString, rewardRarity, rewardType);

        return result;
    }

}

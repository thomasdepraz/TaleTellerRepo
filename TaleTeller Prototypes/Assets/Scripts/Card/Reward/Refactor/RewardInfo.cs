using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardRarity
{
    NONE,
    COMMON, 
    RARE,
    EPIC
}

public enum RewardType
{
    MAX_STATS,
    TEMP_STATS,
    INSPIRE
}
public class RewardInfo
{
    public RewardType type;
    public RewardRarity rarity;

    public RewardInfo(int actNumber)
    {
        type = GetRewardType();
        rarity = GetRewardRarity(actNumber);
    }

    public RewardInfo(RewardRarity rarity)
    {
        type = GetRewardType();
        this.rarity = rarity;
    }

    public static RewardType GetRewardType()
    {
        RewardType result = RewardType.MAX_STATS;
        Vector3 rates = RewardManager.Instance.rewardProfile.rewardTypeRates;
        float random = Random.Range(0f, rates.x + rates.y + rates.z);

        if (random <= rates.x)
            result = RewardType.MAX_STATS;
        else if (random > rates.x && random <= rates.x+rates.y)
            result = RewardType.TEMP_STATS;
        else if (random > rates.x + rates.y && random <= rates.x+rates.y+rates.z)
            result = RewardType.TEMP_STATS;

        return result;
    }

    public static RewardRarity GetRewardRarity(int actNumber)
    {
        Vector3 rates = Vector3.zero;
        switch (actNumber)
        {
            case 0:
                rates = RewardManager.Instance.rewardProfile.actDropRates[0];
                break;
            case 1:
                rates = RewardManager.Instance.rewardProfile.actDropRates[1];
                break;
            case 2:
                rates = RewardManager.Instance.rewardProfile.actDropRates[2];
                break;
            default:
                Debug.LogWarning($"actNumber : {actNumber} is invalid");
                return RewardRarity.NONE;
        }
        return GetRandomRarity(rates.x, rates.y, rates.z);
    }

    public static  RewardRarity GetRandomRarity(float commonWeight, float rareWeight, float epicWeight)
    {
        float random = Random.Range(0f, 100f);
        if (random <= commonWeight)
            return RewardRarity.COMMON;
        else if (random > commonWeight && random <= commonWeight + rareWeight)
            return RewardRarity.RARE;
        else if (random > commonWeight + rareWeight && random <= commonWeight + rareWeight + epicWeight)
            return RewardRarity.EPIC;

        return RewardRarity.NONE;
    }
}



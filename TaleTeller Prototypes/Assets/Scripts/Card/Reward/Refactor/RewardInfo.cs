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
        float random = Random.Range(0f,100f);

        if (random <= 50f) //50%
            result = RewardType.MAX_STATS;
        else if (random > 50f && random <= 90f) //40%
            result = RewardType.TEMP_STATS;
        else if (random > 90f) //10%
            result = RewardType.TEMP_STATS;

        return result;
    }

    public static RewardRarity GetRewardRarity(int actNumber)
    {
        switch (actNumber)
        {
            case 0:
                return GetRandomRarity(65f, 25f, 10f);
            case 1:
                return GetRandomRarity(45f, 40f, 15f);
            case 2:
                return GetRandomRarity(20f, 60f, 20f);
            default:
                Debug.LogWarning($"actNumber : {actNumber} is invalid");
                return RewardRarity.NONE;
        }
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



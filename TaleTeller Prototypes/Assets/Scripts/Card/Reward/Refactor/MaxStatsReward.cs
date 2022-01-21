using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxStatsReward : Reward
{
    int buffValue;
    MaxHeroStats statType;

    public MaxStatsReward(MaxHeroStats statType)
    {
        info = RewardManager.Instance.currentRewardInfo;
        this.statType = statType;
        buffValue = GetBuffValue(statType, info.rarity);
    }

    public static MaxHeroStats GetStatType()
    {
        float r = Random.Range(0f, 100f);
        if (r <= 50f) return MaxHeroStats.MAX_PURSE;
        else return MaxHeroStats.MAX_HEALTH;
    }

    public static MaxHeroStats GetStatType(MaxHeroStats previousType)
    {
        float r = Random.Range(0f, 100f);
        if (previousType == MaxHeroStats.MAX_PURSE)
        {
            if (r <= 80f) return MaxHeroStats.MAX_HEALTH;
            else return MaxHeroStats.MAX_ATTACK;

        }
        else
        {
            if (r <= 80f) return MaxHeroStats.MAX_PURSE;
            else return MaxHeroStats.MAX_ATTACK;
        }
    }

    public int GetBuffValue(MaxHeroStats statType, RewardRarity rarity)
    {
        switch (rarity)
        {
            case RewardRarity.COMMON:
                return GetValueFromType(statType, 3, 2, 1);
            case RewardRarity.RARE:
                return GetValueFromType(statType, 5, 3, 1);
            case RewardRarity.EPIC:
                return GetValueFromType(statType, 10, 5, 2);
            default:
                Debug.LogWarning($"{rarity} is not a valid type");
                return 0;
        }
    }

    int GetValueFromType(MaxHeroStats statType, int purseValue, int maxHpValue, int maxAttackValue)
    {
        switch (statType)
        {
            case MaxHeroStats.MAX_ATTACK:
                return maxAttackValue;
            case MaxHeroStats.MAX_HEALTH:
                return maxHpValue;
            case MaxHeroStats.MAX_PURSE:
                return purseValue;
            default:
                return 0;
        }
    }

    public override IEnumerator ApplyRewardRoutine(EventQueue queue)
    {
        switch (statType)
        {
            case MaxHeroStats.MAX_ATTACK:
                GameManager.Instance.currentHero.attackDamage += buffValue;
                break;
            case MaxHeroStats.MAX_HEALTH:
                GameManager.Instance.currentHero.maxLifePoints += buffValue;
                break;
            case MaxHeroStats.MAX_PURSE:
                GameManager.Instance.currentHero.maxGoldPoints += buffValue;
                break;
            default:
                break;
        }
        yield return new WaitForEndOfFrame();
        queue.UpdateQueue();
    }

    public override string GetString()
    {
        throw new System.NotImplementedException();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempStatsReward : Reward
{
    int buffValue;
    TempHeroStats statType;

    public TempStatsReward(TempHeroStats statType)
    {
        info = RewardManager.Instance.currentRewardInfo;
        this.statType = statType;
        buffValue = GetBuffValue(statType, info.rarity);
    }

    public static TempHeroStats GetStatType()
    {
        float r = Random.Range(0f, 100f);
        if (r <= 50f) return TempHeroStats.GOLD;
        else return TempHeroStats.HEALTH;
    }

    public static TempHeroStats GetStatType(TempHeroStats previousType)
    {
        float r = Random.Range(0f, 100f);
        if (previousType == TempHeroStats.GOLD)
        {
            if (r <= 50f) return TempHeroStats.HEALTH;
            else return TempHeroStats.TEMP_ATTACK;

        }
        else
        {
            if (r <= 50f) return TempHeroStats.GOLD;
            else return TempHeroStats.TEMP_ATTACK;
        }
    }

    public int GetBuffValue(TempHeroStats statType, RewardRarity rarity)
    {
        Vector3 values = Vector3.zero;
        switch (rarity)
        {
            case RewardRarity.COMMON:
                values = RewardManager.Instance.rewardProfile.tempStatsRewardValues[0];
                break;
            case RewardRarity.RARE:
                values = RewardManager.Instance.rewardProfile.tempStatsRewardValues[1];
                break;
            case RewardRarity.EPIC:
                values = RewardManager.Instance.rewardProfile.tempStatsRewardValues[2];
                break;
            default:
                Debug.LogWarning($"{rarity} is not a valid type");
                return 0;
        }
        return GetValueFromType(statType, (int)values.x, (int)values.y, (int)values.z);
    }

    int GetValueFromType(TempHeroStats statType, int goldValue, int healthValue, int bonusAttackValue)
    {
        switch (statType)
        {
            case TempHeroStats.TEMP_ATTACK:
                return bonusAttackValue;
            case TempHeroStats.HEALTH:
                return healthValue;
            case TempHeroStats.GOLD:
                return goldValue;
            default:
                return 0;
        }
    }

    public override IEnumerator ApplyRewardRoutine(EventQueue queue)
    {
        switch (statType)
        {
            case TempHeroStats.TEMP_ATTACK:
                GameManager.Instance.currentHero.bonusDamage += buffValue;
                break;
            case TempHeroStats.HEALTH:
                GameManager.Instance.currentHero.lifePoints += buffValue;
                break;
            case TempHeroStats.GOLD:
                GameManager.Instance.currentHero.goldPoints += buffValue;
                break;
            default:
                break;
        }
        yield return new WaitForEndOfFrame();
        queue.UpdateQueue();
    }

    public override string GetString()
    {
        string sprite =string.Empty;
        switch (statType)
        {
            case TempHeroStats.TEMP_ATTACK:
                sprite = "<sprite name= \"AttackTemp\">";
                break;
            case TempHeroStats.HEALTH:
                sprite = "<sprite name= \"Heal\">";
                break;
            case TempHeroStats.GOLD:
                sprite = "<sprite name= \"Gold\">";
                break;
            default:
                break;
        }

        return $"+ {buffValue} {sprite}";
    }
}

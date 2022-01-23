using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaxHeroStats
{
    MAX_ATTACK, 
    MAX_HEALTH, 
    MAX_PURSE
}
public enum TempHeroStats
{
    TEMP_ATTACK,
    HEALTH,
    GOLD
}
public abstract class Reward
{
    public RewardInfo info;

    public void ApplyReward(EventQueue queue)
    {
        queue.events.Add(ApplyRewardRoutine(queue));
    }

    public abstract IEnumerator ApplyRewardRoutine(EventQueue queue);

    public abstract string GetString();
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspireTokenReward : Reward
{
    public int stackCount;
    public InspireTokenReward(int stackCount)
    {
        this.stackCount = stackCount;
    }

    public override IEnumerator ApplyRewardRoutine(EventQueue queue)
    {
        CardManager.Instance.inspire.UpdateStacks(stackCount);
        yield return new WaitForEndOfFrame();
        queue.UpdateQueue();
    }

    public override string GetString()
    {
        throw new System.NotImplementedException();
    }
}

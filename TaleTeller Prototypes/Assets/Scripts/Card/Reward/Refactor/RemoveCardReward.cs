using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCardReward : Reward
{
    public int numberToDiscard;
    
    public RemoveCardReward(int numberToDiscard)
    {
        this.numberToDiscard = numberToDiscard;
    }

    public override IEnumerator ApplyRewardRoutine(EventQueue queue)
    {
        EventQueue discardQueue = new EventQueue();
        CardManager.Instance.DiscardFromDeck(numberToDiscard ,discardQueue);
        discardQueue.StartQueue();
        while (!discardQueue.resolved) yield return new WaitForEndOfFrame();
    }

    public override string GetString()
    {
        return $"Remove {numberToDiscard} cards";
    }
}

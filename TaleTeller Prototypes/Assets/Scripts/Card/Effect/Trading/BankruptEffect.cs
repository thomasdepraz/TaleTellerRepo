using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankruptEffect : Effect
{
    public EffectValue bankruptValue;

    public Effect effectTriggerIfBankrupt;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(bankruptValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        yield return null;

        EventQueue bankruptQueue = new EventQueue();

        if (GameManager.Instance.currentHero.goldPoints <= bankruptValue.value)
        {
            //Bankrupt Feedback 
            yield return new WaitForSeconds(1);

            linkedData.effects[linkedData.effects.IndexOf(this) + 1].OnTriggerEffect(bankruptQueue);

            //effectTriggerIfCanPay.OnTriggerEffect(buyQueue);
        }

        bankruptQueue.StartQueue();
        while (!bankruptQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }
}

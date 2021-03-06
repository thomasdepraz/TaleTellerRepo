using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyEffect : ConditionalTriggerEffects
{
    public EffectValue buyValue;

    public Effect effectTriggerIfCanPay;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(buyValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        yield return null;

        EventQueue buyQueue = new EventQueue();

        if (GameManager.Instance.currentHero.goldPoints >= buyValue.value)
        {
            GameManager.Instance.currentHero.goldPoints -= buyValue.value;
            //Buy Feedback 
            yield return new WaitForSeconds(1);

            linkedData.effects[linkedData.effects.IndexOf(this) + 1].OnTriggerEffect(buyQueue);

            //effectTriggerIfCanPay.OnTriggerEffect(buyQueue);
        }

        buyQueue.StartQueue();
        while (!buyQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }
}

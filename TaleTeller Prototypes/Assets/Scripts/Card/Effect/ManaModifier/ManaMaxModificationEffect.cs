using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaMaxModificationEffect: ManaEffect
{
    public EffectValue manaModificationValue;
    public int turnToLast;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(manaModificationValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        var manaModifier = new ManaSystem.ManaPoolModifier(manaModificationValue.value, turnToLast, this);
        CardManager.Instance.manaSystem.AddManaPoolModifier(manaModifier);
        
        yield return null;
        currentQueue.UpdateQueue();
    }
}

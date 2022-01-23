using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscardEffect : DiscardTypeEffects
{
    public EffectValue discardValue;
    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(discardValue);
    }

    protected override int GetAmountToDiscard()
    {
        return discardValue.value;
    }
}

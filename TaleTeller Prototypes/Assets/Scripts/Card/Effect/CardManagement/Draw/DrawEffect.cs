using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : DrawTypeEffects
{
    public EffectValue drawValue;
    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(drawValue);
    }

    protected override int GetAmountToDraw()
    {
        return drawValue.value;
    }
}

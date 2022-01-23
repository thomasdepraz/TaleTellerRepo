using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Remi Secher - 12/10/2021 00:46 - Creation
/// Draw X Card, X is the number of slot that the hero walked across before activating this card
/// </summary>
public class JourneyDrawEffect : DrawTypeEffects
{
    public EffectValue drawValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(drawValue);
    }

    protected override int GetAmountToDraw()
    {
        return drawValue.value * (linkedData.currentContainer.currentSlot.slotIndex + 1);
    }
}

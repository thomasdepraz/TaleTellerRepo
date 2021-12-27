using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscardDependingCardTypeEffect : DiscardTypeEffects
{
    public enum CardTypeParameter { All, Object, Character, Location };

    public CardTypeParameter cardTypeTarget;
    public EffectValue discardValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(discardValue);
    }

    protected override int GetAmountToDiscard()
    {
        int amountToDiscard = 0;
        var possibleTargets = GetTargets();

        switch (cardTypeTarget)
        {
            case CardTypeParameter.All:
                amountToDiscard = possibleTargets.Count;
                break;

            case CardTypeParameter.Character:
                 amountToDiscard = possibleTargets
                    .Select(t => t.cardType)
                    .Where(ct => ct.GetType() == typeof(CharacterType))
                    .Count();
                break;

            case CardTypeParameter.Location:
                amountToDiscard = possibleTargets
                    .Select(t => t.cardType)
                    .Where(ct => ct.GetType() == typeof(LocationType))
                    .Count();
                break;

            case CardTypeParameter.Object:
                amountToDiscard = possibleTargets
                    .Select(t => t.cardType)
                    .Where(ct => ct.GetType() == typeof(ObjectType))
                    .Count();
                break;
        }

        return amountToDiscard * discardValue.value;
    }
}

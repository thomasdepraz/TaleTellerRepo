using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawDependingCardTypeEffect : DrawTypeEffects
{
    public enum CardTypeParameter {All, Object, Character, Location };

    public CardTypeParameter cardTypeTarget;
    public EffectValue drawValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(drawValue);
    }

    protected override int GetAmountToDraw()
    {
        int amountToDraw = 0;
        var possibleTargets = GetTargets();

        switch (cardTypeTarget)
        {
            case CardTypeParameter.All:
                amountToDraw = possibleTargets.Count;
                break;

            case CardTypeParameter.Character:
                amountToDraw = possibleTargets
                   .Select(t => t.cardType)
                   .Where(ct => ct.GetType() == typeof(CharacterType))
                   .Count();
                break;

            case CardTypeParameter.Location:
                amountToDraw = possibleTargets
                    .Select(t => t.cardType)
                    .Where(ct => ct.GetType() == typeof(LocationType))
                    .Count();
                break;

            case CardTypeParameter.Object:
                amountToDraw = possibleTargets
                    .Select(t => t.cardType)
                    .Where(ct => ct.GetType() == typeof(ObjectType))
                    .Count();
                break;
        }

        return amountToDraw * drawValue.value;
    }
}


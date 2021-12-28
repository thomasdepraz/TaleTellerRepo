using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaMaxModificationEffect: ManaEffect
{
    public EffectValue manaModificationValue;
    public int turnToLast;
    public bool triggerNextTurn;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(manaModificationValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        var manaSystem = CardManager.Instance.manaSystem;

        int modifierValue = 0;

        switch (manaModificationValue.op)
        {
            case EffectValueOperator.Addition:
                modifierValue = manaModificationValue.value;
                break;

            case EffectValueOperator.Division:
                modifierValue = -((manaSystem.maxManaBase / manaModificationValue.value) * (manaModificationValue.value - 1));
                break;

            case EffectValueOperator.Product:
                modifierValue = manaSystem.maxManaBase * (manaModificationValue.value - 1);
                break;

            case EffectValueOperator.Substraction:
                modifierValue = -manaModificationValue.value;
                break;

            default:
                break;
        }

        var manaModifier = manaSystem.CreateManaPoolModifier(modifierValue, turnToLast, this);
        CardManager.Instance.manaSystem.AddManaPoolModifier(manaModifier, triggerNextTurn);
        
        yield return null;
        currentQueue.UpdateQueue();
    }
}

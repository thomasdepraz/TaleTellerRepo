using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using NaughtyAttributes;

public class DrawDependingEffectTypeEffect : DrawTypeEffects
{
    //Considering the purpose of this effect it's better to place a security by letting ops choose only beetween a range of supported fields
    //If needed add a new type of effect Value in the following enum and adapat the method GetAmountToDraw()
    internal enum CustomEffectValueType { Draw, Discard, Buy, Bankrupt }

    [Serializable]
    internal struct EffectTypeTarget
    {
        bool showOperator => false;
        // If you add new effect value type and you need ops to specify and operator
        // replace false by a condition like bellow (Draw and/or discard should be the value that need operator); 
        //    type == CustomEffectValueType.Draw || 
        //    type == CustomEffectValueType.Discard;

        [SerializeField]
        internal CustomEffectValueType type;
        [ShowIf("showOperator"), AllowNesting, SerializeField]
        internal EffectValueOperator op;
    }

    [SerializeField]
    EffectTypeTarget effectTypeToTarget;
    public EffectValue drawValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(drawValue);
    }

    protected override int GetAmountToDraw()
    {
        int amountToDraw = 0;
        List<Effect> possibleTargets = new List<Effect>();

        var tempTargets = GetTargets().Select(t => t.effects);
        foreach (List<Effect> lEffets in tempTargets)
            possibleTargets.AddRange(lEffets);

        int ParameterTesting(Effect _effect, EffectValueType type, EffectValueOperator op = EffectValueOperator.None)
        {
            var allEffectValues = _effect.values;
            int counter = 0;

            foreach (EffectValue val in allEffectValues)
            {
                if (val.op == op && val.type == type)
                    counter++;
            }

            return counter;
        }

        switch (effectTypeToTarget.type)
        {
            case CustomEffectValueType.Buy:
                foreach (Effect _effect in possibleTargets)
                    amountToDraw += ParameterTesting(_effect, EffectValueType.Buy);
                break;

            case CustomEffectValueType.Bankrupt:
                foreach (Effect _effect in possibleTargets)
                    amountToDraw += ParameterTesting(_effect, EffectValueType.Bankrupt);
                break;

            case CustomEffectValueType.Discard:
                foreach (Effect _effect in possibleTargets)
                    amountToDraw += ParameterTesting(_effect, EffectValueType.Card, EffectValueOperator.Substraction);
                break;

            case CustomEffectValueType.Draw:
                foreach (Effect _effect in possibleTargets)
                    amountToDraw += ParameterTesting(_effect, EffectValueType.Card, EffectValueOperator.Addition);
                break;
        }

        return amountToDraw * drawValue.value;
    }
}

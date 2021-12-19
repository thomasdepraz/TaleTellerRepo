using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ValueNulliferEffect : ValueModifierEffect
{
    internal struct NullifiedValuesInfos
    {
        internal Effect effect;
        internal EffectValue value;
        internal int originalValue;

        internal NullifiedValuesInfos(Effect _effect, EffectValue _value, int _originalValue)
        {
            effect = _effect;
            value = _value;
            originalValue = _originalValue;
        }
    }

    [Header("Nullify Params")]
    public EffectValueType targetedType;
    public EffectValueOperator targetedOperator;

    [Tooltip("Check this if you want the nullify effect to reverse on end of the story")]
    public bool nullifyTemporary;

    List<NullifiedValuesInfos> valuesInfos = new List<NullifiedValuesInfos>();

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        if (nullifyTemporary)
            card.onStoryEnd += StartReverseNullify;
    }

    public void StartReverseNullify(EventQueue queue)
    {
        queue.events.Add(ReverseNullify(queue));
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        var targetedCards = GetTargets();
        List<Effect> targetedEffect = new List<Effect>();

        foreach (CardData card in targetedCards)
        {
            targetedEffect.AddRange(
                card.effects.Where(e => e.values.Any(v => v.type == targetedType && v.op == targetedOperator)));
        }

        foreach (Effect effect in targetedEffect)
        {
            var targetedValues = effect.values.Where(v => v.type == targetedType && v.op == targetedOperator);

            foreach (EffectValue value in targetedValues)
            {
                valuesInfos.Add(new NullifiedValuesInfos(effect, value, value.value));
                value.value = 0;
            }
        }

        yield return null;
        currentQueue.UpdateQueue();
    }

    IEnumerator ReverseNullify(EventQueue currentQueue)
    {
        foreach (NullifiedValuesInfos infos in valuesInfos)
        {
            if (infos.effect?.linkedData?.currentContainer)
                infos.effect.values.Where(v => v == infos.value).First().value += infos.originalValue;
        }

        yield return null;
        currentQueue.UpdateQueue();
    }
}

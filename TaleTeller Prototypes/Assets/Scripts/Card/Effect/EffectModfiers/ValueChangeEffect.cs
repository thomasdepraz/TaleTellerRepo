using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ValueChangeEffect : ValueModifierEffect
{
    internal struct ModifyValuesInfos
    {
        internal Effect effect;
        internal EffectValue value;

        internal ModifyValuesInfos(Effect _effect, EffectValue _value)
        {
            effect = _effect;
            value = _value;
        }
    }

    [Serializable]
    private struct TypeToTarget
    {
        internal EffectValueType typeToTarget;
        internal EffectValueOperator operatorToTarget;
        internal EffectValue modification;
    }

    [Header("Modifier Param")]
    private TypeToTarget allEffectValueModification;
    [Tooltip("Check this to revert the modification on storyEnd")]
    public bool modifyTemporary;

    List<ModifyValuesInfos> valuesInfos = new List<ModifyValuesInfos>();

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

            values.Add(allEffectValueModification.modification);

        if (modifyTemporary)
            card.onStoryEnd += StartReverseModify;
    }

    private void StartReverseModify(EventQueue queue)
    {
        queue.events.Add(ReverseModify(queue));
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        var targetedCards = GetTargets();
        List<Effect> targetedEffect = new List<Effect>();

        foreach (CardData card in targetedCards)
        {
            targetedEffect.AddRange(
                card.effects.Where(e => e.values.Any(v => v.type == allEffectValueModification.typeToTarget && v.op == allEffectValueModification.operatorToTarget)));
        }

        foreach (Effect effect in targetedEffect)
        {
            var targetedValues = effect.values.Where(v => v.type == allEffectValueModification.typeToTarget && v.op == allEffectValueModification.operatorToTarget) ;

            foreach (EffectValue value in targetedValues)
            {
                valuesInfos.Add(new ModifyValuesInfos(effect, value));

                if (allEffectValueModification.typeToTarget != EffectValueType.Card)
                {
                    switch (allEffectValueModification.modification.op)
                    {
                        case EffectValueOperator.Addition:
                            value.value += allEffectValueModification.modification.value;
                            break;

                        case EffectValueOperator.Division:
                            value.value /= allEffectValueModification.modification.value;
                            break;

                        case EffectValueOperator.Product:
                            value.value *= allEffectValueModification.modification.value;
                            break;

                        case EffectValueOperator.Substraction:
                            value.value -= allEffectValueModification.modification.value;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    value.value += allEffectValueModification.modification.value;
                }
            }
        }

        yield return null;
        currentQueue.UpdateQueue();
    }

    private IEnumerator ReverseModify(EventQueue queue)
    {
        foreach (ModifyValuesInfos infos in valuesInfos)
        {
            if (infos.effect?.linkedData?.currentContainer)
            {
                if (effectValueToModifiy.type != EffectValueType.Card)
                {
                    switch (effectValueToModifiy.op)
                    {
                        case EffectValueOperator.Addition:
                            infos.effect.values.Where(v => v == infos.value).First().value -= effectValueToModifiy.value;
                            break;

                        case EffectValueOperator.Division:
                            infos.effect.values.Where(v => v == infos.value).First().value *= effectValueToModifiy.value;
                            break;

                        case EffectValueOperator.Product:
                            infos.effect.values.Where(v => v == infos.value).First().value /= effectValueToModifiy.value;
                            break;

                        case EffectValueOperator.Substraction:
                            infos.effect.values.Where(v => v == infos.value).First().value += effectValueToModifiy.value;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    infos.effect.values.Where(v => v == infos.value).First().value -= effectValueToModifiy.value;
                }

            }

        }

        yield return null;
        queue.UpdateQueue();
    }
}

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




    [Header("Modifier Param"), SerializeField]

    private EffectValueType typeToTarget;
    [SerializeField]
    private EffectValueOperator operatorToTarget;
    public  EffectValue modification;
    [Tooltip("Check this to revert the modification on storyEnd")]
    public bool modifyTemporary;

    List<ModifyValuesInfos> valuesInfos = new List<ModifyValuesInfos>();

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

            values.Add(modification);

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
                card.effects.Where(e => e.values.Any(v => v.type == typeToTarget && v.op == operatorToTarget)));
        }

        EventQueue feedbackQueue = new EventQueue();

        foreach (Effect effect in targetedEffect)
        {
            var targetedValues = effect.values.Where(v => v.type == typeToTarget && v.op == operatorToTarget) ;

             foreach (EffectValue value in targetedValues)
             {
                 valuesInfos.Add(new ModifyValuesInfos(effect, value));

                switch (modification.op)
                {
                    case EffectValueOperator.Addition:
                        value.value += modification.value;
                        CardManager.Instance.cardTweening.EffectChangeFeedback(effect.linkedData.currentContainer, 1, modification.value,feedbackQueue);
                        break;

                    case EffectValueOperator.Division:
                        value.value /= modification.value;
                        CardManager.Instance.cardTweening.EffectChangeFeedback(effect.linkedData.currentContainer, -1, modification.value, feedbackQueue);
                        break;

                    case EffectValueOperator.Product:
                        value.value *= modification.value;
                        CardManager.Instance.cardTweening.EffectChangeFeedback(effect.linkedData.currentContainer, 1, modification.value, feedbackQueue);
                        break;

                    case EffectValueOperator.Substraction:
                        value.value -= modification.value;
                        CardManager.Instance.cardTweening.EffectChangeFeedback(effect.linkedData.currentContainer, -1, modification.value, feedbackQueue);
                        break;

                    default:
                        feedbackQueue.resolved = true;
                        break;
                }

                effect.linkedData.currentContainer.UpdateBaseInfo();
             }

            if (targetedValues.Count() == 0)
            {
                feedbackQueue.resolved = true;
            }
        }

        if (targetedEffect.Count == 0)
        {
            feedbackQueue.resolved = true;
        }

        while (!feedbackQueue.resolved)//Wait 
        {
            yield return new WaitForEndOfFrame();
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
                switch (modification.op)
                {
                    case EffectValueOperator.Addition:
                        infos.effect.values.Where(v => v == infos.value).First().value -= modification.value;
                        break;

                    case EffectValueOperator.Division:
                        infos.effect.values.Where(v => v == infos.value).First().value *= modification.value;
                        break;

                    case EffectValueOperator.Product:
                        infos.effect.values.Where(v => v == infos.value).First().value /= modification.value;
                        break;

                    case EffectValueOperator.Substraction:
                        infos.effect.values.Where(v => v == infos.value).First().value += modification.value;
                        break;

                    default:
                        break;
                }

             }

         }

         yield return null;
         queue.UpdateQueue();
     }
}

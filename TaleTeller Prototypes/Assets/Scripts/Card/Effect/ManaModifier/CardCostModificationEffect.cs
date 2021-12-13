using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCostModificationEffect : ManaEffect
{
    enum HandTargetParam { Random, All}
    bool ShowHandParam => target == EffectTarget.Hand;
    bool ShowRandomAmountParam => handTarget == HandTargetParam.Random && ShowHandParam;

    [Header("Hand Target Param")]
    [SerializeField, ShowIf("ShowHandParam")]
    HandTargetParam handTarget;
    [ShowIf("ShowRandomAmountParam")]
    public int cardAmount;

    [Header("Cost Modification Value")]
    public EffectValue manaCostModification;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(manaCostModification);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        ModifiyManaCost(GetTargetsExtension());
        yield return null;
        currentQueue.UpdateQueue();
    }

    protected List<CardData> GetTargetsExtension()
    {
        List<CardData> cardToImpact = GetTargets();

        if (target == EffectTarget.Hand && handTarget == HandTargetParam.Random)
        {
            var tempCollection = new List<CardData>();

            for (int i = 0; i < cardAmount; i++)
            {
                int randomIndex = Random.Range(0, cardToImpact.Count());
                tempCollection.Add(cardToImpact[randomIndex]);
                cardToImpact.RemoveAt(randomIndex);
            }

            cardToImpact = tempCollection;
        }

        return cardToImpact;
    }

    protected void ModifiyManaCost(List<CardData> cards)
    {
        foreach(CardData card in cards)
        {
            switch (manaCostModification.op)
            {
                case EffectValueOperator.Addition:
                    card.manaCost += manaCostModification.value;
                    break;

                case EffectValueOperator.Division:
                    card.manaCost /= manaCostModification.value;
                    break;

                case EffectValueOperator.Product:
                    card.manaCost *= manaCostModification.value;
                    break;

                case EffectValueOperator.Substraction:
                    card.manaCost -= manaCostModification.value;
                    break;

                default:
                    break;
            } 
        }
    }
}

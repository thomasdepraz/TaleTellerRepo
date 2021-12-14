using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscardXEffect : Effect
{
    public EffectValue xParameterMultiplier;

    public EffectValue xParameter;
    public enum CardTypeParameter { None, All, Object, Character, Location };

    public CardTypeParameter cardTypeParameter;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(xParameterMultiplier);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue discardQueue = new EventQueue();

        List<CardData> targetsToParameter = GetTargets();

        int numberOfCardsToDiscard = 0;

        if (cardTypeParameter == CardTypeParameter.All)
        {
            numberOfCardsToDiscard = targetsToParameter.Count;
        }
        else if (cardTypeParameter == CardTypeParameter.None)
            for (int i = 0; i < targetsToParameter.Count; i++)
            {
                if (linkedData != targetsToParameter[i])
                    for (int x = 0; x < targetsToParameter[i].effects.Count; x++)
                    {

                        for (int z = 0; z < targetsToParameter[i].effects[x].values.Count; z++)
                        {
                            if (targetsToParameter[i].effects[x].values[z].type == xParameter.type)
                            {
                                numberOfCardsToDiscard += xParameterMultiplier.value;
                            }
                        }
                    }
            }
        else
        {
            for (int i = 0; i < targetsToParameter.Count; i++)
            {
                if (linkedData != targetsToParameter[i])
                {
                    switch (cardTypeParameter)
                    {
                        case CardTypeParameter.Object:
                            if (targetsToParameter[i].cardType is ObjectType)
                            {
                                numberOfCardsToDiscard += xParameterMultiplier.value;
                            }
                            break;

                        case CardTypeParameter.Character:
                            if (targetsToParameter[i].cardType is CharacterType)
                            {
                                numberOfCardsToDiscard += xParameterMultiplier.value;
                            }
                            break;

                        case CardTypeParameter.Location:
                            if (targetsToParameter[i].cardType is LocationType)
                            {
                                numberOfCardsToDiscard += xParameterMultiplier.value;
                            }
                            break;
                    }
                }

            }
        }

        Debug.Log("DiscardEffect");
        yield return null;

        //Get targetlist of cards
        List<CardData> targetsToDiscard = CardManager.Instance.cardHand.GetHandDataList();

        //Eventually filter the targets based on any property of the card you want -----
        targetsToDiscard = targetsToDiscard.Where(t => t.GetType() != typeof(PlotCard)).ToList();

        //----

        //Discard random cards //TODO add logic to queue events

        if (targetsToDiscard.Count > 0)
        {
            EventQueue pickQueue = new EventQueue();
            List<CardData> pickedCards = new List<CardData>();

            CardManager.Instance.cardPicker.Pick(pickQueue, targetsToDiscard, pickedCards, numberOfCardsToDiscard, false);

            pickQueue.StartQueue();
            while (!pickQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }

            //discard all of the picked cards
            for (int i = 0; i < pickedCards.Count; i++)
            {
                CardManager.Instance.cardHand.DiscardCardFromHand(pickedCards[i].currentContainer, discardQueue);
            }

        }

        discardQueue.StartQueue(); //Actual Discard

        while (!discardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }



        currentQueue.UpdateQueue();
    }
}

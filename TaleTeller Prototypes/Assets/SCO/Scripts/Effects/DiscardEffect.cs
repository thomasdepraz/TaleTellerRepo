using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiscardEffect", menuName = "Card Effect/DiscardEffect")]
public class DiscardEffect : CardEffect
{
    [Header("Stats")]
    public int numberToDiscard;
    public override void TriggerEffect(CardData target = null)
    {
        //Make a list of the cards in hand
        List<CardContainer> cardsInHand = new List<CardContainer>();
        for (int i = 0; i < CardManager.Instance.cardHand.currentHand.Count; i++)
        {
            cardsInHand.Add(CardManager.Instance.cardHand.currentHand[i]);
        }


        //Pick as many as numberToDiscard and discard them
        for (int i = 0; i < numberToDiscard; i++)
        {
            if(cardsInHand.Count > 0)
            {
                int r = Random.Range(0,cardsInHand.Count - 1);
                CardManager.Instance.cardDeck.discardPile.Add(cardsInHand[r].data);
                cardsInHand[r].ResetContainer();
                cardsInHand.RemoveAt(r);
            }
        }
    }

}

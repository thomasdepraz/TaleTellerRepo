using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DiscardTopDeckEffect : MalusEffect
{
    public EffectValue discardValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(discardValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        var deckManager = CardManager.Instance.cardDeck;
        var deck = deckManager.cardDeck.ToArray();

        var cardToDiscard = deck.Where(c => Array.IndexOf(deck, c) <= discardValue.value - 1).ToList();

        EventQueue drawQueue = new EventQueue();

        foreach (CardData card in cardToDiscard)
            CardManager.Instance.cardDeck.OverDraw(drawQueue, card);

        drawQueue.StartQueue();
        while (!drawQueue.resolved)
            yield return new WaitForEndOfFrame();

        yield return null;
        currentQueue.UpdateQueue();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFullHandEffect : BonusEffect
{
    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        var cardHand = CardManager.Instance.cardHand;
        int roomInHand = cardHand.maxHandSize - cardHand.currentHand.Count; 
        EventQueue drawQueue = new EventQueue();
        
        CardManager.Instance.cardDeck.DrawCards(roomInHand, drawQueue);

        drawQueue.StartQueue();//Actual draw
        while (!drawQueue.resolved)
            yield return new WaitForEndOfFrame();

        currentQueue.UpdateQueue();
    }
}

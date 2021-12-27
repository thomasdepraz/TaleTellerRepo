using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTypeEffects : Effect
{
    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        int cardToDraw = GetAmountToDraw();
        EventQueue drawQueue = new EventQueue();

        CardManager.Instance.cardDeck.DrawCards(cardToDraw, drawQueue);

        drawQueue.StartQueue();//Actual draw
        while (!drawQueue.resolved)
            yield return new WaitForEndOfFrame();
        currentQueue.UpdateQueue();
    }

    protected virtual int GetAmountToDraw()
    {
        return 1;
    }
}

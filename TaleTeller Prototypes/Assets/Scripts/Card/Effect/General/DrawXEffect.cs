using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawXEffect : Effect
{
    public EffectValue xParameter;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(xParameter);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue drawQueue = new EventQueue();

        List<CardData> targets = GetTargets();

        //Logic that add methods to queue
        Debug.Log("Draw ?");
        CardManager.Instance.cardDeck.DrawCards((int)xParameter.value, drawQueue);

        drawQueue.StartQueue();//Actual draw
        while (!drawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }



        currentQueue.UpdateQueue();
    }
}

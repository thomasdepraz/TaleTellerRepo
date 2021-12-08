using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : BonusEffect
{
    public EffectValue drawValue;
    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(drawValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue)
    {
        EventQueue drawQueue = new EventQueue();


        //Logic that add methods to queue
        Debug.Log("Draw ?");
        CardManager.Instance.cardDeck.DrawCards((int)drawValue.value, drawQueue);

        drawQueue.StartQueue();//Actual draw
        while (!drawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }



        currentQueue.UpdateQueue();
    }
}

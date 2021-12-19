using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Remi Secher - 12/10/2021 00:46 - Creation
/// </summary>
public class JourneyDrawEffect : BonusEffect
{
    public EffectValue drawValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(drawValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue drawQueue = new EventQueue();
        int drawAmount = drawValue.value * (linkedData.currentContainer.currentSlot.slotIndex +1 );

        CardManager.Instance.cardDeck.DrawCards(drawAmount, drawQueue);

        drawQueue.StartQueue();//Actual draw
        while (!drawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return null;
        currentQueue.UpdateQueue();
    }
}

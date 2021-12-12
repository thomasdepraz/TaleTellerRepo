using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCEffect : MalusEffect
{
    public EffectValue discardValue;
    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(discardValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue discardQueue = new EventQueue();

        Debug.Log("DiscardEffect");
        yield return null;

        //Get targetlist of cards
        List<CardData> targets = GetTargets();

        //Eventually filter the targets based on any property of the card you want -----


        //----

        //Discard random cards //TODO add logic to queue events

            if (targets.Count > 0)
            {
                EventQueue pickQueue = new EventQueue();
                List<CardData> pickedCards = new List<CardData>();

                CardManager.Instance.cardPicker.Pick(pickQueue, CardManager.Instance.cardHand.GetHandDataList(), pickedCards, discardValue.value, false);

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

        while(!discardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        //Update the event queue <-- This is mandatory
        currentQueue.UpdateQueue();
    }
}

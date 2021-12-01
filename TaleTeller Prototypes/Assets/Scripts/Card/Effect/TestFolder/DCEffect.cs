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

    public override IEnumerator EffectLogic(EventQueue currentQueue)
    {
        EventQueue discardQueue = new EventQueue();

        Debug.Log("DiscardEffect");
        yield return null;

        //Get targetlist of cards
        List<CardData> targets = GetTargets();

        //Eventually filter the targets based on any property of the card you want -----


        //----

        //Discard random cards //TODO add logic to queue events
        for (int i = 0; i < discardValue.value; i++)
        {
            if (targets.Count > 0)
            {
                int r = Random.Range(0, targets.Count - 1);

                if (target == EffectTarget.Hand)
                {
                    CardManager.Instance.cardHand.DiscardCardFromHand(targets[r].currentContainer);//TODO missing queue implementation
                }
                else if (target == EffectTarget.Board)
                {
                    CardManager.Instance.board.DiscardCardFromBoard(targets[r].currentContainer, discardQueue);
                }

                targets.RemoveAt(r);

            }
            else break;
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

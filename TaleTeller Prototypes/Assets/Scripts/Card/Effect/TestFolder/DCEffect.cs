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

    public override IEnumerator EffectLogic()
    {
        bool discardEnded = false;

        Debug.Log("DiscardEffect");
        yield return null;

        //Get targetlist of cards
        List<CardData> targets = GetTargets();

        //Eventually filter the targets based on any property of the card you want -----


        //----

        //Discard random cards
        for (int i = 0; i < discardValue.value; i++)
        {
            if (targets.Count > 0)
            {
                int r = Random.Range(0, targets.Count - 1);

                if (target == EffectTarget.Hand)
                {
                    CardManager.Instance.cardHand.DiscardCardFromHand(targets[r].currentContainer);
                    discardEnded = true;//<--This is TEMPORARY, this bool should be handled in the DiscardCardFromHand method
                }
                else if (target == EffectTarget.Board)
                {
                    CardManager.Instance.board.DiscardCardFromBoard(targets[r].currentContainer, ref discardEnded);
                }
                else
                {
                    discardEnded = true;
                }

                while(!discardEnded)
                {
                    yield return new WaitForEndOfFrame();
                }

                targets.RemoveAt(r);

            }
            else break;
        }

        //Update the event queue <-- This is mandatory
        CardManager.Instance.board.UpdateQueue();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscardTypeEffects : Effect
{
    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue discardQueue = new EventQueue();

        int numberOfCardsToDiscard = GetAmountToDiscard();

        //Get targetlist of cards
        List<CardData> targetsToDiscard = CardManager.Instance.cardHand.GetHandDataList();

        //Eventually filter the targets based on any property of the card you want -----
        targetsToDiscard = targetsToDiscard.Where(t => t.GetType() != typeof(PlotCard)).ToList();

        //----



        //Discard cards through picker
        //TODO: [GD] Rework if we choose to keep only discard based on random
        //If we choose to keep only discard based on picker let it this way

        if (targetsToDiscard.Count > 0)
        {
            //Clamp numberToDiscard to prevent softlock
            if (numberOfCardsToDiscard > targetsToDiscard.Count) numberOfCardsToDiscard = targetsToDiscard.Count;

            CardPickerScreen screen = new CardPickerScreen(PickScreenMode.WITHDRAW, numberOfCardsToDiscard, targetsToDiscard, false);
            bool wait = true;
            screen.Open(() => wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }

            while (screen.open) { yield return new WaitForEndOfFrame(); }
            wait = true;
            screen.Close(() => wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }

            //discard all of the picked cards
            for (int i = 0; i < screen.pickedCards.Count; i++)
                CardManager.Instance.CardHandToDiscard(screen.pickedCards[i].container.data.currentContainer, discardQueue);
        }

        discardQueue.StartQueue(); //Actual Discard

        while (!discardQueue.resolved)
            yield return new WaitForEndOfFrame();
        currentQueue.UpdateQueue();
    }

    protected virtual int GetAmountToDiscard()
    {
        return 1;
    }
}

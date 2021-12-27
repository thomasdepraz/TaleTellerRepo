using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceEffect : CardManagementMiscEffects
{
    public EffectValue choiceValue;
    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(choiceValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        

        Debug.Log("ChoiceEffect");
        yield return null;

        //Get targetlist of cards
        
        //Eventually filter the targets based on any property of the card you want -----
        
        for (int i = 0; i < choiceValue.value; i++)
        {
            EventQueue drawQueue = new EventQueue();

            List<CardData> targets = GetTargets();

            List<CardData> cardsForChoice = new List<CardData>();

            int index = targets.Count < 3 ? targets.Count : 3; 
            for (int x = 0; x < index; x++)
            {
                cardsForChoice.Add(targets[x]);
            }

            if (targets.Count > 0)
            {
                EventQueue pickQueue = new EventQueue();
                List<CardData> pickedCards = new List<CardData>();

                CardManager.Instance.cardPicker.Pick(pickQueue, cardsForChoice, pickedCards, 1, false, "Choose one of these cards");

                pickQueue.StartQueue();
                while (!pickQueue.resolved)
                {
                    yield return new WaitForEndOfFrame();
                }

                //Draw the 3 cards
                for (int x = 0; x < cardsForChoice.Count; x++)
                {
                    CardManager.Instance.cardDeck.Deal(drawQueue, cardsForChoice[x]);
                }

                drawQueue.StartQueue();

                while (!drawQueue.resolved)
                {
                    yield return new WaitForEndOfFrame();
                }

                cardsForChoice.Remove(pickedCards[0]);

                EventQueue discardQueue = new EventQueue();

                //Discard all cards but the picked one
                for (int x = 0; x < cardsForChoice.Count; x++)
                {
                    CardManager.Instance.CardHandToDiscard(pickedCards[i].currentContainer, discardQueue);

                }

                discardQueue.StartQueue();
                while(!discardQueue.resolved)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            else break;
        }

         //Actual Discard

        //Update the event queue <-- This is mandatory
        currentQueue.UpdateQueue();
    }
}

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
                //EventQueue pickQueue = new EventQueue();
                //List<CardData> pickedCards = new List<CardData>();

                //string instruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.choiceEffectInstruction);
                //string newInstruction = instruction.Replace("$value$", choiceValue.value.ToString());

                //CardManager.Instance.cardPicker.Pick(pickQueue, cardsForChoice, pickedCards, 1, newInstruction);

                //pickQueue.StartQueue();
                //while (!pickQueue.resolved)
                //{
                //    yield return new WaitForEndOfFrame();
                //}
                CardPickerScreen screen = new CardPickerScreen(PickScreenMode.CHOICE, 1, CardManager.Instance.cardHand.GetHandDataList(), true);
                bool wait = true;
                screen.Open(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }

                while (screen.open) { yield return new WaitForEndOfFrame(); }
                wait = true;
                screen.Close(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }

                cardsForChoice.Remove(screen.pickedCards[0].container.data);

                //Draw the picked card
                CardManager.Instance.cardDeck.cardDeck.Remove(screen.pickedCards[0].container.data);
                CardManager.Instance.CardAppearToHand(screen.pickedCards[0].container.data, drawQueue, CardManager.Instance.deckTransform.position);

                drawQueue.StartQueue();

                while (!drawQueue.resolved)
                {
                    yield return new WaitForEndOfFrame();
                }


                //Discard other cards
                for (int x = 0; x < cardsForChoice.Count; x++)
                {
                    EventQueue discardQueue = new EventQueue();

                    CardManager.Instance.CardAppear(discardQueue,cardsForChoice[i],CardManager.Instance.deckTransform.position);
                    CardManager.Instance.cardDeck.Burn(discardQueue, cardsForChoice[i]);

                    discardQueue.StartQueue();
                    while(!discardQueue.resolved)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            else break;
        }

         //Actual Discard

        //Update the event queue <-- This is mandatory
        currentQueue.UpdateQueue();
    }
}

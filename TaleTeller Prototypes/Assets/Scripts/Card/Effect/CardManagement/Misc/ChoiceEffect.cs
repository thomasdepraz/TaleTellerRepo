using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using NaughtyAttributes;

public class ChoiceEffect : CardManagementMiscEffects
{
    //Considering the purpose of this effect it's better to place a security by letting ops choose only beetween a range of supported fields
    //If needed add a new type of effect Value in the following enum and adapat the method GetAmountToDraw()
    internal enum CustomEffectValueType { Draw, Discard, Buy, Bankrupt }

    [Serializable]
    internal struct EffectTypeTarget
    {
        bool showOperator => false;
        // If you add new effect value type and you need ops to specify and operator
        // replace false by a condition like bellow (Draw and/or discard should be the value that need operator); 
        //    type == CustomEffectValueType.Draw || 
        //    type == CustomEffectValueType.Discard;

        [SerializeField]
        internal CustomEffectValueType type;
        [ShowIf("showOperator"), AllowNesting, SerializeField]
        internal EffectValueOperator op;
    }

    public bool boardEffectTypesDependant;

    [SerializeField]
    [ShowIf("boardEffectTypesDependant")]
    EffectTypeTarget effectTypeToTarget;

    public EffectValue choiceValue;
    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(choiceValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        yield return null;

        //Get targetlist of cards

        //Eventually filter the targets based on any property of the card you want -----

        int trueChoiceValue = choiceValue.value;

        if(boardEffectTypesDependant)
        {
            trueChoiceValue = GetAmountToChoice();
        }
        
        for (int i = 0; i < trueChoiceValue; i++)
        {
            EventQueue drawQueue = new EventQueue();

            List<CardData> targets = CardManager.Instance.cardDeck.cardDeck;

            List<CardData> cardsForChoice = new List<CardData>();

            int index = targets.Count < 3 ? targets.Count : 3; 
            for (int x = 0; x < index; x++)
            {
                cardsForChoice.Add(targets[x]);
            }

            if (cardsForChoice.Count > 0)
            {

                CardPickerScreen screen = new CardPickerScreen(PickScreenMode.CHOICE, 1, cardsForChoice, true);
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
                CardManager.Instance.CardAppearToHand(screen.pickedCards[0].container.data, drawQueue, CardManager.Instance.deckAppearTransform.position);

                drawQueue.StartQueue();

                while (!drawQueue.resolved)
                {
                    yield return new WaitForEndOfFrame();
                }

                //Discard other cards
                for (int x = 0; x < cardsForChoice.Count; x++)
                {
                    EventQueue discardQueue = new EventQueue();

                    CardManager.Instance.CardAppear(discardQueue,cardsForChoice[i],CardManager.Instance.deckAppearTransform.position);
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

    protected int GetAmountToChoice()
    {
        int amountToChoice = 0;
        List<Effect> possibleTargets = new List<Effect>();

        var tempTargets = GetTargets().Select(t => t.effects);
        foreach (List<Effect> lEffets in tempTargets)
            possibleTargets.AddRange(lEffets);

        int ParameterTesting(Effect _effect, EffectValueType type, EffectValueOperator op = EffectValueOperator.None)
        {
            var allEffectValues = _effect.values;
            int counter = 0;

            foreach (EffectValue val in allEffectValues)
            {
                if (val.op == op && val.type == type)
                    counter++;
            }

            return counter;
        }

        switch (effectTypeToTarget.type)
        {
            case CustomEffectValueType.Buy:
                foreach (Effect _effect in possibleTargets)
                    amountToChoice += ParameterTesting(_effect, EffectValueType.Buy);
                break;

            case CustomEffectValueType.Bankrupt:
                foreach (Effect _effect in possibleTargets)
                    amountToChoice += ParameterTesting(_effect, EffectValueType.Bankrupt);
                break;

            case CustomEffectValueType.Discard:
                foreach (Effect _effect in possibleTargets)
                    amountToChoice += ParameterTesting(_effect, EffectValueType.Card, EffectValueOperator.Substraction);
                break;

            case CustomEffectValueType.Draw:
                foreach (Effect _effect in possibleTargets)
                    amountToChoice += ParameterTesting(_effect, EffectValueType.Card, EffectValueOperator.Addition);
                break;
        }

        return amountToChoice * choiceValue.value;
    }
}

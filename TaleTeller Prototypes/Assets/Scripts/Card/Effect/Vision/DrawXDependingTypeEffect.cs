using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawXDependingTypeEffect : Effect
{
    public EffectValue xParameter;
    public enum CardTypeParameter { None, Object, Character, Location };

    public CardTypeParameter cardTypeParameter;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(xParameter);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue drawQueue = new EventQueue();

        List<CardData> targets = GetTargets();

        int numberOfCardsToDraw = 0;

        if (cardTypeParameter == CardTypeParameter.None)
            for (int i = 0; i < targets.Count; i++)
            {
                if (linkedData != targets[i])
                    for (int x = 0; x < targets[i].effects.Count; x++)
                    {

                        for (int z = 0; z < targets[i].effects[x].values.Count; z++)
                        {
                            if (targets[i].effects[x].values[z].type == xParameter.type)
                            {
                                numberOfCardsToDraw += xParameter.value;
                            }
                        }
                    }
            }
        else
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (linkedData != targets[i])
                {
                    switch (cardTypeParameter)
                    {
                        case CardTypeParameter.Object:
                            if (targets[i].cardType is ObjectType)
                            {
                                numberOfCardsToDraw += xParameter.value;
                            }
                            break;

                        case CardTypeParameter.Character:
                            if (targets[i].cardType is CharacterType)
                            {
                                numberOfCardsToDraw += xParameter.value;
                            }
                            break;

                        case CardTypeParameter.Location:
                            if (targets[i].cardType is LocationType)
                            {
                                numberOfCardsToDraw += xParameter.value;
                            }
                            break;
                    }
                }

            }
        }



        //Logic that add methods to queue
        Debug.Log("Draw ?");
        CardManager.Instance.cardDeck.DrawCards(numberOfCardsToDraw, drawQueue);

        drawQueue.StartQueue();//Actual draw
        while (!drawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }



        currentQueue.UpdateQueue();
    }

}

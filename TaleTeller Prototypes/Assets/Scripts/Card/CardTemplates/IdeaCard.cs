using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Idea Card", menuName = "Data/IdeaCard")]
public class IdeaCard : CardData
{
    //This is how an idea card will be initialized (as long as a card is in the game loop (somewhere inside a list), it stays subscribed)
    public override CardData InitializeData(CardData data)
    {
        data = Instantiate(data);//make data an instance of itself

        //Instantiate other scriptables objects
        if (data.cardType != null)
        {
            data.cardType = Instantiate(data.cardType);
            data.cardType.InitType(data);//<--Watch out, subscribing to events can happen in here
        }


        for (int i = 0; i < data.effects.Count; i++)
        {
            if (effects[i] != null) data.effects[i] = Instantiate(data.effects[i]);
        }


        //Write logic to determine how the card subscribe to the events
        if (data.cardType == null)//All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            //Subscribe to onEnterEvent so it at least processes the events if any
            data.onEnterEvent += OnEnter;

            //Subscribe to OnEnd to Discard
            data.onEndEvent += OnEnd;
        }

        return data;
    }

}

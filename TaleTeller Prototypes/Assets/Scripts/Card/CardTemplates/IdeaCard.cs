using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Idea Card", menuName = "Data/IdeaCard")]
public class IdeaCard : CardData
{
    //This is how an idea card will be initialized (as long as a card is in the game loop (somewhere inside a list), it stays subscribed)
    public override CardData InitializeData(CardData data)//An idea card is basically the same as a base card
    {
        return base.InitializeData(data);
    }

}

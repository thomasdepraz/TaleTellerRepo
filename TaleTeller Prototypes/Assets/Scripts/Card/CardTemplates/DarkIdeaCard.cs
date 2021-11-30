using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dark Idea Card", menuName = "Data/DarkIdeaCard")]
public class DarkIdeaCard : CardData
{
    //For now a darkidea card basically works like an idea card
    public override CardData InitializeData(CardData data)
    {
        return base.InitializeData(data);
    }
}

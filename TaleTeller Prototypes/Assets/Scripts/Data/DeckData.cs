using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Base Deck", menuName = "Data/Deck")]
public class DeckData : ScriptableObject
{
    public List<CardData> deck = new List<CardData>();
}

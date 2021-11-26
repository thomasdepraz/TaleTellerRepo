using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public string effectName;
    public abstract void TriggerEffect(CardData target = null);
}

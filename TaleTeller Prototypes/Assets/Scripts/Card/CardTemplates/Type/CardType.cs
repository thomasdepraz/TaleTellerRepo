using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CardTypes : ScriptableObject
{
    public abstract void InitType(CardData card);

    public abstract void OnEnd(EventQueue queue);
}

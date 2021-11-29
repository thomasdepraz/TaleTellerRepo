using System.Collections;
using UnityEngine;

public enum Trigger
{
    None,
    OnEnter,
    OnStoryStart,
    OnStoryEnd,
    OnCardDeath
}

public enum Range
{
    None,
    Self,
    Left,
    Right
}

public enum EffectValueType
{

}

public enum EffectValueOperator
{
    Addition,
    Substraction,
    Product,
    Division
}
public struct EffectValue
{
    public float value;
    public EffectValueType type;
    public EffectValueOperator op;
}
public class Effect : ScriptableObject
{
    public string effectName;
    public Trigger trigger;
    public Range range;

    //All of these methods needs to be overwritten

    public virtual void InitEffect()
    {
        //switch case that subscribes OnTriggerEffect() to the right event based on the effect trigger

    }

    public virtual void OnTriggerEffect()
    {
        //ajouter la coroutine à la queue
    }

    public virtual IEnumerator EffectLogic()
    {
        //actual effect
        yield return null;
    }

    public virtual void ResetEffect()
    {
        //unsubscribe from every events


    }
}




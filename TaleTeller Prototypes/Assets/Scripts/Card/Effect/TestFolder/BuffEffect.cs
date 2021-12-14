using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : BonusEffect
{
    public EffectValue buffValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(buffValue);
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        Debug.Log("Buff Effect");
        yield return null;
        List<CardData> targets = GetTargets();

        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = 0; j < targets[i].effects.Count; j++)
            {
                for (int k = 0; k < targets[i].effects[j].values.Count; k++)
                {
                    //Filter
                    EffectValue currentValue = targets[i].effects[j].values[k];

                    if (currentValue.type == buffValue.type)//This is my filter this could be anything
                    {
                        if(currentValue.op == buffValue.op)
                            currentValue.value += buffValue.value;
                    }
                }
            }
        }

        currentQueue.UpdateQueue();
    }

}

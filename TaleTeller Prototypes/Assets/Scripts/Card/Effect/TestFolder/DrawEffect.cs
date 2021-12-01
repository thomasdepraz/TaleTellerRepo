using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : BonusEffect
{
    public override IEnumerator EffectLogic(EventQueue currentQueue)
    {
        //return base.EffectLogic();
        Debug.Log("DrawEffect");
        yield return null;
        currentQueue.UpdateQueue();
    }
}

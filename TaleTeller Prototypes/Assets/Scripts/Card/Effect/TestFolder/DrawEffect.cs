using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : BonusEffect
{
    public override IEnumerator EffectLogic()
    {
        //return base.EffectLogic();
        Debug.Log("DrawEffect");
        yield return null;
        CardManager.Instance.board.UpdateQueue();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddJunkToHandEffect : Effect
{
    public JunkCard junkCardToSpawn;

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue drawQueue = new EventQueue();

        CardManager.Instance.CardAppearToHand(junkCardToSpawn, drawQueue, CardManager.Instance.cardHand.GetPositionInHand(junkCardToSpawn));

        drawQueue.StartQueue();//Actual draw
        while (!drawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        currentQueue.UpdateQueue();
    }
}

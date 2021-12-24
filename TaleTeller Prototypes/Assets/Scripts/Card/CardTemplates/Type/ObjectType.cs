using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectType : CardTypes
{
    [HideInInspector] public CardData data;
    public override void InitType(CardData data)
    {
        this.data = data;

        //Init Effects
        data.InitializeCardEffects(data);
    }

    public override void OnEnd(EventQueue queue)
    {
        queue.events.Add(OnEndRoutine(queue));
    }
    private IEnumerator OnEndRoutine(EventQueue currentQueue)
    {
        EventQueue discardQueue = new EventQueue();

        CardManager.Instance.CardBoardToDiscard(data.currentContainer, discardQueue);

        discardQueue.StartQueue();

        while (!discardQueue.resolved)//Wait while the action has not ended
        {
            yield return new WaitForEndOfFrame();
        }

        //Unqueue
        currentQueue.UpdateQueue();
    }
}

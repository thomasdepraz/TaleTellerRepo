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

        //OnEnd it discards
        data.onEndEvent += OnObjectEnd;
    }

    private void OnObjectEnd()
    {
        CardManager.Instance.board.currentQueue.Add(OnEndRoutine());
    }
    private IEnumerator OnEndRoutine()
    {
        bool discardEnded = false;

        CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer, ref discardEnded);
        
        while (!discardEnded)//Wait while the action has not ended
        {
            yield return new WaitForEndOfFrame();
        }

        //Unqueue
        CardManager.Instance.board.UpdateQueue();
    }
}

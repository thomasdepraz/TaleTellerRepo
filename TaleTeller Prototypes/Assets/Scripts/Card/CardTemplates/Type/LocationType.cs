using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationType : CardTypes
{
    [HideInInspector] public CardData data;
    public Effect leftEffect;
    public Effect rightEffect;
    public override void InitType(CardData data)
    {
        this.data = data;

        //Init effects
        data.InitializeCardEffects(data);

        leftEffect = Instantiate(leftEffect);
        leftEffect.InitEffect(data);

        rightEffect = Instantiate(rightEffect);
        rightEffect.InitEffect(data);

        //onStart both effect happens
        data.onStartEvent += OnStart;

        //onEnd discard the card
        data.onEndEvent += OnLocationEnd;
    }

    #region OnEnd
    private void OnLocationEnd(EventQueue queue)
    {
        queue.events.Add(OnEndRoutine(queue));
    }
    private IEnumerator OnEndRoutine(EventQueue currentQueue)
    {
        bool temp = false;
        EventQueue discardQueue = new EventQueue();

        CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer, ref temp);//<--TODO implement add to event queue parameter

        discardQueue.StartQueue();

        while (!discardQueue.resolved)//Wait while the action has not ended
        {
            yield return new WaitForEndOfFrame();
        }

        //Unqueue
        currentQueue.UpdateQueue();
    }
    #endregion

    #region OnStart
    public void OnStart(EventQueue queue)
    {
        queue.events.Add(OnStartRoutine(queue));
    }

    IEnumerator OnStartRoutine(EventQueue currentQueue)
    {

        EventQueue effectQueue = new EventQueue();

        leftEffect.OnTriggerEffect(effectQueue);
        rightEffect.OnTriggerEffect(effectQueue);

        effectQueue.StartQueue();

        while(!effectQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }
    #endregion
}

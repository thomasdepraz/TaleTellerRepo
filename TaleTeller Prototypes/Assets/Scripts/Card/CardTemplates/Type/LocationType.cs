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
    private void OnLocationEnd()
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
    #endregion

    #region
    public void OnStart()
    {
        CardManager.Instance.board.currentQueue.Add(OnStartRoutine());
    }

    IEnumerator OnStartRoutine()
    {
        yield return null;
        leftEffect.OnTriggerEffect();
        rightEffect.OnTriggerEffect();

        CardManager.Instance.board.UpdateQueue();//This probably isn't consistent since this part is already part of the queue and the triggers add more events to the same queue
    }
    #endregion
}

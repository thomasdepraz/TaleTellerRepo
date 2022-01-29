using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Junk Card", menuName = "Data/JunkCard")]
public class JunkCard : CardData
{
    public PlotObjective objective;
    public int objectiveIndex;
    public override CardData InitializeData(CardData data)
    {
        JunkCard junk = Instantiate(data.dataReference) as JunkCard;
        junk.dataReference = data.dataReference;

        data.currentContainer = null;

        if(data.cardTypeReference != null)
        {
            CardTypes type = Instantiate(data.cardTypeReference);
            junk.cardType = type;
            junk.cardTypeReference = data.cardTypeReference;

            junk.cardType.InitType(junk);
        }
        else
        {
            InitializeCardEffects(junk);
        }

        junk.onStoryEnd += junk.OnEndJunk; //subscribe to basic discard behavior

        JunkCard reference = data as JunkCard;
        if (reference.objective != null)
        {
            junk.objective = reference.objective;
            junk.objective.linkedJunkedCards[objectiveIndex] = junk;
            junk.objectiveIndex = reference.objectiveIndex;

            if(junk.objective.GetType() == typeof(KillJunkObj))
            {
                var obj = (KillJunkObj)junk.objective;
                obj.SubscribeToJunkDeath(junk);
            }
        }

        return junk;
    }

    public void OnEndJunk(EventQueue queue)
    {
        queue.events.Add(OnEndJunkRoutine(queue));
    }
    IEnumerator OnEndJunkRoutine(EventQueue currentQueue)
    {
        if(currentContainer == null)
        {
            currentQueue.UpdateQueue();

            yield break;
        }

        if(cardType!=null && cardType.GetType()== typeof(CharacterType))
        {
            EventQueue returnToHand = new EventQueue();

            CardManager.Instance.CardBoardToHand(currentContainer, false, returnToHand);

            returnToHand.StartQueue();
            while(!returnToHand.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            EventQueue discardQueue = new EventQueue();

            CardManager.Instance.CardBoardToDiscard(currentContainer, discardQueue);

            discardQueue.StartQueue();
            while (!discardQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        currentQueue.UpdateQueue();
    }

    public void DestroyJunkCard(EventQueue queue)
    {
        queue.events.Add(DestroyJunkCardRoutine(queue));
    }
    IEnumerator DestroyJunkCardRoutine(EventQueue currentQueue) //LATER probably have this method in the card manager as DestroyCard(CardData data) {}
    {
        EventQueue destroyQueue = new EventQueue();

        CardManager.Instance.CardToOblivion(destroyQueue, this);

        destroyQueue.StartQueue();//<-- actual destroy happens here

        while (!destroyQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }
}

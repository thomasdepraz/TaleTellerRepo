using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public RectTransform handTransform;
    public List<CardContainer> hiddenHand = new List<CardContainer>();
    public List<CardContainer> currentHand = new List<CardContainer>();
    public int maxHandSize;

    public void InitCard(EventQueue queue, CardData data, bool deal = true)
    {
        queue.events.Add(InitCardRoutine(queue, data, deal));
    }
    public IEnumerator InitCardRoutine(EventQueue queue, CardData data, bool deal)
    {
        yield return null;
        if(data.currentContainer == null)
        {
            for (int i = 0; i < hiddenHand.Count; i++)
            {
                if (!hiddenHand[i].gameObject.activeSelf)
                {
                    currentHand.Add(hiddenHand[i]);
                    hiddenHand[i].InitializeContainer(data);
                    hiddenHand[i].rectTransform.SetParent(handTransform);

                    //Set parent and move
                    if (deal)
                    {
                        CardManager.Instance.cardTweening.MoveCard(hiddenHand[i], RandomPositionInRect(handTransform), true, true, queue);
                        yield return new WaitForSeconds(0.2f);
                    }
                    break;
                }
            }
        }
        else
        {
            if (deal)
            {
                data.currentContainer.rectTransform.SetParent(handTransform);
                CardManager.Instance.cardTweening.MoveCard(data.currentContainer, RandomPositionInRect(handTransform), false, false, queue);
                yield return new WaitForSeconds(0.2f);
            }
        }
        queue.UpdateQueue();
    }

    public void DiscardCardFromHand(CardContainer card, EventQueue queue)
    {
        queue.events.Add(DiscardCardFromHandRoutine(card, queue));
    }

    IEnumerator DiscardCardFromHandRoutine(CardContainer card, EventQueue queue)
    {
        #region Event OnCardDiscard
        EventQueue onCardDiscardQueue = new EventQueue();

        if (card.data.onCardDiscard != null)
            card.data.onCardDiscard(onCardDiscardQueue, card.data);

        onCardDiscardQueue.StartQueue();
        while (!onCardDiscardQueue.resolved)
        { yield return new WaitForEndOfFrame(); }
        #endregion

        #region Event OnAnyCardDiscard (Overload)
        EventQueue overloadQueue = new EventQueue();

        CardManager.Instance.board.CallBoardEvents("overload", overloadQueue);

        overloadQueue.StartQueue();
        while (!overloadQueue.resolved)
        { yield return new WaitForEndOfFrame(); }
        #endregion

        //Add feedback
        EventQueue feedback = new EventQueue();
        CardManager.Instance.cardTweening.MoveCard(card,CardManager.Instance.discardPileTransform.localPosition, true, false, feedback);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }

        card.data = card.data.ResetData(card.data);
        currentHand.Remove(card);
        CardManager.Instance.cardDeck.discardPile.Add(card.data);
        card.ResetContainer();

        queue.UpdateQueue();
    }

    public List<CardData> GetHandDataList()
    {
        List<CardData> result = new List<CardData>();
        for (int i = 0; i < currentHand.Count; i++)
        {
            if(result.GetType() != typeof(PlotCard))
                result.Add(currentHand[i].data);
        }
        return result;
    }

    #region Visuals

    public Vector3 RandomPositionInRect(RectTransform transform)
    {
        Vector3 randomPos;
        float height = transform.rect.height;
        float width = transform.rect.width;

        randomPos = new Vector3(Random.Range(-width/2, width/2), Random.Range(-height/4, height/4), 0);

        return randomPos;
    }
    #endregion
}

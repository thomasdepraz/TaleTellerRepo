using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public RectTransform handTransform;
    public RectTransform handTransformIdea;
    public RectTransform handTransformPlot;
    public List<CardContainer> hiddenHand = new List<CardContainer>();
    public List<CardContainer> currentHand = new List<CardContainer>();
    public int maxHandSize;

    public List<CardData> GetHandDataList()
    {
        List<CardData> result = new List<CardData>();
        for (int i = 0; i < currentHand.Count; i++)
        {
            if(currentHand[i].data.GetType() != typeof(PlotCard))
                result.Add(currentHand[i].data);
        }
        return result;
    }
    
    public bool IsInHand(CardContainer container)
    {
        Type cardType = container.data.GetType();
        if (cardType != typeof(PlotCard))
        {
            if (Contains(handTransformIdea, container.rectTransform.localPosition))
            {
                return true;
            }
        }
        else
        {
            if (Contains(handTransformPlot, container.rectTransform.localPosition)) 
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(RectTransform transform,Vector3 point)
    {
        float width = transform.rect.width / 2;
        float height = transform.rect.height / 2;
          if (point.x >= transform.localPosition.x - width && point.x <= transform.localPosition.x + width && point.y >= transform.localPosition.y - height && point.y <= transform.localPosition.y + height)
            return true;
        else return false;
    }

    public Vector3 RandomPositionInRect(RectTransform transform)
    {
        Vector3 randomPos;
        float height = transform.rect.height;
        float width = transform.rect.width;

        randomPos = transform.localPosition + new Vector3(UnityEngine.Random.Range(-width/2.3f, width/2.3f), UnityEngine.Random.Range(-height/5, height/5), 0);
        //randomPos = transform.localPosition;

        return randomPos;
    }

    public Vector3 GetPositionInHand(CardData data)
    {
        Vector3 position = Vector3.zero;
        Type cardType =  data.GetType();
        if(cardType != typeof(PlotCard))
        {
            position = RandomPositionInRect(handTransformIdea);
        }
        else
        {
            position = RandomPositionInRect(handTransformPlot);
        }

        return position;
    }

    public void ResetAllHand()
    {
        while(currentHand.Count > 0)
        {
            currentHand[0].ResetContainer();
        }
    }

    public int GetHandCount()
    {
        int count = 0;
        for (int i = 0; i < currentHand.Count; i++)
        {
            if (currentHand[i].data.GetType() != typeof(PlotCard))
                count++;
        }
        return count;
    }
}

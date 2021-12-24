using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public RectTransform handTransform;
    public List<CardContainer> hiddenHand = new List<CardContainer>();
    public List<CardContainer> currentHand = new List<CardContainer>();
    public int maxHandSize;


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

    public Vector3 RandomPositionInRect(RectTransform transform)
    {
        Vector3 randomPos;
        float height = transform.rect.height;
        float width = transform.rect.width;

        randomPos = new Vector3(Random.Range(-width/2, width/2), Random.Range(-height/4, height/4), 0);

        return randomPos;
    }
}

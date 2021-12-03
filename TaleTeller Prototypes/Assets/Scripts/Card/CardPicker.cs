using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPicker : MonoBehaviour
{
    public void Pick(EventQueue queue, List<CardContainer> pickedCards, int numberToPick, bool isInstantaneous)
    {
        queue.events.Add(PickRoutine(queue, pickedCards, numberToPick, isInstantaneous));
    }
    IEnumerator PickRoutine(EventQueue queue, List<CardContainer> pickedCards, int numberToPick, bool isInstantaneous)
    {
        yield return null;

        //Show UI
           


        //Wait for ending conditions


        queue.UpdateQueue();
    }
}

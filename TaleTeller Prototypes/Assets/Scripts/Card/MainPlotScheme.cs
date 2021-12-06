using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SchemeStep
{
    public List<CardData> stepOptions;
}

[CreateAssetMenu(fileName = "New Plot Scheme", menuName = "Data/Plot Scheme")]
public class MainPlotScheme : ScriptableObject
{
    public List<SchemeStep> schemeSteps = new List<SchemeStep>();
    [HideInInspector]public int currentStep;

    public void InitScheme(MainPlotScheme scheme)
    {
        scheme = Instantiate(scheme);
        scheme.currentStep = 0;

        for (int i = 0; i < schemeSteps.Count; i++)
        {
            for (int j = 0; j < schemeSteps[i].stepOptions.Count; j++)
            {
                schemeSteps[i].stepOptions[j].InitializeData(schemeSteps[i].stepOptions[j]);
            }
        }
    }

    public void LoadStep(EventQueue queue, MainPlotScheme scheme)
    {
        queue.events.Add(LoadStepRoutine(queue, scheme));
    }
    IEnumerator LoadStepRoutine(EventQueue queue, MainPlotScheme scheme)//TODO
    {
        yield return null;
        //if only one card in current step send it to the hand    
        if(scheme.schemeSteps[scheme.currentStep].stepOptions.Count == 1)
        {
            EventQueue toHandQueue = new EventQueue();

            //send card to hand
            PlotsManager.Instance.SendPlotToHand(toHandQueue, scheme.schemeSteps[scheme.currentStep].stepOptions[0]);

            toHandQueue.StartQueue();
            while(!toHandQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }

        }
        else if(scheme.schemeSteps[scheme.currentStep].stepOptions.Count == 1) //if multiple options make the player pick one of them and send it to the hand
        {
            List<CardData> pickedCard = new List<CardData>();
            EventQueue pickQueue = new EventQueue();

            CardManager.Instance.cardPicker.Pick(pickQueue, scheme.schemeSteps[scheme.currentStep].stepOptions ,pickedCard, 1, false);

            pickQueue.StartQueue();
            while(!pickQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        scheme.currentStep++;
        queue.UpdateQueue();
    }
}

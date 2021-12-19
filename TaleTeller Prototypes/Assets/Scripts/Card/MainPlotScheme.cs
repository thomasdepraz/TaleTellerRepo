using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SchemeStep
{
    public List<CardData> stepOptions;
    public string descriptionChapterChoice;
    public string descriptionChapterChoice1;
    public string descriptionChapterChoice2;
}

[CreateAssetMenu(fileName = "New Plot Scheme", menuName = "Data/Plot Scheme")]
public class MainPlotScheme : ScriptableObject
{
    public List<SchemeStep> schemeSteps = new List<SchemeStep>();
    [HideInInspector]public int currentStep;

    public MainPlotScheme InitScheme(MainPlotScheme _scheme)
    {
        MainPlotScheme scheme = Instantiate(_scheme);
        scheme.currentStep = 0;

        for (int i = 0; i < scheme.schemeSteps.Count; i++)
        {
            for (int j = 0; j < scheme.schemeSteps[i].stepOptions.Count; j++)
            {
                scheme.schemeSteps[i].stepOptions[j] = scheme.schemeSteps[i].stepOptions[j].InitializeData(scheme.schemeSteps[i].stepOptions[j]);
            }
        }
        return scheme;
    }

    public void LoadStep(EventQueue queue, MainPlotScheme scheme)
    {
        queue.events.Add(LoadStepRoutine(queue, scheme));
    }
    IEnumerator LoadStepRoutine(EventQueue queue, MainPlotScheme scheme)//TODO
    {
        yield return null;
        var optionList = scheme.schemeSteps[scheme.currentStep].stepOptions;

        List<CardData> pickedCard = new List<CardData>();
        EventQueue pickQueue = new EventQueue();

        CardManager.Instance.cardPicker.Pick(pickQueue, optionList, pickedCard, 1, false, "Choose how your plot goes on", scheme.schemeSteps[scheme.currentStep].descriptionChapterChoice, scheme.schemeSteps[scheme.currentStep].descriptionChapterChoice1, scheme.schemeSteps[scheme.currentStep].descriptionChapterChoice2);

        pickQueue.StartQueue();
        while (!pickQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        EventQueue toHandQueue = new EventQueue();

        //send card to hand
        //PlotsManager.Instance.SendPlotToHand(toHandQueue, pickedCard[0]);
        PlotsManager.Instance.currentPickedCard = pickedCard[0];
        pickedCard[0].onCardAppear(toHandQueue, pickedCard[0]);

        toHandQueue.StartQueue();
        while (!toHandQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        /*Old System
        //if only one card in current step send it to the hand    
        if (optionList.Count == 1)
        {
            EventQueue toHandQueue = new EventQueue();

            //send card to hand
            //PlotsManager.Instance.SendPlotToHand(toHandQueue, scheme.schemeSteps[scheme.currentStep].stepOptions[0]);
            PlotsManager.Instance.currentPickedCard = optionList[0];
            optionList[0].onCardAppear(toHandQueue, optionList[0]);


            toHandQueue.StartQueue();
            while(!toHandQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }

        }
        else if(optionList.Count > 1) //if multiple options make the player pick one of them and send it to the hand
        {
            List<CardData> pickedCard = new List<CardData>();
            EventQueue pickQueue = new EventQueue();

            CardManager.Instance.cardPicker.Pick(pickQueue, optionList, pickedCard, 1, false, "Choose how your plot goes on");

            pickQueue.StartQueue();
            while(!pickQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }

            EventQueue toHandQueue = new EventQueue();

            //send card to hand
            //PlotsManager.Instance.SendPlotToHand(toHandQueue, pickedCard[0]);
            PlotsManager.Instance.currentPickedCard = pickedCard[0];
            pickedCard[0].onCardAppear(toHandQueue, pickedCard[0]);

            toHandQueue.StartQueue();
            while (!toHandQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }

        }*/

        scheme.currentStep++;
        queue.UpdateQueue();
    }


    public void UpdateScheme(EventQueue queue, MainPlotScheme scheme)
    {
        LoadStep(queue, scheme);
    }
}

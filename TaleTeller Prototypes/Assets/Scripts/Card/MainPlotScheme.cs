using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SchemeStep
{
    public List<CardData> stepOptions;
    public string chapterDescription;
}

[CreateAssetMenu(fileName = "New Plot Scheme", menuName = "Data/Plot Scheme")]
public class MainPlotScheme : ScriptableObject
{
    public List<SchemeStep> schemeSteps = new List<SchemeStep>();
    [HideInInspector]public int currentStep;

    public string plotName;
    public string plotDescription;
    public string plotEndDescription;
    public Sprite plotIllustration;
 
    public MainPlotScheme InitScheme(MainPlotScheme _scheme)
    {
        MainPlotScheme scheme = Instantiate(_scheme);
        scheme.plotDescription = LocalizationManager.Instance.GetString(LocalizationManager.Instance.schemesDescriptionsDictionary, scheme.plotDescription);

        scheme.currentStep = 0;

        for (int i = 0; i < scheme.schemeSteps.Count; i++)
        {
            //Init description
            scheme.schemeSteps[i].chapterDescription = LocalizationManager.Instance.GetString(LocalizationManager.Instance.schemesDescriptionsDictionary, scheme.schemeSteps[i].chapterDescription);

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
    IEnumerator LoadStepRoutine(EventQueue queue, MainPlotScheme scheme)
    {
        SchemeStep schemeStep = scheme.schemeSteps[scheme.currentStep];

        ChapterScreen chapterScreen = new ChapterScreen(schemeStep);
        bool wait = true;
        chapterScreen.Open(() => { wait = false; });
        while (wait) { yield return new WaitForEndOfFrame(); }

        while(chapterScreen.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        chapterScreen.Close(() => { wait = false; });
        while (wait) { yield return new WaitForEndOfFrame(); }

        //send card to hand
        EventQueue toHandQueue = new EventQueue();

        CardData card = chapterScreen.chosenCard.data.InitializeData(chapterScreen.chosenCard.data.dataReference);
        card.onCardAppear(toHandQueue, card);

        toHandQueue.StartQueue();
        while (!toHandQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        scheme.currentStep++;
        queue.UpdateQueue();
    }


    public void UpdateScheme(EventQueue queue, MainPlotScheme scheme)
    {
        LoadStep(queue, scheme);
    }
}

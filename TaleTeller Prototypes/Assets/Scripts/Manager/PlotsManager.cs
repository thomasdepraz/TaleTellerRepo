using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SchemePool
{
    public List<MainPlotScheme> schemes = new List<MainPlotScheme>();
    public void InitSchemes()
    {
        for (int i = 0; i < schemes.Count; i++)
        {
            schemes[i] = schemes[i].InitScheme(schemes[i]);
        }
    }
}
public class PlotsManager : Singleton<PlotsManager>
{
    public MainPlotScheme currentMainPlotScheme;
    public List<SchemePool> schemePools = new List<SchemePool>();
    public List<CardData> secondaryPlots = new List<CardData>();
    public List<CardData> darkIdeas = new List<CardData>();

    [HideInInspector] public CardData currentPickedCard;

    private void Awake()
    {
        CreateSingleton();
    }

    public void Start()
    {
        //Init schemes
        for (int i = 0; i < schemePools.Count; i++)
        {
            schemePools[i].InitSchemes();
        }
        //InitData
        for (int i = 0; i < secondaryPlots.Count; i++)
        {
            secondaryPlots[i] = secondaryPlots[i].InitializeData(secondaryPlots[i]); 
        }
        //for (int i = 0; i < darkIdeas.Count; i++)
        //{
        //    darkIdeas[i] = darkIdeas[i].InitializeData(darkIdeas[i]);

        //}
    }

    //Link this method to the act beggining
    public void ChooseMainPlot(EventQueue queue, List<MainPlotScheme> schemesToChooseFrom)
    {
        queue.events.Add(ChooseMainPlotRoutine(queue, schemesToChooseFrom));
    }
    IEnumerator ChooseMainPlotRoutine(EventQueue queue, List<MainPlotScheme> schemes)
    {
        ChapterScreen chapterScreen = new ChapterScreen(schemes);

        bool wait = true;
        chapterScreen.Open(() => { wait = false; });
        while (wait) { yield return new WaitForEndOfFrame(); }

        while (chapterScreen.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        chapterScreen.Close(() => { wait = false; });
        while (wait) { yield return new WaitForEndOfFrame(); }

        //Load MainScheme
        EventQueue loadQueue = new EventQueue();

        currentMainPlotScheme = chapterScreen.chosenScheme;

        currentMainPlotScheme.LoadStep(loadQueue, currentMainPlotScheme);

        loadQueue.StartQueue();

        while (!loadQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }


    //Link this method to the event occuring every 2-3 turns TODO
    public void ChooseSecondaryPlots(EventQueue queue)
    {
        queue.events.Add(ChooseSecondaryPlotsRoutine(queue));
    }
    IEnumerator ChooseSecondaryPlotsRoutine(EventQueue queue)
    {
        yield return null;
    //    EventQueue pickQueue = new EventQueue();
    //    List<CardData> pickedCards = new List<CardData>();

    //    string instruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseSecondayPlotInstruction);
    //    CardManager.Instance.cardPicker.Pick(pickQueue, secondaryPlots, pickedCards, 1, instruction);

    //    pickQueue.StartQueue();

    //    while(!pickQueue.resolved)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }

    //    EventQueue appearQueue = new EventQueue();

    //    //Send the picked card to hand and init 
    //    if(pickedCards.Count != 0)
    //    {
    //        for (int i = 0; i < pickedCards.Count; i++)
    //        {
    //            currentPickedCard = pickedCards[i]; //<-- This doesn't work if more than one pickedCard TODO fix this
    //            pickedCards[i].onCardAppear(appearQueue, pickedCards[i]); //This manages the appear animation + all the junk apparition
    //            secondaryPlots.Remove(pickedCards[i]); //TEMP  
    //        }

    //    }
    //    else//If the picked card is null send a random plot card to deck
    //    {
    //        int r = Random.Range(0, secondaryPlots.Count - 1);

    //        PlotCard card = secondaryPlots[r] as PlotCard;
    //        card.onCardAppear -= card.OnPlotAppear; //Unsubscribe from the onAppear event since it wont gbe useful later

    //        card.onCardDraw += card.OnPlotAppear;//Subscribe to the onDraw event to spawn correctly the junk cards;

    //        //animate card to deck
    //        //for now only add it to deck list
    //        //SendPlotToDeck(appearQueue, card);
    //        CardManager.Instance.CardAppearToDeck(card, appearQueue, CardManager.Instance.plotAppearTransform.position);
    //    }


    //    appearQueue.StartQueue();

    //    while(!appearQueue.resolved)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }

    //    queue.UpdateQueue();
    }
}

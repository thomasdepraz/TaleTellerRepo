using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPicker : MonoBehaviour
{
    [Header("References")]
    public List<CardContainer> cardPlaceholders = new List<CardContainer>();
    public List<SchemeDescription> schemeDescriptions = new List<SchemeDescription>();
    public Image backgroundPanel;
    public Button confirmButton;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI descriptionText;


    [Header("Data")]
    public float backgroundFadeSpeed;

    List<CardData> selectedCards = new List<CardData>();
    CardContainer lastSelectedContainer;
    int numberToSelect;
    bool confirmed;

    SchemeDescription selectedSchemeDescription;

    public void Start()
    {
        InitEventCallbacks(cardPlaceholders);

        List<CardContainer> schemeDescriptionContainers = new List<CardContainer>();
        for (int i = 0; i < schemeDescriptions.Count; i++)
        {
            schemeDescriptionContainers.Add(schemeDescriptions[i].cardContainer);
        }
        InitEventCallbacks(schemeDescriptionContainers);
    }

    public void Pick(EventQueue queue, List<CardData> targetCards,List<CardData> pickedCards, int numberToPick, bool isInstantaneous, string instruction)
    {
        queue.events.Add(PickRoutine(queue, targetCards, pickedCards, numberToPick, isInstantaneous, instruction)) ;
    }
    IEnumerator PickRoutine(EventQueue queue, List<CardData> targetCards, List<CardData> pickedCards, int numberToPick, bool isInstantaneous, string instruction)
    {
        canvasGroup.blocksRaycasts = true;
        selectedCards = pickedCards;

        if (numberToPick <= targetCards.Count) numberToSelect = numberToPick;
        else numberToSelect = targetCards.Count;

        confirmButton.interactable = false;

        if (!isInstantaneous) confirmed = false;
        else confirmed = true;

        //Fade in background
        bool fadeEnded = false;
        LeanTween.color(gameObject, Color.black, backgroundFadeSpeed).setOnUpdate((Color col) => { backgroundPanel.color = col; }).setOnComplete(onEnd => { fadeEnded = true; }) ;
        while(!fadeEnded)
        {
            yield return new WaitForEndOfFrame();
        }
        fadeEnded = false;

        InitializePlaceholders(targetCards);
        instructionText.gameObject.SetActive(true);
        instructionText.text = instruction;

        //TODO fade in cards



        //If necessary show button
        if (!isInstantaneous)
        {
            confirmButton.gameObject.SetActive(true);
        }
        //Wait for ending conditions
        while(selectedCards.Count != numberToPick || !confirmed)
        {
            yield return new WaitForEndOfFrame();
        }

        ResetPlaceHolders();
        instructionText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);

        Color transparent = new Color(0, 0, 0, 0);
        LeanTween.color(gameObject, transparent, backgroundFadeSpeed).setOnUpdate((Color col) => { backgroundPanel.color = col; }).setOnComplete( 
            onEnd => { canvasGroup.blocksRaycasts = false; queue.UpdateQueue(); });

        
    }

    public void PickScheme(EventQueue queue, List<MainPlotScheme> schemes, List<MainPlotScheme> chosenScheme, bool cardVersion = false)
    {
        queue.events.Add(PickSchemeRoutine(queue, schemes, chosenScheme, cardVersion));
    }
    IEnumerator PickSchemeRoutine(EventQueue queue, List<MainPlotScheme> schemes, List<MainPlotScheme> chosenScheme, bool cardVersion)
    {
        yield return null;
        canvasGroup.blocksRaycasts = true;
        confirmButton.interactable = false;
        confirmed = false;

        selectedCards.Clear();

        //Appear background
        backgroundPanel.rectTransform.anchorMax = new Vector2(1,0);
        backgroundPanel.color = Color.black;

        bool ended = false;
        LeanTween.value(0,1, 0.5f).setEaseOutQuint().setOnUpdate(value => { backgroundPanel.rectTransform.anchorMax = new Vector2(1, value); }).
            setOnComplete(end => {ended = true; });
        while(!ended){yield return new WaitForEndOfFrame();}
        ended = false;

        //Load Instruction
        string instruction = string.Empty; 
        if(cardVersion)
            instruction = GameManager.Instance.instructionsData.chooseSchemeStepInstruction;
        else
            instruction = GameManager.Instance.instructionsData.chooseSchemeInstruction;

        instructionText.text = instruction;
        instructionText.gameObject.SetActive(true); //TODO Tween the text;

        //Load image and description or cards
        if(cardVersion)
        {
            numberToSelect = 1;
            int step = PlotsManager.Instance.currentMainPlotScheme.currentStep;
            List<CardData> stepOptions =  PlotsManager.Instance.currentMainPlotScheme.schemeSteps[step].stepOptions;

            //Appear previous completion text
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = PlotsManager.Instance.currentMainPlotScheme.schemeSteps[step].descriptionChapterChoice;


            for (int i = 0; i < stepOptions.Count; i++)//Load containers TODO implement queueing for twening feedback
            {
                PlotCard card = stepOptions[i] as PlotCard;
                schemeDescriptions[i].cardContainer.InitializeContainer(stepOptions[i], true);
                schemeDescriptions[i].description.text = card.plotChoiceDescription;//Temp

                schemeDescriptions[i].LoadCardContainer();
            }

        }
        else
        {
            for (int i = 0; i < schemes.Count; i++)//TODO Tween these objects
            {
                schemeDescriptions[i].description.text = schemes[i].plotDescription;
                schemeDescriptions[i].illustration.sprite = schemes[i].plotIllustration;
                schemeDescriptions[i].linkedScheme = schemes[i];

                schemeDescriptions[i].LoadIllustration();
            }
        }

        //Appear button
        confirmButton.gameObject.SetActive(true);

        //wait until the card are picked
        while(!confirmed)
        {
            yield return new WaitForEndOfFrame();
        }

        if(cardVersion)
        {
            PlotsManager.Instance.currentPickedCard = selectedCards[0];
        }
        else
        {
            chosenScheme.Add(selectedSchemeDescription.linkedScheme);
        }

        //Deactivate placeholders
        confirmButton.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        for (int i = 0; i < schemeDescriptions.Count; i++)
        {
            schemeDescriptions[i].description.text = string.Empty;
            schemeDescriptions[i].illustration.sprite = null;
            schemeDescriptions[i].linkedScheme = null;
            schemeDescriptions[i].cardContainer.ResetContainer(true);
            Deselect(schemeDescriptions[i].cardContainer);
            schemeDescriptions[i].gameObject.SetActive(false);
        }
       
        LeanTween.value(backgroundPanel.gameObject, 1, 0, 0.5f).setOnUpdate(value => { backgroundPanel.rectTransform.anchorMax = new Vector2(1,value); }).setOnComplete(
            onEnd => {backgroundPanel.rectTransform.anchorMax =  Vector2.one ; canvasGroup.blocksRaycasts = false; backgroundPanel.color = Color.clear; queue.UpdateQueue(); });
    }


    #region Card Picking Utility
    void InitializePlaceholders(List<CardData> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = 0; j < cardPlaceholders.Count; j++)
            {
                if(cardPlaceholders[i].data == null)
                {
                    cardPlaceholders[i].selfImage.color = Color.white;
                    cardPlaceholders[i].gameObject.SetActive(true);
                    cardPlaceholders[i].InitializeContainer(targets[i],true);
                    break;
                }
            }
        }
    }

    public void ResetPlaceHolders()
    {
        for (int i = 0; i < cardPlaceholders.Count; i++)
        {
            if(cardPlaceholders[i].data != null)
            {
                cardPlaceholders[i].ResetContainer(true);
            }
        }
    }

    void InitEventCallbacks(List<CardContainer> container)
    {
        for (int i = 0; i < container.Count; i++)
        {
            EventTrigger trigger = container[i].gameObject.GetComponent<EventTrigger>();

            int j = i;
            EventTrigger.Entry clickEvent = new EventTrigger.Entry();
            clickEvent.eventID = EventTriggerType.PointerClick;
            clickEvent.callback.AddListener(data => { SelectCard(container[j]); });
            trigger.triggers.Add(clickEvent);

            EventTrigger.Entry enterEvent = new EventTrigger.Entry();
            enterEvent.eventID = EventTriggerType.PointerEnter;
            enterEvent.callback.AddListener(data => { PointerEnter(container[j]); });
            trigger.triggers.Add(enterEvent);

            EventTrigger.Entry exitEvent = new EventTrigger.Entry();
            exitEvent.eventID = EventTriggerType.PointerExit;
            exitEvent.callback.AddListener(data => { PointerExit(container[j]); });
            trigger.triggers.Add(exitEvent);
        }
    }

    public void SelectCard(CardContainer container)
    {
        if(!selectedCards.Contains(container.data))//Not selected
        {
            if (selectedCards.Count == numberToSelect)
                Deselect(lastSelectedContainer);

            Select(container);
        }
        else//already selected
        {
            Deselect(container);
        }
    }
    public void PointerEnter(CardContainer container)
    {
        //Tween
        LeanTween.rotate(container.gameObject, Vector3.zero, 0.2f).setEaseInOutCubic();
        LeanTween.scale(container.gameObject, Vector3.one * 1.3f, 0.1f).setEaseInOutCubic(); 
    }
    public void PointerExit(CardContainer container)
    {
        //Tween
        LeanTween.scale(container.gameObject, Vector3.one, 0.1f).setEaseInOutCubic();
    }

    void Select(CardContainer container)
    {
        lastSelectedContainer = container;

        //Show selected shader
        container.selfImage.color = Color.green;

        //Add to list 
        selectedCards.Add(container.data);

        //if valid show button
        if (isValid()) confirmButton.interactable = true;
    }
    void Deselect(CardContainer container)
    {
        //Show selected shader
        container.selfImage.color = Color.white;

        //Add to list 
        selectedCards.Remove(container.data);

        //if valid show button
        if (!isValid()) confirmButton.interactable = false;
    }

    bool isValid()
    {
        return selectedCards.Count == numberToSelect;
    }
    public void OnButtonClick()
    {
        confirmed = true;
        confirmButton.gameObject.SetActive(false);
    }

    public string BuildInstruction(string baseString, int value = 0)
    {
        if (baseString.Contains("$value$"))
        {
            baseString.Replace("$value$", value.ToString());
        }
        return baseString;
    }
    #endregion


    #region Scheme picking utility
    public void SelectIllustration(SchemeDescription schemeDescription)
    {
        if(selectedSchemeDescription != null)
        {
            if(schemeDescription != selectedSchemeDescription)
            {
                DeselectIllu(selectedSchemeDescription);
                SelectIllu(schemeDescription);

                confirmButton.interactable = true;
            }
            else
            {
                DeselectIllu(schemeDescription);
                confirmButton.interactable = false;
            }
        }
        else
        {
            SelectIllu(schemeDescription);

            confirmButton.interactable = true;
        }
    }

    public void PointerEnterIllustration(SchemeDescription schemeDescription)
    {
        //Tweening shit happens
    }

    public void PointerExitIllustration(SchemeDescription schemeDescription)
    {
        //Tweening shit happens again
    }

    void SelectIllu(SchemeDescription schemeDescription)
    {
        //Feedback
        schemeDescription.illustration.color = Color.green;

        //
        selectedSchemeDescription = schemeDescription;
    }

    void DeselectIllu(SchemeDescription schemeDescription)
    {
        //Feedback 
        schemeDescription.illustration.color = Color.white;

        //
        selectedSchemeDescription = null;
    }
    #endregion 

}

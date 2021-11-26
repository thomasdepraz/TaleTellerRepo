using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Fight
{
    public CardData fighterA;
    public CardData fighterB;

    public Fight(CardData fighterA, CardData fighterB)
    {
        this.fighterA = fighterA;
        this.fighterB = fighterB;
    }
}
public struct CardToInit {
    public CardData card;
    public int index;

    public CardToInit(CardData card, int index)
    {
        this.card = card;
        this.index = index;    }
}

public class StoryManager : MonoBehaviour
{
    [Header("Stats")]
    [HideInInspector] public int chapterCount;
    [HideInInspector] public List<CardToInit> cardsToInit = new List<CardToInit>();
    public List<List<CardData>> steps = new List<List<CardData>>();
    public float heroMovingSpeed;
    [HideInInspector] public bool isReadingStory;


    [Header("References")]
    public List<RectTransform> stepsUI = new List<RectTransform>();
    public List<StoryStep> stepsContainers = new List<StoryStep>();
    public RectTransform heroUITransform;
    public Image heroGraph;

    //Private variables
    private int currentStepIndex = 0;
    private int currentStepEventListIndex = 0;
    private Hero currentHero;


    private void Start()
    {
        currentHero = GameManager.Instance.currentHero;
        Debug.LogError("Opening the console");
        steps = new List<List<CardData>>();
        //Init list
        for (int i = 0; i < 4; i++)
        {
            steps.Add(new List<CardData>());
        }
    }

    public void StartNewChapter()
    {
        isReadingStory = false;

        //Reset keyCards
        List<CardData> clearList = new List<CardData>();
        for (int i = 0; i < steps.Count; i++)
        {
            for (int j = 0; j < steps[i].Count; j++)
            {
                if(steps[i][j].isKeyCard)
                {
                    steps[i][j].feedback.UnloadCardFeedback(steps[i][j]);//Unload graph
                    CardManager.Instance.cardHand.InitCard(steps[i][j], false);
                    clearList.Add(steps[i][j]);
                }
            }

            for (int j = 0; j < clearList.Count; j++)
            {
                steps[i].Remove(clearList[j]);
            }
            clearList.Clear();
        }

        //Reset player position
        heroUITransform.SetParent(stepsContainers[0].self);
        heroUITransform.SetAsLastSibling();

        /*//Clear lists
        for (int i = 0; i < steps.Count; i++)
        {
            steps[i].Clear();
        }*/


        //Clear player temporary values
        GameManager.Instance.currentHero.bonusDamage = 0;

        //Check for keyCards to update
        CardManager.Instance.cardHand.UpdateKeyCardStatus();

        //Deal Cards
        CardManager.Instance.cardDeck.DealCards(CardManager.Instance.cardHand.maxHandSize - CardManager.Instance.cardHand.currentHand.Count);

        //Give creativity
        GameManager.Instance.creativityManager.creativity += 5;
    }
    IEnumerator InitCards()
    {
        if(cardsToInit.Count > 0)
        {
            CardData card = cardsToInit[0].card;
            int stepIndex = cardsToInit[0].index;

            cardsToInit.RemoveAt(0);//Remove the card to prevent stack overflow

            if(card.keyCardActivated)
            {
                card.currentInterestCooldown = card.interestCooldown;//reset interest cooldown if needed
            }
            if(card.isKeyCard && !card.keyCardActivated)
            {
                card.keyCardActivated = true;//Activate Key Card
            }

            //Init card graphics
            for (int i = 0; i < stepsContainers[stepIndex + 1].cardFeedbackContainers.Count; i++)
            {
                if(!stepsContainers[stepIndex + 1].cardFeedbackContainers[i].gameObject.activeSelf)
                {
                    stepsContainers[stepIndex + 1].cardFeedbackContainers[i].InitCardFeedback(card);
                    break;
                }
            }

            steps[stepIndex].Add(card);//Just add card an let it resolve later

            yield return StartCoroutine(InitCards());
        }
        else
        {
            //Plus de cartes à init --> start story
            print("The card ar initiated, resolve can proceed");
            StartCoroutine(ResolveSteps(0));
        }
    }
    IEnumerator ResolveSteps(int stepIndex)
    {


        if(stepIndex < 4)
        {
            //Make a fight list between characters
            #region Fight MatchMaking
            List<CardData> stepCharacters = new List<CardData>();
            for (int index = 0; index < steps[stepIndex].Count; index++)
            {
                if (steps[stepIndex][index].type == CardType.Character)
                {
                    stepCharacters.Add(steps[stepIndex][index]);
                }
            }

            List<Fight> fights = new List<Fight>();
            for (int index = 0; index < stepCharacters.Count; index++)
            {
                for (int i = index; i < stepCharacters.Count; i++)
                {
                    if (stepCharacters[index].characterBehaviour != stepCharacters[i].characterBehaviour)//if they have opposed beahaviour boom boom
                    {
                        fights.Add(new Fight(stepCharacters[index], stepCharacters[i]));//Add fight to queue
                    }
                }
            }
            #endregion

            //Apply objects effects
            List<CardData> stepObjects = new List<CardData>();
            for (int i = 0; i < steps[stepIndex].Count; i++)
            {
                if(steps[stepIndex][i].type == CardType.Object)
                {
                    stepObjects.Add(steps[stepIndex][i]);
                }
            }

            for (int i = 0; i < stepObjects.Count; i++)
            {
                for (int j = 0; j < steps[stepIndex].Count; j++)
                {
                    if(steps[stepIndex][j].type == CardType.Character)
                    {
                        stepObjects[i].effect.TriggerEffect(steps[stepIndex][j]);

                        CardManager.Instance.cardDeck.discardPile.Add(stepObjects[i]);

                        stepObjects[i].feedback.UnloadCardFeedback(stepObjects[i]);

                        steps[stepIndex].Remove(stepObjects[i]);

                        stepObjects.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            //Apply location effects

            //Resolve fights
            if (fights.Count > 0)
            {
                yield return StartCoroutine(Fight(fights, stepIndex));
            }
            else
            { 
                yield return StartCoroutine(ResolveSteps(stepIndex + 1));
            }
        }
        else
        {
            print("The story can begin");
            yield return StartCoroutine(ReadStory());//Read
        }
    }
    public IEnumerator Fight(List<Fight> fights, int stepIndex)
    {
        if(fights.Count > 0)
        {
            CardData fighterA = fights[0].fighterA;
            CardData fighterB = fights[0].fighterB;

            if (fighterA.characterStats.baseLifePoints > 0 && fighterB.characterStats.baseLifePoints > 0)//if both fighters are alive they can fight
            {
                print($"Fight between {fighterA.cardName} and {fighterB.cardName} at step {stepIndex + 1}");

                yield return new WaitForSeconds(1);
                fighterB.characterStats.baseLifePoints -= fighterA.characterStats.baseAttackDamage;
                fighterB?.feedback.UpdateText(fighterB);

                yield return new WaitForSeconds(1);
                if (fighterB.characterStats.baseLifePoints > 0)//if alive hit back
                {
                    fighterA.characterStats.baseLifePoints -= fighterA.characterStats.baseAttackDamage;
                    fighterA?.feedback.UpdateText(fighterA);
                }

                if (fighterB.characterStats.baseLifePoints <= 0)//Check if dead then reset card
                {
                    //reset card and remove from story line
                    print($"{fighterB.cardName} is dead, returning the card to discard pile");
                    fighterB.feedback.UnloadCardFeedback(fighterB);
                    fighterB.ResetCharacterStats();
                    steps[stepIndex].Remove(fighterB);

                    if (!fighterB.isKeyCard) //HARDCODED
                        CardManager.Instance.cardDeck.discardPile.Add(fighterB);

                    //Trigger on death event if need be
                    if (fighterB.trigger == CardEventTrigger.OnDeath)
                        fighterB.effect.TriggerEffect();
                }

                if (fighterA.characterStats.baseLifePoints <= 0)//Check if dead then reset card
                {
                    //reset card and remove from story line
                    print($"{fighterA.cardName} is dead, returning the card to discard pile");
                    fighterA.feedback.UnloadCardFeedback(fighterA);
                    fighterA.ResetCharacterStats();
                    steps[stepIndex].Remove(fighterA);

                    if(!fighterA.isKeyCard) //HARDCODED
                        CardManager.Instance.cardDeck.discardPile.Add(fighterA);

                    //Trigger on death event if need be
                    if (fighterA.trigger == CardEventTrigger.OnDeath)
                        fighterA.effect.TriggerEffect();
                }
            }

            fights.RemoveAt(0);
            yield return StartCoroutine(Fight(fights, stepIndex));   
        }
        else
        {
            //Keep init cards going
            StartCoroutine(ResolveSteps(stepIndex + 1));
        }
    }
    public void StartStory()
    {
        isReadingStory = true;
        StartCoroutine(InitCards());
    }
    public IEnumerator ReadStory()
    {
        //Visually move the player
        Debug.LogError($"Moving to step {currentStepIndex + 1}");

        heroUITransform.SetParent(stepsContainers[currentStepIndex + 1].self);
        heroUITransform.SetAsLastSibling();
        
        //Make the hero go through every events and trigger enter and exit on every event
        yield return new WaitForSeconds(0.5f);
        if (steps[currentStepIndex].Count > 0)
        {
            List<CardData> clearList = new List<CardData>();

            for (int i = 0; i < steps[currentStepIndex].Count; i++)
            {
                currentStepEventListIndex++;
                CardData currentCard = steps[currentStepIndex][i];

                //If needs be, trigger event
                if (currentCard.trigger == CardEventTrigger.OnEncounter)
                    currentCard.effect.TriggerEffect();

                switch (currentCard.type)
                {
                    case CardType.Character:
                        if(currentCard.characterBehaviour == CharacterBehaviour.Agressive)
                        {
                            //Fight
                            currentCard.characterStats.baseLifePoints -= (currentHero.attackDamage + currentHero.bonusDamage);
                            currentCard?.feedback.UpdateText(currentCard);

                            yield return new WaitForSeconds(1f);//wait to show feedback

                            if (currentCard.characterStats.baseLifePoints > 0)
                                currentHero.lifePoints -= currentCard.characterStats.baseAttackDamage;
                        }
                        if (currentCard.characterStats.baseLifePoints <= 0)
                        {
                            //End the fight and keep going 
                            //steps[currentStepIndex].Remove(currentCard); CANT REMOVE HERE
                            CardManager.Instance.cardDeck.discardPile.Add(currentCard);

                            currentCard.feedback.UnloadCardFeedback(currentCard);
                            currentCard.ResetCharacterStats();

                            //Trigger on death event if need be
                            if (currentCard.trigger == CardEventTrigger.OnDeath)
                                currentCard.effect.TriggerEffect();

                            clearList.Add(currentCard);

                        }
                        if(currentHero.lifePoints <= 0)
                        {
                            //make a new Hero
                            Debug.LogError("player dead, reviving hero");
                            currentHero.ReviveHero();
                        }
                        yield return new WaitForSeconds(1f);
                        break;

                    case CardType.Object:
                        //Trigger object effect on player
                        currentCard.effect.TriggerEffect();

                        currentCard?.feedback.UnloadCardFeedback(currentCard);

                        //Put card in discard
                        CardManager.Instance.cardDeck.discardPile.Add(currentCard);

                        //Add to a clear list
                        clearList.Add(currentCard);

                        break;
                    case CardType.Location:

                        break;

                    default:
                        break;
                }

                //HARDCODE VICTORY
                if (currentCard.isKeyCard && currentCard.cardName == "Vice The Assassin")
                {
                    if (currentCard.characterStats.baseLifePoints <= 0)//Hardcoded key card character death
                    {
                        print($"{currentCard.cardName} is dead, YOU WIN");
                    }
                }
            }

            for (int i = 0; i < clearList.Count; i++)
            {
                steps[currentStepIndex].Remove(clearList[i]);
            }

            yield return new WaitForSeconds(1f);
            MoveToNextStep();//For now
        }
        else
        {
            yield return new WaitForSeconds(1f);
            MoveToNextStep();
        }
        yield return null;
    }

    #region Move Hero
    public void MoveToNextStep()
    {
        StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        if (currentStepEventListIndex < steps[currentStepIndex].Count - 1)
        {
            //launch readstory
            StartCoroutine(ReadStory());
        }
        else
        {
            if (currentStepIndex < 3)
            {
                currentStepEventListIndex = 0;
                currentStepIndex++;
                //launch readstory
                StartCoroutine(ReadStory());
            }
            else
            {
                Debug.LogError("Fin du chapitre");

                //Move player to last step
                heroUITransform.SetParent(stepsContainers[stepsContainers.Count - 1].self);
                heroUITransform.SetAsLastSibling();

                chapterCount++;
                currentStepIndex = 0;
                currentStepEventListIndex = 0;

                yield return new WaitForSeconds(1);


                //Fade To Black
                GameManager.Instance.Fade(true);
                yield return new WaitForSeconds(0.5f);

                //Start new Chapter
                StartNewChapter();

                yield return new WaitForSeconds(0.2f);
                //Unfade
                GameManager.Instance.Fade(false);
            }
        }
    }
    #endregion

    public void StartEventCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    #region Feedbacks
    public void HeroLifeFeedback(float value)
    {
        print(value);
        if(value<0)
        {
            //Damage
            StartCoroutine(HitFeedback());
        }
        else
        {
            //Heal

        }
    }
    IEnumerator HitFeedback()
    { 
        heroGraph.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        heroGraph.color = Color.white;
    }
    #endregion
}

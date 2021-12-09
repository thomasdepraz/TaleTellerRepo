using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterType : CardTypes
{
    [HideInInspector] public CardData data;

    public enum CharacterFightingRange
    {
        None, Left, Right, LeftAndRight
    }

    /// <summary>
    /// The base stats of the character
    /// </summary>
    public CharacterStats stats;

    /// <summary>
    /// The behaviour of the character (Allied or enemy)
    /// </summary>
    public CharacterBehaviour behaviour;

    public CharacterFightingRange fightingRange;

    /// <summary>
    /// How many times the player is able to use the card.
    /// </summary>
    public int useCount;
    private int maxUseCount;

    #region Fighting Logic
    /// <summary>
    /// Fight logic between characters /with players. 
    /// </summary>
    public void InitFightEvents(EventQueue queue) //All the fights gets added as an event in the board //TODO Add multiple strke support
    {
        List <CharacterType> characters = GetFightingTargets();

        //if agressive add player to fighting list
        if(behaviour == CharacterBehaviour.Agressive)
        {
            queue.events.Add(FightVSPlayer(queue));
        }

        for (int i = 0; i < characters.Count; i++)
        {
            queue.events.Add(FightVSCharacter(characters[i], queue));
        }
    }

    //Here is the actual fighting logic
    IEnumerator FightVSPlayer(EventQueue currentQueue)
    {
        Debug.Log("Castagne avec le perso");

        if (data.onCharFight != null) data.onCharFight(currentQueue, null);

        //Le character se prend un coup
        stats.baseLifePoints -= GameManager.Instance.currentHero.attackDamage;
        Debug.Log($"Character has {stats.baseLifePoints}");

        yield return new WaitForSeconds(0.2f);

        //Check s'il est encore en vie
        if(stats.baseLifePoints <= 0)//Le character met un coup au player
        {
            EventQueue characterDeathQueue = new EventQueue();

            CharacterDeath(true, characterDeathQueue); //No need to update the queue since it'll be cleaned

            characterDeathQueue.StartQueue();

            while(!characterDeathQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }

            currentQueue.UpdateQueue();
        }
        else
        {

            GameManager.Instance.currentHero.lifePoints -= stats.baseAttackDamage;

            //check for player death, if still alive then keep going

            yield return new WaitForSeconds(0.2f);
            currentQueue.UpdateQueue();
        }
    }   

    IEnumerator FightVSCharacter(CharacterType character, EventQueue currentQueue)
    {
        EventQueue characterDeathQueue = new EventQueue();

        if (data.onCharFight != null) data.onCharFight(currentQueue, character.data);

        //L'autre perso se prend un coup
        Debug.Log("Castagne entre persos");
        
        character.stats.baseLifePoints -= stats.baseAttackDamage;
        Debug.Log($"Other character has {character.stats.baseLifePoints}");
        yield return new WaitForSeconds(0.2f);

        if(character.stats.baseLifePoints <= 0)
        {
            character.CharacterDeath(false, characterDeathQueue);//<-- Add character to queue events
        }

        characterDeathQueue.StartQueue();///<-- actually character death

        while(!characterDeathQueue.resolved)//Wait 
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }

    List<CharacterType> GetFightingTargets()
    {
        List<CharacterType> characters = new List<CharacterType>();

        //Get characters based on fighting range
        switch (fightingRange)
        {
            case CharacterFightingRange.None:

                break;

            case CharacterFightingRange.Left:
                #region Left
                if (data.currentContainer.currentSlot.slotIndex > 0)//Check if can grab left
                {
                    CardContainer cardContainer = CardManager.Instance.board.slots[data.currentContainer.currentSlot.slotIndex - 1].currentPlacedCard;
                    if (cardContainer !=null)//Check if thers a card left
                    {
                        if(cardContainer.data.cardType != null)//Check if theres a type
                        {
                            if(cardContainer.data.cardType.GetType() == typeof(CharacterType))//if cardtype of card is charactertype
                            {
                                CharacterType otherCharacter = cardContainer.data.cardType as CharacterType;

                                if(otherCharacter.behaviour !=  behaviour)//if character have opposing behaviour
                                {
                                    characters.Add(otherCharacter);
                                }
                            }
                        }
                    }
                }
                #endregion
                break;
            case CharacterFightingRange.Right:
                #region Right
                if (data.currentContainer.currentSlot.slotIndex < CardManager.Instance.board.slots.Count - 1)//Check if can grab right
                {
                    CardContainer cardContainer = CardManager.Instance.board.slots[data.currentContainer.currentSlot.slotIndex + 1].currentPlacedCard;
                    if (cardContainer != null)//Check if thers a card left
                    {
                        if (cardContainer.data.cardType != null)//Check if theres a type
                        {
                            if (cardContainer.data.cardType.GetType() == typeof(CharacterType))//if cardtype of card is charactertype
                            {
                                CharacterType otherCharacter = cardContainer.data.cardType as CharacterType;

                                if (otherCharacter.behaviour != behaviour)//if character have opposing behaviour
                                {
                                    characters.Add(otherCharacter);
                                }
                            }
                        }
                    }
                }
                #endregion
                break;

            case CharacterFightingRange.LeftAndRight:
                #region Left
                if (data.currentContainer.currentSlot.slotIndex > 0)//Check if can grab left
                {
                    CardContainer cardContainer = CardManager.Instance.board.slots[data.currentContainer.currentSlot.slotIndex - 1].currentPlacedCard;
                    if (cardContainer != null)//Check if thers a card left
                    {
                        if (cardContainer.data.cardType != null)//Check if theres a type
                        {
                            if (cardContainer.data.cardType.GetType() == typeof(CharacterType))//if cardtype of card is charactertype
                            {
                                CharacterType otherCharacter = cardContainer.data.cardType as CharacterType;

                                if (otherCharacter.behaviour != behaviour)//if character have opposing behaviour
                                {
                                    characters.Add(otherCharacter);
                                }
                            }
                        }
                    }
                }
                #endregion
                #region Right
                if (data.currentContainer.currentSlot.slotIndex < CardManager.Instance.board.slots.Count - 1)//Check if can grab right
                {
                    CardContainer cardContainer = CardManager.Instance.board.slots[data.currentContainer.currentSlot.slotIndex + 1].currentPlacedCard;
                    if (cardContainer != null)//Check if thers a card left
                    {
                        if (cardContainer.data.cardType != null)//Check if theres a type
                        {
                            if (cardContainer.data.cardType.GetType() == typeof(CharacterType))//if cardtype of card is charactertype
                            {
                                CharacterType otherCharacter = cardContainer.data.cardType as CharacterType;

                                if (otherCharacter.behaviour != behaviour)//if character have opposing behaviour
                                {
                                    characters.Add(otherCharacter);
                                }
                            }
                        }
                    }
                }
                #endregion
                break;

            default:
                break;
        }
        return characters;
    }

    void CharacterDeath(bool isCurrentCharacter, EventQueue currentQueue)
    {
        currentQueue.events.Add(CharacterDeathRoutine(isCurrentCharacter, currentQueue));
    }

    IEnumerator CharacterDeathRoutine(bool isCurrentCharacter, EventQueue currentQueue)
    {
        EventQueue onCharDeathQueue = new EventQueue();

        if (data.onCharDeath != null) data.onCharDeath(onCharDeathQueue, data);

        onCharDeathQueue.StartQueue();
        while (!onCharDeathQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        EventQueue discardQueue = new EventQueue();

        // Manage character death card discard, card reset, events deletion
        if (!isCurrentCharacter)
        {
            CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer, discardQueue);
        }
        else//If Im currently resolving this card event, the have to be cleared to prevent errors
        {
            CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer, discardQueue);
            ClearCharacterEvents(discardQueue);//Add queue event 
        }

        discardQueue.StartQueue();

        while(!discardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }
   
    void ClearCharacterEvents(EventQueue queue)
    {
        queue.events.Add(ClearCharacterEventsRoutine(queue));
    }

    IEnumerator ClearCharacterEventsRoutine(EventQueue currentQueue)
    {
        //Clear events
        yield return null;

        CardManager.Instance.board.ClearEvents();
        currentQueue.UpdateQueue();
    }
    #endregion

    /// <summary>
    /// Update method of the useCount.
    /// </summary>
    public void UpdateUseCount()
    {
        useCount--;
    }

    public override void InitType(CardData data)
    {
        this.data = data;

        //Init card effects
        data.InitializeCardEffects(data);

        maxUseCount = useCount;

        data.onCardEnter += OnEnter;// Override onEnter to add fight triggers too
    }

    #region Events


    #region OnEnd (Return to hand / discard)

    public override void OnEnd(EventQueue queue)
    {
        queue.events.Add(OnEndRoutine(queue));
    }
    //The On End Event of character is different since the use count update and it returns to the hand instead of going directly in the discard pile
    private IEnumerator OnEndRoutine(EventQueue currentQueue)
    {
        //Return to hand but cannot push cards out of the hand

        UpdateUseCount();//Maybe move this to onStartEvent 

        EventQueue discardQueue = new EventQueue();

        if(useCount > 0)
        {
            CardManager.Instance.board.ReturnCardToHand(data.currentContainer, false, discardQueue);//TODO implement the add to eventqueue part
        }
        else//No more uses so its discarded
        {
            useCount = maxUseCount;
            CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer, discardQueue);//TODO implement the add to eventqueue part
        }

        discardQueue.StartQueue();//<-- The actual discard happens here

        while(!discardQueue.resolved)//Wait while the action has not ended
        {
            yield return new WaitForEndOfFrame();
        }

        //Unqueue
        currentQueue.UpdateQueue();
    }
    #endregion

    #region OnEnter (Event Trigger + Fight)
    void OnEnter(EventQueue queue, CardData data)
    {
        ////add effects to board manager list <-- This is no longer needed normally
        //for (int i = 0; i < data.effects.Count; i++)
        //{
        //    //Init effect that adds a routine to the manager list

        //    CardManager.Instance.board.currentQueue.Add(tempEffect());//THIS IS TEMPORARY
        //}

        //CardManager.Instance.board.cardEffectQueue.Add(EffectTrigger());

        InitFightEvents(queue);
    }

    #endregion

    #endregion
}

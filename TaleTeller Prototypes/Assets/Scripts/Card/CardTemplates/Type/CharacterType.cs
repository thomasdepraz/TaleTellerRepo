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
    public void InitFightEvents() //All the fights gets added as an event in the board
    {
        List <CharacterType> characters = GetFightingTargets();

        //if agressive add player to fighting list
        if(behaviour == CharacterBehaviour.Agressive)
        {
            CardManager.Instance.board.currentQueue.Add(FightVSPlayer());
        }

        for (int i = 0; i < characters.Count; i++)
        {
            CardManager.Instance.board.currentQueue.Add(FightVSCharacter(characters[i]));
        }
    }

    //Here is the actual fighting logic
    IEnumerator FightVSPlayer()
    {
        bool deathFinished = false;
        Debug.Log("Castagne avec le perso");

        //Le character se prend un coup
        stats.baseLifePoints -= GameManager.Instance.currentHero.attackDamage;
        Debug.Log($"Character has {stats.baseLifePoints}");

        yield return new WaitForSeconds(0.2f);

        //Check s'il est encore en vie
        if(stats.baseLifePoints <= 0)//Le character met un coup au player
        {
            CharacterDeath(ref deathFinished, true); //No need to update the queue since it'll be cleaned

            while(!deathFinished)
            {
                yield return new WaitForEndOfFrame();
            }

            //CardManager.Instance.board.UpdateStoryQueue();
        }
        else
        {

            GameManager.Instance.currentHero.lifePoints -= stats.baseAttackDamage;

            //check for player death, if still alive then keep going

            yield return new WaitForSeconds(0.2f);
            CardManager.Instance.board.UpdateQueue();
        }
    }   

    IEnumerator FightVSCharacter(CharacterType character)
    {
        bool fightEnded = false;
        //L'autre perso se prend un coup
        Debug.Log("Castagne entre persos");
        
        character.stats.baseLifePoints -= stats.baseAttackDamage;
        Debug.Log($"Other character has {character.stats.baseLifePoints}");
        yield return new WaitForSeconds(0.2f);

        if(character.stats.baseLifePoints <= 0)
        {
            character.CharacterDeath(ref fightEnded, false);//<-- watch out for the test cases
        }
        else
        {
            fightEnded = true;
        }

        while(!fightEnded)//Wait 
        {
            yield return new WaitForEndOfFrame();
        }

        CardManager.Instance.board.UpdateQueue();
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

    void CharacterDeath(ref bool deathEnded, bool isCurrentCharacter)
    {
        //Manage character death card discard, card reset, events deletion
        if(!isCurrentCharacter)
        {
            CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer,ref deathEnded);
        }
        else//If Im currently resolving this card event, the have to be cleared to prevent errors
        {
            CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer, ref deathEnded, ClearCharacterEvents);
        }
    }
    void ClearCharacterEvents()
    {
        //Clear events
        CardManager.Instance.board.ClearEvents();

        //Pickup the story processing
        CardManager.Instance.board.UpdateQueue(); //<-- is this solid enough/ maybe implement a resume processing method in board that is based on the current state of the story processing
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
        data.onStartEvent += OnStart;//This is temporary, normally nothing happens on the start event for a character type card
        data.onEnterEvent += OnEnter;// Override onEnter to add fight triggers too
        data.onEndEvent += OnEndCharacter;
    }

    #region Events

    #region OnStart
    public void OnStart()
    {
        //add to OnStartQueue
        CardManager.Instance.board.currentQueue.Add(OnStartRoutine());
    }

    private IEnumerator OnStartRoutine()
    {
        Debug.Log("Il");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("se passe des trucs");


        yield return new WaitForSeconds(0.5f);

        //Unqueue
        CardManager.Instance.board.UpdateQueue();
    }
    #endregion

    #region OnEnd (Return to hand / discard)
    public void OnEndCharacter()
    {
        CardManager.Instance.board.currentQueue.Add(OnEndRoutine());   
    }
    //The On End Event of character is different since the use count update and it returns to the hand instead of going directly in the discard pile
    private IEnumerator OnEndRoutine()
    {
        bool routineEnded = false;

        //Return to hand but cannot push cards out of the hand
        
        UpdateUseCount();//Maybe move this to onStartEvent 

        if(useCount > 0)
        {
            CardManager.Instance.board.ReturnCardToHand(data.currentContainer, false, ref routineEnded);
        }
        else//No more uses so its discarded
        {
            useCount = maxUseCount;
            CardManager.Instance.board.DiscardCardFromBoard(data.currentContainer, ref routineEnded);
        }



        //yield return new WaitForSeconds(0.5f);
        while(!routineEnded)//Wait while the action has not ended
        {
            yield return new WaitForEndOfFrame();
        }


        //Unqueue
        CardManager.Instance.board.UpdateQueue();
    }
    #endregion

    #region OnEnter (Event Trigger + Fight)
    void OnEnter()
    {
        ////add effects to board manager list <-- This is no longer needed normally
        //for (int i = 0; i < data.effects.Count; i++)
        //{
        //    //Init effect that adds a routine to the manager list

        //    CardManager.Instance.board.currentQueue.Add(tempEffect());//THIS IS TEMPORARY
        //}

        //CardManager.Instance.board.cardEffectQueue.Add(EffectTrigger());

        InitFightEvents();
    }
    #endregion

    #endregion
}

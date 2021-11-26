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

    List<IEnumerator> fightQueue;

    /// <summary>
    /// Fight logic between characters /with players. 
    /// </summary>
    public void InitFightEvents() //All the fights gets added as an event in the board
    {
        List <CharacterType> characters = GetFightingTargets();

        for (int i = 0; i < characters.Count; i++)
        {
            CardManager.Instance.board.cardEventQueue.Add(FightVSCharacter(characters[i]));
        }

        //if agressive add player to fighting list
        if(behaviour == CharacterBehaviour.Agressive)
        {
            CardManager.Instance.board.cardEventQueue.Add(FightVSPlayer());
        }
    }
    //Here is the actual fighting logic
    IEnumerator FightVSPlayer()
    {
        //Le player se prend un coup
        Debug.Log("Castagne avec le perso");
        yield return null;
        CardManager.Instance.board.UpdateStoryQueue();
    }   

    IEnumerator FightVSCharacter(CharacterType character)
    {
        //L'autre perso se prend un coup
        Debug.Log("Castagne entre persos");
        yield return null;
        CardManager.Instance.board.UpdateStoryQueue();
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

                break;

            case CharacterFightingRange.Right:

                break;

            case CharacterFightingRange.LeftAndRight:

                break;

            default:
                break;
        }
        return characters;
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

        maxUseCount = useCount;
        data.onStartEvent += OnStart;//This is temporary, normally nothing happens on the start event for a character type card
        data.onEnterEvent += OnEnter;// Override onEnter to add fight triggers too
        data.onEndEvent += OnEnd;
    }

    #region Events

    #region OnStart
    public void OnStart()
    {
        //add to OnStartQueue
        CardManager.Instance.board.onStartQueue.Add(OnStartRoutine());
    }

    private IEnumerator OnStartRoutine()
    {
        Debug.Log("Il");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("se passe des trucs");


        yield return new WaitForSeconds(0.5f);

        //Unqueue
        CardManager.Instance.board.UpdateOnStartQueue();
    }
    #endregion

    #region OnEnd (Return to hand / discard)
    public void OnEnd()
    {
        CardManager.Instance.board.onEndQueue.Add(OnEndRoutine());   
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
        CardManager.Instance.board.UpdateOnEndQueue();
    }
    #endregion

    #region OnEnter (Event Trigger + Fight)
    void OnEnter()
    {
        //add effects to board manager list
        for (int i = 0; i < data.effects.Count; i++)
        {
            //Init effect that adds a routine to the manager list

            CardManager.Instance.board.cardEffectQueue.Add(tempEffect());//THIS IS TEMPORARY
        }

        //CardManager.Instance.board.cardEffectQueue.Add(EffectTrigger());

        InitFightEvents();
    }
    IEnumerator EffectTrigger()
    {
        yield return null;
        //UpdateStoryQueue
    }

    IEnumerator tempEffect()
    {
        Debug.Log("Trigger Effect");
        yield return null;
        CardManager.Instance.board.UpdateStoryQueue();
    }
    #endregion

    #endregion
}

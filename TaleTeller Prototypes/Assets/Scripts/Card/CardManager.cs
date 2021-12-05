using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : Singleton<CardManager>
{
    private void Awake()
    {
        CreateSingleton();
    }
    [Header("References")]
    public Deck cardDeck;
    public Hand cardHand;
    public Board board;

    public ManaSystem manaSystem;

    public CardPicker cardPicker;

    [HideInInspector] public BoardSlot currentHoveredSlot;
    [HideInInspector] public bool holdingCard;
    [HideInInspector] public CardContainer currentCard;
    [HideInInspector] public CardContainer hoveredCard;

    public GameObject cardHandContainer;

    public Pointer pointerRef;
    public GameObject pointer;
}

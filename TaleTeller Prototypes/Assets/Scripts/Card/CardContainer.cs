using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour
     , IBeginDragHandler
     , IEndDragHandler
     , IDragHandler
     , IPointerEnterHandler
     , IPointerExitHandler
 
{
    [Header("References")]
    public RectTransform rectTransform;
    public Image selfImage;
    private Transform targetTransform;
    private Vector3 origin;
    private Vector3 basePosition;

    public DraftSlot currentSlot;
    public CardData data;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardCreativityCost;

    public GameObject attackUI, healthUI, timerUI;
    public TextMeshProUGUI attackText, healthText, timerText;

    public RectTransform shadowTransform;

    delegate void UIEvent();

    private Vector2 originPosition;
    private int siblingIndex;

    //Variables for dragging management
    public CanvasGroup canvasGroup;
    private bool isDragging;
    private bool canTween = true;
    
    private void Update()
    {
        if(isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            canvasGroup.blocksRaycasts = true;
        }
        if(isDragging)
        {
            //follow 
            shadowTransform.rotation = new Quaternion(0,0,0,1);
            rectTransform.position = targetTransform.position;
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, new Quaternion(CardManager.Instance.pointerRef.pointerDirection.y, -CardManager.Instance.pointerRef.pointerDirection.x, 0, 1), Time.deltaTime);
        }
    }

    public void CardInit(CardData data)
    {
        this.data = data;

        attackUI.SetActive(false);
        healthUI.SetActive(false);
        timerUI.SetActive(false);

        data.currentContainer = this;

        //Change effect if needed
        if (data.keyCardActivated)
        {
            data.effect = data.deadCardEffect;
        }

        //Init characterValues
        data.characterStats.Initialize();


        //load Data and activate gameobject
        basePosition = transform.position;
        data.currentInterestCooldown = data.interestCooldown;
        cardName.text = data.cardName;

        if(data.type == CardType.Character)
        {
            attackUI.SetActive(true);
            attackText.text = data.characterStats.baseAttackDamage.ToString();

            healthUI.SetActive(true);
            healthText.text = data.characterStats.baseLifePoints.ToString();
        }

        if(data.isKeyCard && data.keyCardActivated)
        {
            timerUI.SetActive(true);
            timerText.text = data.currentInterestCooldown.ToString();
        }


        if (data.effect == null)
            cardDescription.text = data.description;
        else
            cardDescription.text = data.effect.effectName;

        cardCreativityCost.text = data.creativityCost.ToString();

        gameObject.SetActive(true);
        originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

        //add random rotation 
        rectTransform.rotation = new Quaternion(0,0,Random.Range(-0.1f,0.1f),1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Allows OnBeginDrag and OnEndDrag.
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!GameManager.Instance.storyManager.isReadingStory)//Can only interact with cards if story isnt reading
        {
            #region Tweening + Graphics
            LeanTween.cancel(gameObject);

            origin = rectTransform.anchoredPosition;
            targetTransform = CardManager.Instance.pointer.transform;
            transform.SetAsLastSibling();
            shadowTransform.gameObject.SetActive(true);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            //rectTransform.localScale = Vector3.one;

            #endregion

            CardManager.Instance.holdingCard = true;
            CardManager.Instance.currentCard = this;

            //Trigger movement detection
            isDragging = true;
            canvasGroup.blocksRaycasts = false;

            if (currentSlot != null)
                currentSlot.canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        #region Tweening + Graphics
        canTween = false;
        rectTransform.rotation = new Quaternion(0, 0, 0, 0);
        rectTransform.localScale = Vector3.one;
        shadowTransform.gameObject.SetActive(false);
        #endregion

        //If hovers, swap card, else drop in or out of slot
        if(CardManager.Instance.hoveredCard != null)
        {
            #region Swap Card
            //Swap cards from the board
            if (currentSlot != null) //If card in board
            {
                if (CardManager.Instance.hoveredCard.currentSlot != null) //if other card in board
                {
                    DraftSlot tempSlot = CardManager.Instance.hoveredCard.currentSlot;

                    CardManager.Instance.hoveredCard.currentSlot = currentSlot;
                    CardManager.Instance.hoveredCard.currentSlot.currentPlacedCard = CardManager.Instance.hoveredCard;
                    CardManager.Instance.hoveredCard.rectTransform.position = CardManager.Instance.hoveredCard.currentSlot.transform.position;
                    CardManager.Instance.hoveredCard.currentSlot.canvasGroup.blocksRaycasts = false;

                    tempSlot.currentPlacedCard = this;
                    currentSlot = tempSlot;
                    rectTransform.position = currentSlot.transform.position;
                }
                else //if other card in hand
                {
                    //If you have enough mana to swap cards
                    int manaDifference = CardManager.Instance.hoveredCard.data.creativityCost - data.creativityCost;
                    if (CardManager.Instance.manaSystem.CanUseCard(manaDifference))
                    {
                        DraftSlot tempSlot = currentSlot;
                        tempSlot.currentPlacedCard = CardManager.Instance.hoveredCard;
                        CardManager.Instance.hoveredCard.currentSlot = tempSlot;

                        currentSlot = null;


                        rectTransform.anchoredPosition = CardManager.Instance.hoveredCard.originPosition;

                        CardManager.Instance.hoveredCard.rectTransform.position = tempSlot.transform.position;
                        transform.SetParent(CardManager.Instance.cardHandContainer.transform);

                        //ApplyManaDiff
                        if (manaDifference > 0) CardManager.Instance.manaSystem.LoseMana(manaDifference);
                        else if (manaDifference < 0) CardManager.Instance.manaSystem.GainMana(Mathf.Abs(manaDifference));

                    }
                    else
                    {
                        //reset card position
                        rectTransform.anchoredPosition = originPosition;
                        canTween = true; //temp ?
                        Debug.Log("Not enough mana");
                    }

                }

                CardManager.Instance.hoveredCard = null;
            }
            else //if card in hand
            {
                if (CardManager.Instance.hoveredCard.currentSlot != null)//If card other card in board
                {
                    //If you have enough mana to swap cards
                    int manaDifference = data.creativityCost - CardManager.Instance.hoveredCard.data.creativityCost;

                    if (CardManager.Instance.manaSystem.CanUseCard(manaDifference))
                    {
                        DraftSlot tempSlot = CardManager.Instance.hoveredCard.currentSlot;
                        tempSlot.currentPlacedCard = this;
                        currentSlot = tempSlot;

                        CardManager.Instance.hoveredCard.currentSlot = null;

                        CardManager.Instance.hoveredCard.rectTransform.anchoredPosition = originPosition;
                        CardManager.Instance.hoveredCard.transform.SetParent(CardManager.Instance.cardHandContainer.transform); //set back to hand

                        rectTransform.position = tempSlot.transform.position;

                        //ApplyManaDiff
                        if (manaDifference > 0) CardManager.Instance.manaSystem.LoseMana(manaDifference);
                        else if (manaDifference < 0) CardManager.Instance.manaSystem.GainMana(Mathf.Abs(manaDifference));

                    }
                    else
                    {
                        //reset card position
                        rectTransform.anchoredPosition = originPosition;
                        canTween = true; //temp ?
                        Debug.Log("Not enough mana");
                    }
                }
                else //if other card in hand
                {
                    Vector3 position = CardManager.Instance.hoveredCard.rectTransform.anchoredPosition;

                    CardManager.Instance.hoveredCard.rectTransform.anchoredPosition = originPosition;
                    rectTransform.anchoredPosition = position;
                }
                    
                CardManager.Instance.hoveredCard = null;
            }
            #endregion
        }
        else
        {
            #region Drop in/out slot
            //Drop In Slot
            if (CardManager.Instance.currentHoveredSlot != null)
            {
                //if you have enough man else abort and reset card
                if (CardManager.Instance.manaSystem.CanUseCard(data.creativityCost) && CardManager.Instance.currentHoveredSlot != currentSlot)
                {
                    if (currentSlot != null)
                    {
                        currentSlot.currentPlacedCard = null;
                        currentSlot = null;
                    }

                    CardManager.Instance.currentHoveredSlot.currentPlacedCard = this;
                    currentSlot = CardManager.Instance.currentHoveredSlot;
                    currentSlot.canvasGroup.blocksRaycasts = false;
                    rectTransform.position = CardManager.Instance.currentHoveredSlot.transform.position;

                    CardManager.Instance.manaSystem.LoseMana(data.creativityCost);
                }
                else
                {
                    //reset card position
                    rectTransform.anchoredPosition = originPosition;
                    canTween = true; //temp ?

                    if (CardManager.Instance.currentHoveredSlot == currentSlot) Debug.Log("SameSlot");
                    else Debug.Log("Not enough mana");
                }
            }
            else //Drop Out Slot
            {
                targetTransform = null;
                CardManager.Instance.holdingCard = false;
                originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
                rectTransform.anchoredPosition = originPosition;

                if (currentSlot != null)//Remove card from board !! Add list system
                {
                    currentSlot.currentPlacedCard = null;
                    currentSlot.canvasGroup.blocksRaycasts = true;
                    currentSlot = null;
                }

                //Refill Mana
                CardManager.Instance.manaSystem.GainMana(data.creativityCost);

            }
            #endregion
        }


        CardManager.Instance.currentHoveredSlot = null;
        CardManager.Instance.holdingCard = false;
        CardManager.Instance.currentCard = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardManager.Instance.holdingCard && CardManager.Instance.currentCard != this)
        {
            CardManager.Instance.hoveredCard = this;
        }

        #region Tweening
        if (!LeanTween.isTweening(gameObject))
            originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

        if (transform.parent == CardManager.Instance.cardHandContainer.transform && !CardManager.Instance.holdingCard && canTween)//Check if in hand
        {
            canTween = false;

            //Reset card rotation
            rectTransform.rotation = new Quaternion(0, 0, 0, 0);

            //Scale up and bring to front;
            LeanTween.cancel(gameObject);
            siblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();

            rectTransform.anchoredPosition = originPosition;
            originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

            rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0);
            rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            //LeanTween.move(rectTransform, rectTransform.anchoredPosition + new Vector2(0, 10f), 0.5f).setEaseOutSine();
            shadowTransform.gameObject.SetActive(true);
        }
        #endregion
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(CardManager.Instance.holdingCard && CardManager.Instance.currentCard != this)
        {
            CardManager.Instance.hoveredCard = null;
        }

        #region Tweening
        canTween = true;
        if (transform.parent == CardManager.Instance.cardHandContainer.transform && !CardManager.Instance.holdingCard)//Check if in hand
        {
            //Scale down 
            shadowTransform.gameObject.SetActive(false);
            LeanTween.cancel(gameObject);
            rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0.5f);
            rectTransform.localScale = Vector3.one;

            //LeanTween.move(rectTransform, originPosition, 0.5f).setEaseOutSine();
            //rectTransform.anchoredPosition = originPosition;

            if (!isDragging)
                transform.SetSiblingIndex(siblingIndex);
        }
        #endregion
    }

    public void ResetCard()
    {
        //unload Data and deactivate gameobject
        cardName.text = string.Empty;
        cardDescription.text = string.Empty;
        cardCreativityCost.text = string.Empty;

        data.currentContainer = null;
        data = null;
        currentSlot = null;

        rectTransform.position = basePosition;

        //Reset to hidden hand
        CardManager.Instance.cardHand.currentHand.Remove(this);
        gameObject.SetActive(false);
        transform.SetParent(CardManager.Instance.cardHandContainer.transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inspire : MonoBehaviour
{
    public List<GameObject> stacks = new List<GameObject>();
    public Image frame;
    public RectTransform root;

    [Header("Data")]
    public int useCount;
    public int manaCost;
    public int drawCardsCount;
    public int darkIdeasCount;

    void Start()
    {
        for (int i = 0; i < stacks.Count; i++)
        {
            stacks[i].SetActive(false);
        }

        for (int i = 0; i < useCount; i++)
        {
            stacks[i].SetActive(true);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    public void ClickButton()
    {
        if (useCount == 0) ShakeFrame();

        if(useCount > 0 && CardManager.Instance.manaSystem.CanUseCard(manaCost))
        {
            StartCoroutine(InspireRoutine());
        }
    }
    IEnumerator InspireRoutine()
    {
        CardManager.Instance.manaSystem.LoseMana(manaCost);
        UpdateStacks(-1);

        EventQueue drawQueue = new EventQueue();
        CardManager.Instance.cardDeck.DrawCards(drawCardsCount, drawQueue);
        drawQueue.StartQueue();
        while (!drawQueue.resolved) { yield return new WaitForEndOfFrame(); }

        EventQueue appearQueue = new EventQueue();
        int random = Random.Range(0, PlotsManager.Instance.darkIdeas.Count);
        CardData cardToAppear = PlotsManager.Instance.darkIdeas[random];
        cardToAppear = cardToAppear.InitializeData(cardToAppear);

        CardManager.Instance.CardAppearToDeck(cardToAppear, appearQueue, CardManager.Instance.plotAppearTransform.position);

        appearQueue.StartQueue();
        while (!appearQueue.resolved) { yield return new WaitForEndOfFrame(); }
    }


    public void OnPointerEnter()
    {
        LeanTween.cancel(frame.gameObject);
        frame.gameObject.transform.rotation = Quaternion.identity;
        LeanTween.scale(frame.gameObject, Vector2.one * 1.2f, 0.1f).setEaseOutQuint();
    }

    public void OnPointerExit()
    {
        LeanTween.cancel(frame.gameObject);
        frame.gameObject.transform.rotation = Quaternion.identity;
        LeanTween.scale(frame.gameObject, Vector2.one, 0.1f).setEaseOutQuint();
    }

    public void UpdateStacks(int amount)
    {
        BounceFrame();

        useCount += amount;
        if (useCount < 0) useCount = 0;

        if(amount>0)
        {
            for (int i = 0; i < stacks.Count; i++)
            {
                if (!stacks[i].activeSelf)
                {
                    stacks[i].SetActive(true);
                    break;
                }

            }
        }
        else if(amount < 0)
        {
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].activeSelf)
                {
                    stacks[i].SetActive(false);
                    break;
                }
            }
        }

    }

    public void BounceFrame()
    {
        LeanTween.cancel(frame.gameObject);
        frame.gameObject.transform.localScale = Vector3.one;
        frame.gameObject.transform.rotation = Quaternion.identity;
        LeanTween.scale(frame.gameObject, Vector3.one * 1.2f, 0.2f).setEaseInOutQuint().setLoopPingPong(1);
    }

    public void ShakeFrame()
    {
       
        LeanTween.cancel(frame.gameObject);
        frame.gameObject.transform.localScale = Vector3.one;
        LeanTween.rotateZ(frame.gameObject, -3, 0.02f).setEaseInOutCubic().setOnComplete(
            value => LeanTween.rotateZ(frame.gameObject, 3, 0.04f).setEaseInOutCubic().setLoopPingPong(2).setOnComplete(
                val => LeanTween.rotateZ(frame.gameObject, 0, 0.02f).setEaseInOutCubic()));
    }
}

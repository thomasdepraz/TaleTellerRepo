using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardReward : Reward
{

    public List<CardData> rewardCards = new List<CardData>();
    public int numberToPick;

    public AddCardReward(int numberToPick)
    {
        rewardCards = GetCards(StoryManager.Instance.actCount, 6);
        this.numberToPick = numberToPick;
    }

    public AddCardReward(List<CardData> rewardCards, int numberToPick)
    {
        this.rewardCards = rewardCards;
        this.numberToPick = numberToPick;
    }

    public override IEnumerator ApplyRewardRoutine(EventQueue queue)
    {
        UtilityClass.InitCardList(rewardCards);

        CardPickerScreen screen = new CardPickerScreen(PickScreenMode.ADD,numberToPick, rewardCards, true);
        bool wait = true;
        screen.Open(() => { wait = false; });
        while (wait) { yield return new WaitForEndOfFrame(); }

        while(screen.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen.Close(()=> { wait = false; });
        while (wait) { yield return new WaitForEndOfFrame(); }

        List<CardData> cards = new List<CardData>();
        for (int i = 0; i < screen.pickedCards.Count; i++)
        {
            cards.Add(screen.pickedCards[i].container.data);
        }
        EventQueue appearQueue = new EventQueue();
        for (int i = 0; i < cards.Count; i++)
        {
            CardManager.Instance.CardAppearToDeck(cards[i], appearQueue, CardManager.Instance.plotAppearTransform.position);
        }
        appearQueue.StartQueue();
        while (!appearQueue.resolved) { yield return new WaitForEndOfFrame(); }

        queue.UpdateQueue();
    }

    List<CardData> GetCards(int actNumber, int number)
    {
        switch (actNumber)
        {
            case 0:
                return GetRandomCards(55f, 30f, 15f, 0f, number);
            case 1:
                return GetRandomCards(30f, 40f, 20f, 10f, number);
            case 2:
                return GetRandomCards(0f, 40f, 40f, 20f, number);
            default:
                return null;
        }
    }

    List<CardData> GetRandomCards(float commonWeight, float uncommonWeight, float rareWeight, float epicWeight, int number)
    {
        List<CardData> cards = new List<CardData>();
        List<CardData> commonRewards = FilterList(RewardManager.Instance.rewardPoolAllIdeas, CardRarity.Common);
        List<CardData> uncommonRewards = FilterList(RewardManager.Instance.rewardPoolAllIdeas, CardRarity.Uncommon);
        List<CardData> rareRewards = FilterList(RewardManager.Instance.rewardPoolAllIdeas, CardRarity.Rare);
        List<CardData> epicRewards = FilterList(RewardManager.Instance.rewardPoolAllIdeas, CardRarity.Epic);


        for (int i = 0; i < number; i++)
        {
            float random = Random.Range(0f, 100f);
            if (random <= commonWeight)
            {
                int r = Random.Range(0, commonRewards.Count);
                cards.Add(commonRewards[r]);
            }
            else if (random > commonWeight && random <= commonWeight + uncommonWeight)
            {
                int r = Random.Range(0, uncommonRewards.Count);
                cards.Add(uncommonRewards[r]);
            }
            else if (random > commonWeight + uncommonWeight && random <= commonWeight + uncommonWeight + rareWeight)
            {
                int r = Random.Range(0, rareRewards.Count);
                cards.Add(rareRewards[r]);
            }
            else if (random > commonWeight + uncommonWeight + rareWeight && random <= commonWeight + uncommonWeight + rareWeight + epicWeight)
            {
                int r = Random.Range(0, epicRewards.Count);
                cards.Add(epicRewards[r]);
            }
        }
        return cards;
    }

    public List<CardData> FilterList(List<CardData> source, CardRarity rarity)
    {
        List<CardData> result = new List<CardData>();

        for (int i = 0; i < source.Count; i++)
        {
            if (source[i].rarity == rarity)
                result.Add(source[i]);

        }
        return result;
    }

    public override string GetString()
    {
        return $"Add {numberToPick} cards";
    }
}
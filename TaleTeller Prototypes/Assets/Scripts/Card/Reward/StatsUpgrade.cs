using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class StatsUpgrade
{
    static int value;

    public static Action<EventQueue> GetRandomStatUpgrade(ref TextMeshProUGUI statsButtonDescription)
    {
        Action<EventQueue> result = null;
        int r = UnityEngine.Random.Range(0, 3);
        value = UnityEngine.Random.Range(0,3);//Calculus of value -- TODO

        switch (r)
        {
            case 0:
                result = HealthUpgrade;
                statsButtonDescription.text = $"Heal {value} HP";
                break;

            case 1:
                result = AttackUpgrade;
                statsButtonDescription.text = $"Gain {value} Attack Damage";
                break;

            case 2:
                result = GoldUpgrade;
                statsButtonDescription.text = $"Gain {value} Max Gold";
                break;
        }

        return result;
    }

    static void HealthUpgrade(EventQueue queue)
    {
        queue.events.Add(StatUpgrade(queue, "health", value));
    }
    static void AttackUpgrade(EventQueue queue)
    {
        queue.events.Add(StatUpgrade(queue, "attack", value));
    }
    static void GoldUpgrade(EventQueue queue)
    {
        queue.events.Add(StatUpgrade(queue, "gold", value));
    }

    static IEnumerator StatUpgrade(EventQueue queue, string param, int value)
    {
        switch (param)
        {
            case "health":
                GameManager.Instance.currentHero.maxLifePoints += value;
                break;

            case "attack":
                GameManager.Instance.currentHero.attackDamage += value;
                break;

            case "gold":
                GameManager.Instance.currentHero.maxGoldPoints += value;
                break;

            default:
                Debug.LogError("Param Error");
                break;
        }
        yield return null;

        queue.UpdateQueue();
    }

}

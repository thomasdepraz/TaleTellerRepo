using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDumpEffect : MalusEffect
{
    public struct DumpInfos
    {
        public EffectValueType typeDumped;
        public int value;

        public DumpInfos(EffectValueType _type, int _value)
        {
            typeDumped = _type;
            value = _value;
        }
    }

    public EffectValueType typeToDump;

    public delegate void DumpEvent(EventQueue queue, DumpInfos infos);
    public DumpEvent onDump;

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData cardData = null)
    {
        var hero = GameManager.Instance.currentHero;
        DumpInfos infos = new DumpInfos(typeToDump, 0);

        switch (typeToDump)
        {
            case EffectValueType.Attack:
                infos.value = hero.bonusDamage;
                hero.bonusDamage = 0;
                break;

            case EffectValueType.Gold:
                infos.value = hero.goldPoints;
                hero.goldPoints = 0;
                break;

            case EffectValueType.Life:
                infos.value = hero.lifePoints;
                hero.lifePoints = 0;
                break;

            default:
                break;
        }

        EventQueue dumpQueue = new EventQueue();
        if (onDump != null) onDump(dumpQueue, infos);

        dumpQueue.StartQueue();

        while (!dumpQueue.resolved)
            yield return new WaitForEndOfFrame();

        yield return null;
        currentQueue.UpdateQueue();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionReward
{
    Action<EventQueue>[] actions = {DiscardAction};

    public static Action<EventQueue> GetRandomAction()
    {
        Action<EventQueue> action = null;


        return action;
    }

    static void DiscardAction(EventQueue queue)
    {
        queue.events.Add(null);
    }

    
}

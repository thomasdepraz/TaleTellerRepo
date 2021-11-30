using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventQueue
{
    public List<IEnumerator> events;
    public bool resolved = false;
    public void StartQueue()
    {
        //StartCoroutine on monobehaviour manager
        EventManager.Instance.StartCoroutine(events[0]);
    }

    public void UpdateQueue()
    {
        //Unqueue
        if (events.Count > 0) events.RemoveAt(0);

        if (events.Count > 0)//Play next event 
        {
            EventManager.Instance.StartCoroutine(events[0]);
        }
        else//resume the story where you left it
        {
            resolved = true;
        }
    }
}

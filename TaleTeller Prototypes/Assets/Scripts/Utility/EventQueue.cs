using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventQueue
{
    public List<IEnumerator> events = new List<IEnumerator>();
    public bool resolved = false;
    public void StartQueue()
    {
        //StartCoroutine on monobehaviour manager

        if(events.Count > 0)
        {
            StoryManager.Instance.queueList.Add(this);
            EventManager.Instance.StartCoroutine(events[0]);
        }
        else
        {
            resolved = true;
        }
    }

    public void UpdateQueue()
    {
        EventManager.Instance.StartCoroutine(UpdateQueueRoutine());
    }

    public IEnumerator UpdateQueueRoutine()
    {
        while(GameManager.Instance.pause)
        { 
            yield return new WaitForEndOfFrame();
        }

        //Unqueue
        if (events.Count > 0) events.RemoveAt(0);

        if (events.Count > 0)//Play next event 
        {
            EventManager.Instance.StartCoroutine(events[0]);
        }
        else//resume the story where you left it
        {
            resolved = true;
            StoryManager.Instance.queueList.Remove(this);
        }
    }
}

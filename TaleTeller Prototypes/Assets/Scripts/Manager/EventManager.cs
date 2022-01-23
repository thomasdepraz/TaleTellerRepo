using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public void Awake()
    {
        CreateSingleton();
    }

    //THIS MONOBEHAVIOUR SOLE PURPOSE IS TO HOLD EVENTQUEUES COROUTINES -- DO NOT DELETE
}

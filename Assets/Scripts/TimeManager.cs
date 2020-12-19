using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    static TimeManager _instance;


    public TimeManager() {
        if (_instance== null)
        {
            _instance = new TimeManager();
        }
    }

   
    // The TimeEvent is not the real name of the type check with Alon!
    //   LinkedList<TimeEvent> timeEventLList
    // public void RemoveEvent(TimeEvent) {}
    // public void AddEvent(TimeEvent) { }
}

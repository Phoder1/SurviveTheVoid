using System.Collections.Generic;
using UnityEngine;
using Assets.TimeEvents;

public class TimeManager : MonoBehaviour {
    LinkedList<TimeEvent> timedEventsList;

    public static TimeManager _instance;
    private void Awake() {
        if( _instance != null) {
            Destroy(gameObject);
        }else {
            _instance = this;
        }
        timedEventsList = new LinkedList<TimeEvent>();
    }

    // Update is called once per frame
    void Update() {
        if (timedEventsList.Count > 0) {
            while (timedEventsList.First.Value.triggerTime <= Time.time) {
                TimeEvent triggeredEvent = timedEventsList.First.Value;
                triggeredEvent.Trigger();
                if(timedEventsList.Count == 0) { break; }
            }
        }
    }

    public LinkedListNode<TimeEvent> AddEvent(TimeEvent timedEvent) {

        float eventTriggerTime = timedEvent.triggerTime;

        if (timedEventsList.Count > 0 && eventTriggerTime > timedEventsList.First.Value.triggerTime) {
            LinkedListNode<TimeEvent> currentNode = timedEventsList.Last;

            while (currentNode != timedEventsList.First && eventTriggerTime < currentNode.Value.triggerTime) {
                currentNode = currentNode.Previous;
            }

            return timedEventsList.AddAfter(currentNode, timedEvent);
        }
        else {

            return timedEventsList.AddFirst(timedEvent);

        }
    }

    public void RemoveEvent(LinkedListNode<TimeEvent> eventToRemove) {
        if(eventToRemove == null || timedEventsList.Count == 0)
            return;
        timedEventsList.Remove(eventToRemove);
    }
}

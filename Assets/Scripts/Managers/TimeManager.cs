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
    // Start is called before the first frame update
    void Start() { 
        Debug.Log("Current time: " + Time.deltaTime);
        
    }

    // Update is called once per frame
    void Update() {
        if (timedEventsList.Count > 0) {
            while (timedEventsList.First.Value.triggerTime <= Time.time) {
                TimeEvent triggeredEvent = timedEventsList.First.Value;
                triggeredEvent.Trigger();
                if(timedEventsList.First.Value == triggeredEvent) {
                    timedEventsList.RemoveFirst();
                }
                if(timedEventsList.Count == 0) { break; }
            }
        }
    }

    public void AddEvent(TimeEvent timedEvent) {
        float eventTriggerTime = timedEvent.triggerTime;
        if (timedEventsList.Count > 0 && eventTriggerTime > timedEventsList.First.Value.triggerTime) {
            LinkedListNode<TimeEvent> currentNode = timedEventsList.Last;
            while (currentNode != timedEventsList.First && eventTriggerTime < currentNode.Value.triggerTime) {
                currentNode = currentNode.Previous;
            }
            timedEventsList.AddAfter(currentNode, timedEvent);
        }
        else {
            timedEventsList.AddFirst(timedEvent);
        }
    }

    public void RemoveEvent(TimeEvent eventToRemove) {
        LinkedListNode<TimeEvent> currentNode = timedEventsList.First;
        while(currentNode.Value != eventToRemove) {
            currentNode = currentNode.Next;
            if(currentNode == timedEventsList.Last) {
                break;
            }
        }
        if(currentNode.Value == eventToRemove) {
            timedEventsList.Remove(currentNode);
        }
    }
}

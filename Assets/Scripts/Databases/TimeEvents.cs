using UnityEngine;

namespace Assets.TimeEvents
{
    public abstract class TimeEvent
    {
        public float triggerTime;
        protected bool eventTriggered = false;


        public TimeEvent(float triggerTime) {
            this.triggerTime = triggerTime;
            TimeManager._instance.AddEvent(this);
        }

        public abstract void Trigger();
        public void Cancel() {
            if (!eventTriggered) {
                TimeManager._instance.RemoveEvent(this);
            }
        }
        public void UpdateTriggerTime(float triggerTime) {
            this.triggerTime = triggerTime;
            Cancel();
            TimeManager._instance.AddEvent(this);
        }
    }
}

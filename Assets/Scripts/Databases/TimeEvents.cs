using UnityEngine;

namespace Assets.TimeEvents
{
    public abstract class TimeEvent
    {
        public readonly float triggerTime;
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
    }
}

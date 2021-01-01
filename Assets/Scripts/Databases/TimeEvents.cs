using UnityEngine;

namespace Assets.TimeEvents
{
    public abstract class TimeEvent
    {
        public readonly float triggerTime;
        protected TileSlot triggeringTile;
        protected readonly Vector2Int eventPosition;
        protected bool eventTriggered = false;


        public TimeEvent(float triggerTime, Vector2Int eventPosition) {
            this.triggerTime = triggerTime;
            this.eventPosition = eventPosition;
        }

        public abstract void Trigger();
        public void Cancel() {
            if (!eventTriggered) {
                TimeManager._instance.RemoveEvent(this);
            }
        }
    }
}

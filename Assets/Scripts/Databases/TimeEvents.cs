using System.Collections.Generic;

namespace Assets.TimeEvents
{
    public abstract class TimeEvent
    {
        LinkedListNode<TimeEvent> node;
        public float triggerTime;


        public TimeEvent(float triggerTime) {
            Init(triggerTime);
        }

        private void Init(float triggerTime) {
            this.triggerTime = triggerTime;
            node = TimeManager._instance.AddEvent(this);
        }
        public void Trigger() {
            TriggerBehaviour();
            Remove();
        }
        protected abstract void TriggerBehaviour();
        public void Cancel() {
            OnCancel();
            Remove();
        }
        protected virtual void OnCancel() { }
        private void Remove() {
            if (node != null) {
                TimeManager._instance.RemoveEvent(node);
                node = null;
            }
        }
        public void UpdateTriggerTime(float triggerTime) {
            Remove();
            Init(triggerTime);
        }
    }
}

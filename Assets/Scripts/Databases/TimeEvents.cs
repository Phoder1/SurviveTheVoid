using Assets.TilesData;
using UnityEngine;

namespace Assets.TimeEvents
{
    public abstract class TimeEvent
    {
        public readonly float triggerTime;
        protected BlockTile triggeringTile;
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

    public class ToothPasteEvent : TimeEvent
    {
        protected new ToothPaste triggeringTile;

        public ToothPasteEvent(ToothPaste triggeringTile, float triggerTime, Vector2Int eventPosition) : base(triggerTime, eventPosition) {
            this.triggeringTile = triggeringTile;
        }

        public override void Trigger() {
            eventTriggered = true;
            GridManager._instance.SetTile(triggeringTile.replacementTile, eventPosition, TileMapLayer.Floor, false);
            
        }
    }
}

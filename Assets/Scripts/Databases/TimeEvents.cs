using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.TimeEvents
{
    public abstract class TimeEvent
    {
        private static int eventCount = 0;
        private readonly int eventID;
        public readonly float triggerTime;
        protected Tiles.TileAbst triggeringTile;
        protected readonly Vector2Int eventPosition;
        protected bool eventTriggered = false;


        public TimeEvent(float triggerTime, Vector2Int eventPosition) {
            this.triggerTime = triggerTime;
            this.eventPosition = eventPosition;
            eventCount++;
            eventID = eventCount;
        }

        public abstract void Trigger();
        public void Cancel() {
            if (!eventTriggered) {
                TimeManager._instance.RemoveEvent(this);
            }
        }

        /*public override bool Equals(object obj) {
            if( obj is TimeEvent timeEventObj) {
                return base.Equals(obj) && eventID == timeEventObj.eventID && triggeringTile == timeEventObj.triggeringTile;
            }else {
                return false;
            }
            
        }*/
    }

    public class ToothPasteEvent : TimeEvent
    {
        protected new Tiles.ToothPaste triggeringTile;

        public ToothPasteEvent(Tiles.ToothPaste triggeringTile, float triggerTime, Vector2Int eventPosition) : base(triggerTime, eventPosition) {
            this.triggeringTile = triggeringTile;
        }

        public override void Trigger() {
            eventTriggered = true;
            GridManager._instance.SetTile(eventPosition, triggeringTile.replacementTile, false);
            
        }
    }
}

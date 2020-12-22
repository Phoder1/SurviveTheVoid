using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using Assets.TimeEvents;

public class Tiles : MonoBehaviour
{
    public static Tiles _instance;
    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }
    public TilesSO tiles;
    /*
    * How to create a new Tile:
    * 1) Create a new child class to TileSO
    * 3) Make sure you change the 2nd string in the menuName property in CreateAssetMenu to the Tile name
    * 4) Create a new scriptable object instance of your tile
    * 5) Create a new public variable for your tile and drag in the inspector a new instance of the scriptable object.
    * 6) Add the tile to the tile array in AddTiles() and increase the array size by 1.
    */

    [CreateAssetMenu(menuName = "Tiles Pack")]
    public class TilesSO : ScriptableObject
    {
        public MoonTile moonTile;
        public ToothPaste toothPasteTile;
        public CircusTile circusTile;
    }



    [Serializable]
    public abstract class TileAbst
    {
        public TileBase tileBase;
        protected TimeEvent eventInstance;
        public bool isSolid;
        public virtual void Init(Vector2Int _position) { }

        public virtual void Remove() {
            if (eventInstance != null) {
                eventInstance.Cancel();
            }
        }
        public virtual void OnPressInteraction() { }
        public virtual void LongPressUpdate() { }

    }
    [Serializable]
    public class MoonTile : TileAbst
    {
        public MoonTile() {
            if (_instance != null) {
                MoonTile SO = _instance.tiles.moonTile;
                tileBase = SO.tileBase;
            }
        }
    }
    [Serializable]
    public class CircusTile : TileAbst
    {
        public CircusTile() {
            if(_instance != null) {
                CircusTile SO = _instance.tiles.circusTile;
                tileBase = SO.tileBase;
            }
            
        }
    }
    [Serializable]
    public class ToothPaste : TileAbst
    {
        public TileAbst replacementTile;
        public int eventDelay;

        public ToothPaste() {
            if (_instance != null) {
                ToothPaste SO = _instance.tiles.toothPasteTile;
                tileBase = SO.tileBase;
                eventDelay = SO.eventDelay;
                replacementTile = new CircusTile();
            }
        }

        public override void Init(Vector2Int position) {
            eventInstance = new ToothPasteEvent(this, Time.time + eventDelay, position);
            TimeManager._instance.AddEvent(eventInstance);
        }
    }

}
















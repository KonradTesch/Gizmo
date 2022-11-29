using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.TileCreation
{
    public class LevelTileData : ScriptableObject
    {
        public List<ChangeData> backgroundTileChanges;
        public List<ChangeData> groundTileChanges;
        public List<ChangeData> platformTileChanges;
        public List<ChangeData> rampTileChanges;
        public List<ChangeData> spikesTileChanges;

        public List<MovingObjectData> movingObjects;

        public Vector2[] collectablePositions;

        public Vector2Int tileSize;
        public TileCreator.TileTypes tileType;
        public Player.PlayerController.PlayerModes playerMode;
    }

    [System.Serializable]
    public class ChangeData
    {
        public Vector3Int position;
        public TileBase tile;
        [HideInInspector]public Matrix4x4 transform;
    }

    [System.Serializable]
    public class MovingObjectData
    {
        public float moveSpeed;
        public Vector2[] waypoints;
        public Level.WaypointFollower.MovingType movingType;
        public bool vanishing;
        public float vanishTime;

        public List<ChangeData> tileChanges;

        public MovingObjectData(float moveSpeed, Vector2[] waypoints, Level.WaypointFollower.MovingType movingType, bool vanishing, float vanishTime)
        {
            this.moveSpeed = moveSpeed;
            this.waypoints = waypoints;
            this.movingType = movingType;
            this.vanishing = vanishing;
            this.vanishTime = vanishTime;

            tileChanges = new();
        }
    }
}

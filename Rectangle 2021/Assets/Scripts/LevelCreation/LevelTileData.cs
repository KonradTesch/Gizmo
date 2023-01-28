using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.LevelCreation
{
    public class LevelTileData : ScriptableObject
    {
        public List<ChangeData> backgroundTileChanges;
        public List<ChangeData> borderTileChanges;
        public List<ChangeData> groundTileChanges;
        public List<ChangeData> platformTileChanges;
        public List<ChangeData> rampTileChanges;
        public List<ChangeData> spikesTileChanges;

        [Space()]
        public List<MovingObjectData> movingObjects;

        [Space()]
        public Vector2[] collectablePositions;

        [Space()]
        public bool hasStar = false;
        public Vector3 starPosition;

        [Space()]
        public bool isRespawn = false;
        public Vector3 respawnPosition;

        [Space()]
        public Vector2Int tileSize;
        public TileCreator.TileTypes tileType;
        public Player.PlayerController.PlayerModes playerMode;

        public ChangeData GetChangeData(List<ChangeData> changeList, Vector3Int position)
        {
            for(int i = 0; i < changeList.Count; i++)
            {
                if(position == changeList[i].position)
                {
                    return changeList[i];
                }
            }

            return null;
        }
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

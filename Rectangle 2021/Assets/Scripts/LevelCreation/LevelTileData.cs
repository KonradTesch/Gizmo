using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.LevelCreation
{
    public class LevelTileData : ScriptableObject
    {

        /// <summary>
        /// The tilemap information for the background layer.
        /// </summary>
        [Tooltip("The tilemap information for the background layer.")]
        public List<ChangeData> backgroundTileChanges;

        /// <summary>
        /// The tilemap information for the border layer.
        /// </summary>
        [Tooltip("The tilemap information for the border layer.")]
        public List<ChangeData> borderTileChanges;

        /// <summary>
        /// The tilemap information for the ground layer.
        /// </summary>
        [Tooltip("The tilemap information for the ground layer.")]
        public List<ChangeData> groundTileChanges;

        /// <summary>
        /// The tilemap information for the platform layer.
        /// </summary>
        [Tooltip("The tilemap information for the platform layer.")]
        public List<ChangeData> platformTileChanges;

        /// <summary>
        /// The tilemap information for the ramp layer.
        /// </summary>
        [Tooltip("The tilemap information for the ramp layer.")]
        public List<ChangeData> rampTileChanges;

        /// <summary>
        /// The tilemap information for the spikes layer.
        /// </summary>
        [Tooltip("The tilemap information for the spikes layer.")]
        public List<ChangeData> spikesTileChanges;

        [Space()]

        /// <summary>
        /// A list of movcing object in the tile.
        /// </summary>
        [Tooltip("A list of movcing object in the tile.")]
        public List<MovingObjectData> movingObjects;

        [Space()]

        /// <summary>
        /// Whether this tile has a collectbale item.
        /// </summary>
        [Tooltip("Whether this tile has a collectbale item.")]
        public bool hasStar = false;

        /// <summary>
        /// The position of the collectable item.
        /// </summary>
        [Tooltip("The position of the collectable item.")]
        public Vector3 starPosition;

        [Space()]

        /// <summary>
        /// Whetther the tile is a respawn.
        /// </summary>
        [Tooltip("Whetther the tile is a respawn.")]
        public bool isRespawn = false;

        /// <summary>
        /// The respawn position, if the tile is an respawn.
        /// </summary>
        [Tooltip("The respawn position, if the tile is an respawn.")]
        public Vector3 respawnPosition;

        [Space()]

        /// <summary>
        /// The size in tilemap tiles of the level tile.
        /// </summary>
        [Tooltip("The size in tilemap tiles of the level tile.")]
        public Vector2Int tileSize;

        /// <summary>
        /// The tile type of the level tile.
        /// </summary>
        [Tooltip("The tile type of the level tile.")]
        public TileCreator.TileTypes tileType;

        /// <summary>
        /// The player mode of the tile.
        /// </summary>
        [Tooltip("The player mode of the tile.")]
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

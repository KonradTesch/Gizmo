using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.Level;

namespace Rectangle.LevelCreation
{
    public class TileCreator : MonoBehaviour
    {
        [Header("TileMaps")]

        /// <summary>
        /// The tilemap for the background layer.
        /// </summary>
        [Tooltip("The tilemap for the background layer.")]
        public Tilemap backgroundTilemap;

        /// <summary>
        /// The tilemap for the border layer.
        /// </summary>
        [Tooltip("The tilemap for the border layer.")]
        public Tilemap borderTilemap;

        /// <summary>
        /// The tilemap for the ground layer.
        /// </summary>
        [Tooltip("The tilemap for the ground layer.")]
        public Tilemap groundTilemap;

        /// <summary>
        /// The tilemap for the ramp layer.
        /// </summary>
        [Tooltip("The tilemap for the ramp layer.")]
        public Tilemap rampTilemap;

        /// <summary>
        /// The tilemap for the platform layer.
        /// </summary>
        [Tooltip("The tilemap for the platform layer.")]
        public Tilemap onewayPlatformTilemap;

        /// <summary>
        /// The tilemap for the spiekes layer.
        /// </summary>
        [Tooltip("The tilemap for the spikes layer.")]
        public Tilemap spikesTilemap;

        /// <summary>
        /// The size in tilemap tiles for the current level tile.
        /// </summary>
        [Tooltip("The size in tilemap tiles for the current level tile.")]
        public Vector2Int tileSize = new Vector2Int(32, 32);

        [Header("Asset Folder")]

        /// <summary>
        /// The patzh were the tile data will be saved.
        /// </summary>
        [Tooltip("The patzh were the tile data will be saved.")]
        public string saveFolderPath;

        [Header("Tile Settings")]

        /// <summary>
        /// The building settings.
        /// </summary>
        [Tooltip("The building settings.")]
        public LevelBuilderSettings builderSettings;

        [Space()]

        /// <summary>
        /// Whether there should bw the possibílity to place a collectable in this level tile.
        /// </summary>
        [Tooltip("Whether there should bw the possibílity to place a collectable in this level tile.")]
        public bool hasCollectable;

        /// <summary>
        /// The transform, that shows the position for an collectbale item.
        /// </summary>
        [Tooltip("")]
        public Transform collectableTransform;

        [Space()]

        /// <summary>
        /// Whether this tile is a respawn pooint.
        /// </summary>
        [Tooltip("")]
        public bool isRespawn;

        /// <summary>
        /// The respawn position, if this tile is a respwn point.
        /// </summary>
        [Tooltip("The respawn position, if this tile is a respwn point.")]
        public Transform respawnPosition;

        [Space()]

        /// <summary>
        /// The name of the tile.
        /// </summary>
        [Tooltip("The name of the tile.")]
        public string tileName;

        /// <summary>
        /// The tile type of the tile.
        /// </summary>
        [Tooltip("The tile type of the tile.")]
        public TileTypes tileType;

        /// <summary>
        /// The player mode of the tile.
        /// </summary>
        [Tooltip("The player mode of the tile.")]
        public Player.PlayerController.PlayerModes playerMode;

        public enum TileTypes { undefined, Anchor, LeftAndRight, LeftAndDown, LeftAndUp, UpAndRight, DownAndRight, UpAndDown };

        [HideInInspector] public List<Tilemap> movingObjects;
    }
}

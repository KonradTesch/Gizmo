using System.Collections;
using System.Collections.Generic;
using Rectangle.LevelCreation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.Level
{
    /// <summary>
    /// A sriptable object, that stores various information fopr the builder classes.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelBuilderSettings", menuName = "Rectangle/Level Builder Settings")]
    public class LevelBuilderSettings : ScriptableObject
    {
        [Header("Tiles")]

        /// <summary>
        /// The size in tilemap tiles of one level tile.
        /// </summary>
        [Tooltip("The size in tilemap tiles of one level tile.")]
        public Vector2Int tileSize;

        /// <summary>
        /// The tilemap information for the grid.
        /// </summary>
        [Tooltip("The tilemap information for the grid.")]
        public LevelTileData gridTiles;

        /// <summary>
        /// The tilemap information for a closed border (for the anchor tiles).
        /// </summary>
        [Tooltip("The tilemap information for a closed border (for the anchor tiles).")]
        public LevelTileData closedBorderTile;

        /// <summary>
        /// The tilemap information for an open border (for the anchor tiles).
        /// </summary>
        [Tooltip("The tilemap information for an open border (for the anchor tiles).")]
        public LevelTileData openBorderTile;

        /// <summary>
        /// The tilemap tile for the border.
        /// </summary>
        [Tooltip("The tilemap tile for the border.")]
        public TileBase borderTile;

        /// <summary>
        /// The tilemap tile for an oneway platform.
        /// </summary>
        [Tooltip("The tiulemap tile for an oneway platform.")]
        public TileBase platformTile;

        [Header("Level Backgrounds")]

        /// <summary>
        /// The layer mask for the grid.
        /// </summary>
        [Tooltip("The layer mask for the grid.")]
        public LayerMask gridLayer;

        /// <summary>
        /// The layer mask for the background.
        /// </summary>
        [Tooltip("The layer mask for the grid.")]
        public LayerMask backgroundLayer;

        /// <summary>
        /// The background image for the grid.
        /// </summary>
        [Tooltip("The background image for the grid.")]
        public Sprite backgroundImage;

        [Header("LevelTiles")]

        /// <summary>
        /// The prefab for a level tile.
        /// </summary>
        [Tooltip("The prefab for a level tile.")]
        public GameObject levelTilePrefab;

        /// <summary>
        /// The prefab for an anchor tile.
        /// </summary>
        [Tooltip("The prefab for an anchor tile.")]
        public GameObject anchorTilePrefab;

        /// <summary>
        /// The sprite for an anchor tile.
        /// </summary>
        [Tooltip("The sprite for an anchor tile.")]
        public Sprite anchorTileSprite;

        /// <summary>
        /// An array with sprites for each tile type.
        /// </summary>
        [Tooltip("An array with sprites for each tile type.")]
        public TileType[] tileTypes;

        /// <summary>
        /// The sprite for the level start.
        /// </summary>
        [Tooltip("The sprite for the level start.")]
        public Sprite startLevelSprite;

        /// <summary>
        /// The sprite for the level finish.
        /// </summary>
        [Tooltip("The sprite for the level finish.")]
        public Sprite endLevelSprite;

        [Header("Collectable Item")]


        /// <summary>
        /// The prefab of the collectable item.
        /// </summary>
        [Tooltip("The prefab of the collectable item.")]
        public GameObject collectablePrefab;

        /// <summary>
        /// The sprite for the nut (collectable item).
        /// </summary>
        [Tooltip("The sprite for the nut (collectable item).")]
        public Sprite nutSprite;

        /// <summary>
        /// Returns the right level tile sprite of an tile type and player mode.
        /// </summary>
        public Sprite GetTileTypeSprite(TileCreator.TileTypes tileType, Player.PlayerController.PlayerModes playerMode)
        {
            foreach(TileType type in tileTypes)
            {
                if(type.tileType == tileType)
                {
                    for(int i = 0; i < type.modeSprites.Count; i++)
                    {
                        if (type.modeSprites[i].playerMode == playerMode)
                        {
                            return type.modeSprites[i].tileSprite;
                        }
                    }
                }
            }

            Debug.LogWarning("The LevelBuildSettings haven't a tileype sprite for type " + tileType);
            return null;
        }

    }

    [System.Serializable]
    public class TileType
    {
        public TileCreator.TileTypes tileType;
        public List<TileSprite> modeSprites;
    }

    [System.Serializable]
    public class TileSprite
    {
        public Player.PlayerController.PlayerModes playerMode;
        public Sprite tileSprite;
    }
}

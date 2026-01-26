using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.LevelCreation;
using Rectangle.Player;
using Rectangle.General;

namespace Rectangle.Level
{
    /// <summary>
    /// The builder class for the level tiles
    /// </summary>
    public class TileBuilder : MonoBehaviour
    {
        /// <summary>
        /// An array wuith the avaivable level tiles.
        /// </summary>
        [Tooltip("An array wuith the avaivable level tiles.")]
        [SerializeField] private LevelTileData[] levelTiles;

        /// <summary>
        /// An array with the avaivable anchor tiles.
        /// </summary>
        [Tooltip("An array with the avaivable anchor tiles.")]
        [SerializeField] private LevelTileData[] anchorTiles;

        /// <summary>
        /// The layer for the lecel grid.
        /// </summary>
        [Tooltip("The layer for the lecel grid.")]
        [SerializeField] private LayerMask gridLayer;

        /// <summary>
        /// The Prefab of the collectable item (nut).
        /// </summary>
        [Tooltip("The Prefab of the collectable item (nut).")]
        [SerializeField] private GameObject collectableTilePrefab;

        /// <summary>
        /// The prefab of an moving platform.
        /// </summary>
        [Tooltip("The prefab of an moving platform.")]
        [SerializeField] private GameObject movingPlatformPrefab;

        [Header("Tilemaps")]
        //The different layers of tilemaps
        [SerializeField] private Tilemap backgroundTilemap;
        [SerializeField] private Tilemap borderTilemap;
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap rampTilemap;
        [SerializeField] private Tilemap onewayPlatformTilemap;
        [SerializeField] private Tilemap spikesTilemap;


        private Dictionary<TileCreator.TileTypes, List<LevelTileData>> modeTiles;

        private LevelBuilderSettings builderSettings;

        public enum Direction { None, Up, Right, Down, Left}

        private void Awake()
        {
            InitModeTiles();
        }

        private void Start()
        {
            builderSettings = GameBehavior.instance.builderSettings;
        }

        /// <summary>
        /// Builds the placed tiles and anchor tiles on the tilemaps.
        /// </summary>
        public void BuildLevel(List<LevelTile> placedTiles, List<LevelTile> anchorLevelTiles, LevelData levelData)
        {
            foreach (LevelTile tile in placedTiles)
            {
                Vector2 originPosition = ((Vector2)tile.transform.position / groundTilemap.transform.lossyScale) - new Vector2(builderSettings.tileSize.x / 2, builderSettings.tileSize.y / 2);

                List<LevelTileData> possibleTiles = new();

                for (int i = 0; i < modeTiles[tile.tileType].Count; i++)
                {
                    if (modeTiles[tile.tileType][i].playerMode == tile.playerMode)
                    {
                        possibleTiles.Add(modeTiles[tile.tileType][i]);
                    }
                }

                int randomIndex = Random.Range(0, possibleTiles.Count);

                Debug.Log("Build Tile with type: " + tile.tileType.ToString() + " and mode: " + tile.playerMode.ToString());

                BuildTile(Vector2Int.RoundToInt(originPosition), possibleTiles[randomIndex]);

            }

            foreach(LevelTile anchorTile in anchorLevelTiles)
            {
                Vector2 originPosition = ((Vector2)anchorTile.transform.position / groundTilemap.transform.lossyScale) - new Vector2(builderSettings.tileSize.x / 2, builderSettings.tileSize.y / 2);

                LevelTileData randomAnchorTile = anchorTiles[Random.Range(0, anchorTiles.Length)];

                BackgroundMode background =  Physics2D.OverlapPoint(anchorTile.transform.position, gridLayer).GetComponent<BackgroundMode>();

                background.playerMode = randomAnchorTile.playerMode;

                anchorTile.gameObject.SetActive(false);

                CloseAnkerTile(anchorTile);
                BuildTile(Vector2Int.RoundToInt(originPosition), randomAnchorTile);
            }
        }

        /// <summary>
        /// Gets a random play mode out of possible play modes for the given type.
        /// </summary>
        public PlayerController.PlayerModes GetRandomPlayerMode(TileCreator.TileTypes tileType)
        {
            int randomIndex = Random.Range(0, modeTiles[tileType].Count);
            return modeTiles[tileType][randomIndex].playerMode;
        }

        /// <summary>
        /// Sets up the sorted mode tiles dictionary.
        /// </summary>
        private void InitModeTiles()
        {
            modeTiles = new()
            {
                { TileCreator.TileTypes.Anchor, new()},
                { TileCreator.TileTypes.LeftAndRight, new()},
                { TileCreator.TileTypes.LeftAndUp, new()},
                { TileCreator.TileTypes.LeftAndDown, new()},
                { TileCreator.TileTypes.UpAndRight, new()},
                { TileCreator.TileTypes.DownAndRight, new()},
                { TileCreator.TileTypes.UpAndDown, new()}

            };

            foreach (LevelTileData tile in levelTiles)
            {
                switch (tile.tileType)
                {
                    case TileCreator.TileTypes.Anchor:
                        modeTiles[TileCreator.TileTypes.Anchor].Add(tile);
                        break;
                    case TileCreator.TileTypes.LeftAndRight:
                        modeTiles[TileCreator.TileTypes.LeftAndRight].Add(tile);
                        break;
                    case TileCreator.TileTypes.LeftAndUp:
                        modeTiles[TileCreator.TileTypes.LeftAndUp].Add(tile);
                        break;
                    case TileCreator.TileTypes.LeftAndDown:
                        modeTiles[TileCreator.TileTypes.LeftAndDown].Add(tile);
                        break;
                    case TileCreator.TileTypes.UpAndRight:
                        modeTiles[TileCreator.TileTypes.UpAndRight].Add(tile);
                        break;
                    case TileCreator.TileTypes.DownAndRight:
                        modeTiles[TileCreator.TileTypes.DownAndRight].Add(tile);
                        break;
                    case TileCreator.TileTypes.UpAndDown:
                        modeTiles[TileCreator.TileTypes.UpAndDown].Add(tile);
                        break;
                }
            }
        }

        /// <summary>
        /// Builds a single level tile on the tilemaps.
        /// </summary>
        private void BuildTile(Vector2Int originPosition, LevelTileData tile)
        {
            foreach (ChangeData change in tile.backgroundTileChanges)
            {
                TileChangeData tileChange = new()
                {
                    position = change.position + (Vector3Int)originPosition,
                    tile = change.tile,
                    transform = change.transform
                };
                backgroundTilemap.SetTile(tileChange, true);
            }
            backgroundTilemap.RefreshAllTiles();

            foreach(ChangeData change in tile.borderTileChanges)
            {
                TileChangeData tileChange = new()
                {
                    position = change.position + (Vector3Int)originPosition,
                    tile = change.tile,
                    transform = change.transform
                };
                borderTilemap.SetTile(tileChange, true);
            }
            borderTilemap.RefreshAllTiles();

            foreach (ChangeData change in tile.groundTileChanges)
            {
                TileChangeData tileChange = new()
                {
                    position = change.position + (Vector3Int)originPosition,
                    tile = change.tile,
                    transform = change.transform
                };
                groundTilemap.SetTile(tileChange, true);
            }
            groundTilemap.RefreshAllTiles();

            foreach (ChangeData change in tile.rampTileChanges)
            {
                TileChangeData tileChange = new()
                {
                    position = change.position + (Vector3Int)originPosition,
                    tile = change.tile,
                    transform = change.transform
                };
                rampTilemap.SetTile(tileChange, true);
            }
            rampTilemap.RefreshAllTiles();

            foreach (ChangeData change in tile.platformTileChanges)
            {
                TileChangeData tileChange = new()
                {
                    position = change.position + (Vector3Int)originPosition,
                    tile = change.tile,
                    transform = change.transform
                };
                onewayPlatformTilemap.SetTile(tileChange, true);
            }
            onewayPlatformTilemap.RefreshAllTiles();

            foreach (ChangeData change in tile.spikesTileChanges)
            {
                TileChangeData tileChange = new()
                {
                    position = change.position + (Vector3Int)originPosition,
                    tile = change.tile,
                    transform = change.transform
                };
                spikesTilemap.SetTile(tileChange, true);
            }
            spikesTilemap.RefreshAllTiles();


            foreach (MovingObjectData movingObject in tile.movingObjects)
            {
                Tilemap platformTilemap = Instantiate(movingPlatformPrefab, groundTilemap.transform.parent).GetComponent<Tilemap>();
                platformTilemap.gameObject.SetActive(false);
                platformTilemap.transform.localScale = groundTilemap.transform.localScale;

                WaypointFollower objjectFollower = platformTilemap.GetComponent<WaypointFollower>();
                objjectFollower.moveSpeed = movingObject.moveSpeed;
                objjectFollower.waypoints = movingObject.waypoints;
                objjectFollower.movingType = movingObject.movingType;
                objjectFollower.vanishing = movingObject.vanishing;
                objjectFollower.vanishingTime = movingObject.vanishTime;

                platformTilemap.gameObject.SetActive(true);

                foreach (ChangeData change in movingObject.tileChanges)
                {
                    TileChangeData tileChange = new()
                    {
                        position = change.position + (Vector3Int)originPosition,
                        tile = change.tile,
                        transform = change.transform
                    };
                    platformTilemap.SetTile(tileChange, true);
                }

                platformTilemap.RefreshAllTiles();
            }

            if (GameBehavior.instance.levelBuilder.levelData.GetGridSpot(GameBehavior.instance.levelBuilder.WorldPositionToCoordinate((Vector2)originPosition)).collectable)
            {
                GameObject star = Instantiate(builderSettings.collectablePrefab);

                star.transform.position = (Vector2)originPosition + (Vector2)tile.starPosition;
            }

        }

        /// <summary>
        /// Closes the unused exits of an anchor tile.
        /// </summary>
        private void CloseAnkerTile(LevelTile anchorTile)
        {
            LevelBuilder levelBuilder = GameBehavior.instance.levelBuilder;

            Vector2Int anchorCoordinate = levelBuilder.WorldPositionToCoordinate(anchorTile.transform.position);

            int tileHeight = (int)(builderSettings.tileSize.y / groundTilemap.transform.localScale.y);
            int tileWidth = (int)(builderSettings.tileSize.x / groundTilemap.transform.localScale.x);

            Vector2Int tileOrigin = anchorCoordinate * new Vector2Int(tileWidth, tileHeight);

            if (Physics2D.OverlapPoint(levelBuilder.CoordinateToWorldPosition(anchorCoordinate + Vector2Int.up), builderSettings.backgroundLayer) == null)
            {
                
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth/2 - 4, tileHeight - 1);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth / 2 + 4, tileHeight - 1);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.closedBorderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth / 2 - 4, tileHeight - 1);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth / 2 + 4, tileHeight - 1);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.openBorderTile);
            }

            if (Physics2D.OverlapPoint(levelBuilder.CoordinateToWorldPosition(anchorCoordinate + Vector2Int.right), builderSettings.backgroundLayer) == null)
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 + 4);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.closedBorderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 + 4);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.openBorderTile);
            }

            if (Physics2D.OverlapPoint(levelBuilder.CoordinateToWorldPosition(anchorCoordinate + Vector2Int.down), builderSettings.backgroundLayer) == null)
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth / 2 - 4, 0);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth / 2 + 4, 0);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.closedBorderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth / 2 - 4, 0);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth / 2 + 4, 0);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.openBorderTile);
            }

            if (Physics2D.OverlapPoint(levelBuilder.CoordinateToWorldPosition(anchorCoordinate + Vector2Int.left), builderSettings.backgroundLayer) == null)
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(0, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(0, tileHeight / 2 + 4);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.closedBorderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(0, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(0, tileHeight / 2 + 4);

                ReplaceBox(borderTilemap, startPos, endPos, builderSettings.openBorderTile);
            }

            borderTilemap.RefreshAllTiles();
        }

        /// <summary>
        /// Replaces a box on a tilemap with the given level tile.
        /// </summary>
        public void ReplaceBox(Tilemap tilemap, Vector2Int pos1, Vector2Int pos2, LevelTileData replaceTile)
        {
            Vector2 diff = pos2 - pos1;

            List<ChangeData> changes = new List<ChangeData>();

            if (tilemap == borderTilemap)
            {
                changes = replaceTile.borderTileChanges;
            }
            else if (tilemap == backgroundTilemap)
            {
                changes = replaceTile.backgroundTileChanges;
            }
            else if (tilemap == groundTilemap)
            {
                changes = replaceTile.groundTileChanges;
            }
            else if (tilemap == spikesTilemap)
            {
                changes = replaceTile.spikesTileChanges;
            }
            else if (tilemap == rampTilemap)
            {
                changes = replaceTile.rampTileChanges;
            }


            for (int y = 0; y <= Mathf.Abs(diff.y); y++)
            {
                for (int x = 0; x <= Mathf.Abs(diff.x); x++)
                {
                    Vector3Int pos;

                    if (diff.x < 0 && diff.y < 0)
                    {
                        pos = (Vector3Int)pos1 - new Vector3Int(x, y, 0);
                    }
                    else if (diff.x < 0)
                    {
                        pos = (Vector3Int)pos1 + new Vector3Int(-x, y, 0);
                    }
                    else if (diff.y < 0)
                    {
                        pos = (Vector3Int)pos1 + new Vector3Int(x, -y, 0);
                    }
                    else
                    {
                        pos = (Vector3Int)pos1 + new Vector3Int(x, y, 0);
                    }

                    ChangeData changeData = replaceTile.GetChangeData(changes, new Vector3Int(pos.x % builderSettings.tileSize.x, pos.y % builderSettings.tileSize.y, 0));

                    if (changeData != null)
                    {
                        TileChangeData tileChangeData = new TileChangeData()
                        {
                            position = pos,
                            tile = changeData.tile,
                            transform = changeData.transform
                        };

                        tilemap.SetTile(pos, null);

                        tilemap.SetTile(tileChangeData, true);
                    }
                    else
                    {
                        tilemap.SetTile(pos, null);
                    }

                }
            }
        }

    }

    [System.Serializable]
    public class TileGroupData
    {
        public TileCreator.TileTypes tileType;
        public PlayerController.PlayerModes playerMode;
        public int tileCount;
        public Sprite tileSprite;
    }
}

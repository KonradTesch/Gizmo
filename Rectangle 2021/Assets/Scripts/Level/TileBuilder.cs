using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.LevelCreation;
using Rectangle.Player;
using Rectangle.General;

namespace Rectangle.Level
{
    public class TileBuilder : MonoBehaviour
    {
        [SerializeField] private LevelTileData[] levelTiles;
        [SerializeField] private LevelTileData[] anchorTiles;
        [SerializeField] private LayerMask gridLayer;
        [SerializeField] private GameObject collectableTilePrefab;
        [SerializeField] private GameObject movingPlatformPrefab;

        [Header("Tilemaps")]

        [SerializeField] private Tilemap backgroundTilemap;
        [SerializeField] private Tilemap borderTilemap;
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap rampTilemap;
        [SerializeField] private Tilemap onewayPlatformTilemap;
        [SerializeField] private Tilemap spikesTilemap;

        [HideInInspector] public Dictionary<Vector2, List<InventoryTile>> collectedTiles = new();

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

        public void BuildLevel(List<LevelTile> placedTiles, List<LevelTile> anchorLevelTiles, LevelData levelData)
        {
            foreach (LevelTile tile in placedTiles)
            {
                Vector2 originPosition = ((Vector2)tile.transform.position / groundTilemap.transform.lossyScale) - new Vector2(tile.tileSize.x / 2, tile.tileSize.y / 2);

                List<LevelTileData> possibleTiles = new();

                for (int i = 0; i < modeTiles[tile.tileType].Count; i++)
                {
                    if (modeTiles[tile.tileType][i].playerMode == tile.playerMode)
                    {
                        possibleTiles.Add(modeTiles[tile.tileType][i]);
                    }
                }

                int randomIndex = Random.Range(0, possibleTiles.Count);

                BuildTile(Vector2Int.RoundToInt(originPosition), possibleTiles[randomIndex]);

            }

            foreach(LevelTile anchorTile in anchorLevelTiles)
            {
                Vector2 originPosition = ((Vector2)anchorTile.transform.position / groundTilemap.transform.lossyScale) - new Vector2(anchorTile.tileSize.x / 2, anchorTile.tileSize.y / 2);

                LevelTileData randomAnchorTile = anchorTiles[Random.Range(0, anchorTiles.Length)];

                BackgroundMode background =  Physics2D.OverlapPoint(anchorTile.transform.position, gridLayer).GetComponent<BackgroundMode>();

                background.playerMode = randomAnchorTile.playerMode;
                background.GetComponent<GridField>().backgroundRend.color = GameBehavior.instance.builderSettings.GetModeColor(randomAnchorTile.playerMode);

                anchorTile.gameObject.SetActive(false);

                CloseAnkerTile(anchorTile);
                BuildTile(Vector2Int.RoundToInt(originPosition), randomAnchorTile);

                Vector2Int coordinate = GameBehavior.instance.levelBuilder.WorldPositionToCoordinate(anchorTile.transform.position);

                if (levelData.GetAnchorByCoordinates(coordinate).collectableTiles.Count > 0)
                {
                    Vector2 corner = (Vector2)anchorTile.transform.position - ((Vector2)GameBehavior.instance.builderSettings.tileSize / 2);

                    PlaceCollectableTiles(levelData.GetAnchorByCoordinates(coordinate).collectableTiles, randomAnchorTile.collectablePositions, corner);
                    anchorTile.collectableTiles = null;
                }
            }
        }

        public PlayerController.PlayerModes GetRandomPlayerMode(TileCreator.TileTypes tileType)
        {
            int randomIndex = Random.Range(0, modeTiles[tileType].Count);
            Debug.Log($"GetRandomPlayerMode({tileType.ToString()})");
            return modeTiles[tileType][randomIndex].playerMode;
        }

        private void PlaceCollectableTiles( List<PlannedTile> collectableTiles, Vector2[] positions, Vector2 tilePosition)
        {
            List<CollectableTile> collectables = new();
            int i;
            foreach (PlannedTile tile in collectableTiles)
            {
                bool alreadyAdded = false;

                if (collectedTiles.ContainsKey(tilePosition))
                {
                    bool alreadyCollected = false;
                    for(int n = 0; n < collectedTiles[tilePosition].Count; n++)
                    {
                        if (collectedTiles[tilePosition][n].tileType == tile.tileType && collectedTiles[tilePosition][n].playerMode == tile.playerMode)
                        {
                            alreadyCollected = true;
                        }
                    }
                    if(alreadyCollected)
                    {
                        continue;
                    }
                }

                    for (i = 0; i < collectables.Count; i++)
                {
                    if (collectables[i].playerMode == tile.playerMode && collectables[i].tileType == tile.tileType)
                    {
                        alreadyAdded = true;
                        collectables[i].count++;
                        break;
                    }
                }

                if(alreadyAdded)
                {
                    continue;
                }

                CollectableTile newCollectable = Instantiate(collectableTilePrefab).GetComponent<CollectableTile>();
                newCollectable.transform.position = tilePosition + positions[i];
                newCollectable.tileType = tile.tileType;
                newCollectable.count = 1;
                newCollectable.playerMode = tile.playerMode;
                newCollectable.tilePosition = tilePosition;
                newCollectable.tileBuilder = this;
                newCollectable.GetComponent<SpriteRenderer>().color = GameBehavior.instance.builderSettings.GetModeColor(tile.playerMode);

                newCollectable.GetComponent<SpriteRenderer>().sprite = General.GameBehavior.instance.builderSettings.GetTileTypeSprite(tile.tileType);

                collectables.Add(newCollectable);
            }

        }

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

            if (GameBehavior.instance.levelBuilder.levelData.GetGridSpot(GameBehavior.instance.levelBuilder.WorldPositionToCoordinate((Vector2)originPosition)).star)
            {
                GameObject star = Instantiate(builderSettings.starPrefab);

                star.transform.position = (Vector2)originPosition + (Vector2)tile.starPosition;
            }

        }

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

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, builderSettings.borderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth / 2 - 4, tileHeight - 1);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth / 2 + 4, tileHeight - 1);

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, null);
            }
            if (Physics2D.OverlapPoint(levelBuilder.CoordinateToWorldPosition(anchorCoordinate + Vector2Int.right), builderSettings.backgroundLayer) == null)
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 + 4);

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, builderSettings.borderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth - 1, tileHeight / 2 + 4);

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, null);
            }
            if (Physics2D.OverlapPoint(levelBuilder.CoordinateToWorldPosition(anchorCoordinate + Vector2Int.down), builderSettings.backgroundLayer) == null)
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth / 2 - 4, 0);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth / 2 + 4, 0);

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, builderSettings.borderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(tileWidth / 2 - 4, 0);
                Vector2Int endPos = tileOrigin + new Vector2Int(tileWidth / 2 + 4, 0);

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, null);
            }
            if (Physics2D.OverlapPoint(levelBuilder.CoordinateToWorldPosition(anchorCoordinate + Vector2Int.left), builderSettings.backgroundLayer) == null)
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(0, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(0, tileHeight / 2 + 4);

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, builderSettings.borderTile);
            }
            else
            {
                Vector2Int startPos = tileOrigin + new Vector2Int(0, tileHeight / 2 - 4);
                Vector2Int endPos = tileOrigin + new Vector2Int(0, tileHeight / 2 + 4);

                levelBuilder.DrawBox(groundTilemap, startPos, endPos, null);
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
        public Color tileColor;
    }
}

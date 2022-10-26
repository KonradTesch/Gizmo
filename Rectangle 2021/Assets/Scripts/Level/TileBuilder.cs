using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.TileCreation;
using Rectangle.Player;

namespace Rectangle.Level
{
    public class TileBuilder : MonoBehaviour
    {
        [SerializeField] private LevelTileData[] levelTiles;
        [SerializeField] private LevelTileData[] anchorTiles;
        [SerializeField] private LayerMask gridLayer;
        [SerializeField] private GameObject collectableTilePrefab;

        [Header("Tilemaps")]

        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap rampTilemap;


        private Dictionary<TileCreator.TileTypes, List<LevelTileData>> modeTiles;

        public enum Direction { None, Up, Right, Down, Left}

        public void BuildLevel(List<LevelTile> placedTiles, List<LevelTile> anchorLevelTiles)
        {
            foreach (LevelTile tile in placedTiles)
            {
                Debug.Log("build tile " + tile.name);
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

                LevelTileData randomTile = anchorTiles[Random.Range(0, anchorTiles.Length)];

                Physics2D.OverlapPoint(anchorTile.transform.position, gridLayer).GetComponent<BackgroundMode>().playerMode = randomTile.playerMode;

                anchorTile.gameObject.SetActive(false);

                BuildTile(Vector2Int.RoundToInt(originPosition), randomTile);

                if (anchorTile.collectableTiles != null)
                {
                    SpriteRenderer rend = anchorTile.GetComponent<SpriteRenderer>();

                    Vector2 origin = rend.bounds.min;

                    PlaceCollectableTiles(anchorTile.collectableTiles, randomTile.collectablePositions, origin);
                    anchorTile.collectableTiles = null;
                }
            }
        }

        public PlayerController.PlayerModes GetRandomPlayerMode(TileCreator.TileTypes tileType)
        {
            if(modeTiles == null)
            {
                InitModeTiles();
            }

            int randomIndex = Random.Range(0, modeTiles[tileType].Count);

            return modeTiles[tileType][randomIndex].playerMode;
        }

        private void PlaceCollectableTiles( List<TileCreator.TileTypes> collectableTiles, Vector2[] positions, Vector2 tilePosition)
        {
            Debug.Log("Place " + collectableTiles.Count + "Collectables.");
            Dictionary<TileCreator.TileTypes, int> collectables = new();
            foreach (TileCreator.TileTypes tileType in collectableTiles)
            {
                if (collectables.ContainsKey(tileType))
                {
                    collectables[tileType]++;
                }
                else
                {
                    collectables.Add(tileType, 1);
                }
            }


            int i = 0;
            foreach (KeyValuePair<TileCreator.TileTypes, int> collectable in collectables)
            {
                CollectableTile newCollectable = Instantiate(collectableTilePrefab).GetComponent<CollectableTile>();
                newCollectable.transform.position = tilePosition + positions[i];
                newCollectable.tileType = collectable.Key;
                newCollectable.count = collectable.Value;

                newCollectable.GetComponent<SpriteRenderer>().sprite = General.GameBehavior.instance.builderSettings.GetTileTypeSprite(collectable.Key);

                i++;
            }
        }

        private void InitModeTiles()
        {
            modeTiles = new()
            {
                { TileCreator.TileTypes.AllSides, new()},
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
                    case TileCreator.TileTypes.AllSides:
                        modeTiles[TileCreator.TileTypes.AllSides].Add(tile);
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
            foreach(ChangeData change in tile.groundTileChanges)
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

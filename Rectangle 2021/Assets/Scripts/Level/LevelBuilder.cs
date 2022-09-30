using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.TileBuilder;

namespace Rectangle.Level
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField] private LevelTileData[] levelTiles;

        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap rampTilemap;
        [SerializeField] private Tilemap nookTilemap;

        private Dictionary<LevelTileBuilder.TileTypes, List<LevelTileData>> modeTiles;

        void Start()
        {
            General.GameBehavior.instance.levelBuilder = this;

            modeTiles = new()
            {
                {LevelTileBuilder.TileTypes.AllSides, new()},
                {LevelTileBuilder.TileTypes.LeftAndRight, new()},
                {LevelTileBuilder.TileTypes.LeftAndUp, new()},
                {LevelTileBuilder.TileTypes.LeftAndDown, new()},
                {LevelTileBuilder.TileTypes.UpAndRight, new()},
                {LevelTileBuilder.TileTypes.DownAndRight, new()},
                {LevelTileBuilder.TileTypes.UpAndDown, new()}

            };

            foreach(LevelTileData tile in levelTiles)
            {
                switch(tile.tileType)
                {
                    case LevelTileBuilder.TileTypes.AllSides:
                        modeTiles[LevelTileBuilder.TileTypes.AllSides].Add(tile);
                        break;
                    case LevelTileBuilder.TileTypes.LeftAndRight:
                        modeTiles[LevelTileBuilder.TileTypes.LeftAndRight].Add(tile);
                        break;
                    case LevelTileBuilder.TileTypes.LeftAndUp:
                        modeTiles[LevelTileBuilder.TileTypes.LeftAndUp].Add(tile);
                        break;
                    case LevelTileBuilder.TileTypes.LeftAndDown:
                        modeTiles[LevelTileBuilder.TileTypes.LeftAndDown].Add(tile);
                        break;
                    case LevelTileBuilder.TileTypes.UpAndRight:
                        modeTiles[LevelTileBuilder.TileTypes.UpAndRight].Add(tile);
                        break;
                    case LevelTileBuilder.TileTypes.DownAndRight:
                        modeTiles[LevelTileBuilder.TileTypes.DownAndRight].Add(tile);
                        break;
                    case LevelTileBuilder.TileTypes.UpAndDown:
                        modeTiles[LevelTileBuilder.TileTypes.UpAndDown].Add(tile);
                        break;
                }
            }
        }

        public void BuildLevel(LevelTile[] tiles)
        {
            foreach(LevelTile tile in tiles)
            {
                Vector2 originPosition = ((Vector2)tile.transform.position / groundTilemap.transform.lossyScale) - new Vector2(tile.tileSize.x / 2, tile.tileSize.y / 2);

                int randomIndex = Random.Range(0, modeTiles[tile.tileType].Count);

                BuildTile(Vector2Int.RoundToInt(originPosition), modeTiles[tile.tileType][randomIndex]);
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

            foreach (ChangeData change in tile.nookTileChanges)
            {
                TileChangeData tileChange = new()
                {
                    position = change.position + (Vector3Int)originPosition,
                    tile = change.tile,
                    transform = change.transform
                };
                nookTilemap.SetTile(tileChange, true);
            }
            nookTilemap.RefreshAllTiles();
        }
    }
}

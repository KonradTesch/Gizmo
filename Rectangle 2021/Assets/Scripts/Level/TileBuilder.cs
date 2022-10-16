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
        [SerializeField] private LayerMask backgroundLayer;

        [Header("Tilemaps")]

        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap rampTilemap;


        private Dictionary<TileCreator.TileTypes, List<LevelTileData>> modeTiles;

        public enum Direction { None, Up, Right, Down, Left}

        public void BuildLevel(List<LevelTile> placedTiles)
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
        }

        public PlayerController.PlayerModes GetPlayerMode(TileCreator.TileTypes tileType)
        {
            if(modeTiles == null)
            {
                InitModeTiles();
            }

            int randomIndex = Random.Range(0, modeTiles[tileType].Count);

            return modeTiles[tileType][randomIndex].playerMode;
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
}

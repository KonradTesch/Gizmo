using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.TileCreater;

namespace Rectangle.Level
{
    public class TileBuilder : MonoBehaviour
    {
        [SerializeField] private LevelTileData[] levelTiles;
        [SerializeField] private LayerMask backgroundLayer;

        [Header("Tilemaps")]

        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap rampTilemap;
        [SerializeField] private Tilemap nookTilemap;

        [Header("Start")]
        [SerializeField] private Transform startPosition;
        [SerializeField] private Direction startDirection;

        [Header("Destination")]
        [SerializeField] private Transform destinationPosition;
        [SerializeField] private Direction destinationDirection;

        private List<LevelTile> placedTiles;

        private Dictionary<TileCreater.TileCreater.TileTypes, List<LevelTileData>> modeTiles;

        public enum Direction { None, Up, Right, Down, Left}
        void Start()
        {
            General.GameBehavior.instance.levelBuilder = this;

            modeTiles = new()
            {
                { TileCreater.TileCreater.TileTypes.AllSides, new()},
                { TileCreater.TileCreater.TileTypes.LeftAndRight, new()},
                { TileCreater.TileCreater.TileTypes.LeftAndUp, new()},
                { TileCreater.TileCreater.TileTypes.LeftAndDown, new()},
                { TileCreater.TileCreater.TileTypes.UpAndRight, new()},
                { TileCreater.TileCreater.TileTypes.DownAndRight, new()},
                { TileCreater.TileCreater.TileTypes.UpAndDown, new()}

            };

            foreach(LevelTileData tile in levelTiles)
            {
                switch(tile.tileType)
                {
                    case TileCreater.TileCreater.TileTypes.AllSides:
                        modeTiles[TileCreater.TileCreater.TileTypes.AllSides].Add(tile);
                        break;
                    case TileCreater.TileCreater.TileTypes.LeftAndRight:
                        modeTiles[TileCreater.TileCreater.TileTypes.LeftAndRight].Add(tile);
                        break;
                    case TileCreater.TileCreater.TileTypes.LeftAndUp:
                        modeTiles[TileCreater.TileCreater.TileTypes.LeftAndUp].Add(tile);
                        break;
                    case TileCreater.TileCreater.TileTypes.LeftAndDown:
                        modeTiles[TileCreater.TileCreater.TileTypes.LeftAndDown].Add(tile);
                        break;
                    case TileCreater.TileCreater.TileTypes.UpAndRight:
                        modeTiles[TileCreater.TileCreater.TileTypes.UpAndRight].Add(tile);
                        break;
                    case TileCreater.TileCreater.TileTypes.DownAndRight:
                        modeTiles[TileCreater.TileCreater.TileTypes.DownAndRight].Add(tile);
                        break;
                    case TileCreater.TileCreater.TileTypes.UpAndDown:
                        modeTiles[TileCreater.TileCreater.TileTypes.UpAndDown].Add(tile);
                        break;
                }
            }
        }

        public void BuildLevel()
        {
            foreach (LevelTile tile in placedTiles)
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

        public bool CheckLevel()
        {
            Direction currentDirection = startDirection;
            LevelTile currentTile;

            if (Physics2D.OverlapPoint(startPosition.position, backgroundLayer) != null ? Physics2D.OverlapPoint(startPosition.position, backgroundLayer).GetComponent<LevelTile>() !=  null : false)
            {
                currentTile = Physics2D.OverlapPoint(startPosition.position, backgroundLayer).GetComponent<LevelTile>();
            }
            else
            {
                Debug.Log("return with 0");
                return false;
            }

            placedTiles = new();

            while (CheckDirection(currentDirection, currentTile.tileType) != Vector2.zero)
            {
                placedTiles.Add(currentTile);

                if (currentTile.transform.position == destinationPosition.position)
                {
                    Debug.Log("return with success");
                    return true;
                }

                Vector2 direction = CheckDirection(currentDirection, currentTile.tileType);
                Vector2 size = currentTile.GetComponent<BoxCollider2D>().size * currentTile.transform.lossyScale;
                Vector2 nextPosition = (Vector2)currentTile.transform.position + (direction * size);

                if (Physics2D.OverlapPoint(nextPosition, backgroundLayer) != null ? Physics2D.OverlapPoint(nextPosition, backgroundLayer).TryGetComponent(out LevelTile _) : false)
                {
                    currentTile = Physics2D.OverlapPoint(nextPosition, backgroundLayer).GetComponent<LevelTile>();
                }
                else
                {
                    Debug.Log("Return with " + placedTiles.Count);
                    return false;
                }

                if(direction == Vector2.up)
                {
                    currentDirection = Direction.Up;
                }
                else if(direction == Vector2.right)
                {
                    currentDirection = Direction.Right;
                }
                else if (direction == Vector2.down)
                {
                    currentDirection = Direction.Down;
                }
                else if (direction == Vector2.left)
                {
                    currentDirection = Direction.Left;
                }


            }
            return false;
        }

        private Vector2 CheckDirection(Direction entry, TileCreater.TileCreater.TileTypes tileType)
        {
            switch (tileType)
            {
                case TileCreater.TileCreater.TileTypes.LeftAndRight:
                    if (entry == Direction.Left)
                        return Vector2.left;
                    else if (entry == Direction.Right)
                        return Vector2.right;
                    break;

                case TileCreater.TileCreater.TileTypes.LeftAndDown:
                    if (entry == Direction.Up)
                        return Vector2.left;
                    else if(entry == Direction.Right)
                        return Vector2.down;
                    break;

                case TileCreater.TileCreater.TileTypes.LeftAndUp:
                    if (entry == Direction.Down)
                        return Vector2.left;
                    else if(entry == Direction.Right)
                        return Vector2.up;
                    break;

                case TileCreater.TileCreater.TileTypes.UpAndRight:
                    if (entry == Direction.Left)
                        return Vector2.up;
                    else if(entry == Direction.Down)
                        return Vector2.right;
                    break;

                case TileCreater.TileCreater.TileTypes.DownAndRight:
                    if (entry == Direction.Left)
                        return Vector2.down;
                    else if (entry == Direction.Up)
                        return Vector2.right;
                    break;

                case TileCreater.TileCreater.TileTypes.UpAndDown:
                    if (entry == Direction.Up)
                        return Vector2.up;
                    else if (entry == Direction.Down)
                        return Vector2.down;
                    break;
            }

            return Vector2.zero; ;
        }
    }
}

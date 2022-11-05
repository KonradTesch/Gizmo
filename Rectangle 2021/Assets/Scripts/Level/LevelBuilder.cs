using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.TileCreation;

namespace Rectangle.Level
{
    public class LevelBuilder : MonoBehaviour
    {
        [Header("Builder Data")]
        [SerializeField] private Texture2D[] levelLayouts;
        [SerializeField] private LevelBuilderSettings builderSettings;

        [Header("Tilemaps")]
        [SerializeField] private Tilemap gridTilemap;
        [SerializeField] private Tilemap borderTilemap;

        [Header("LevelText")]
        [SerializeField] private GameObject startText;
        [SerializeField] private GameObject endText;
        [SerializeField] private GameObject anchorTextPrefab;

        [HideInInspector] public List<LevelTile> placedTiles;
        [HideInInspector] public List<LevelTile> anchorTiles = new();

        private LevelGrid gridData;
        private Vector2Int startDirection;
        private Vector2Int endDirection;
        private List<Vector2Int> blockedPositions;

        public void BuildLevel()
        {
            DestroyImmediate(GameObject.Find("GridColliders"));

            gridData = GetLevelGrid();

            GameObject gridColliders = new GameObject("GridColliders");

            GameObject gridCollider = new GameObject("GridCollider");

            BoxCollider2D col = gridCollider.AddComponent<BoxCollider2D>();
            col.size = Vector2.one * builderSettings.tileSize * gridTilemap.transform.lossyScale;
            col.isTrigger = true;

            gridCollider.AddComponent<GridField>();

            anchorTiles.Clear();

            gridTilemap.ClearAllTiles();
            borderTilemap.ClearAllTiles();

            for (int y = 0; y < gridData.height; y++)
            {
                for (int x = 0; x < gridData.width; x++)
                {
                    if (gridData.grid[new Vector2Int(x, y)])
                    {
                        foreach (ChangeData change in builderSettings.gridTiles.groundTileChanges)
                        {
                            TileChangeData tileChange = new()
                            {
                                tile = change.tile,
                                position = change.position + (new Vector3Int(x, y, 0) * builderSettings.tileSize),
                                transform = change.transform
                            };

                            gridTilemap.SetTile(tileChange, true);
                        }

                        GameObject newGridCol = Instantiate(gridCollider, new Vector2(x + 0.5f, y + 0.5f) * builderSettings.tileSize * gridTilemap.transform.lossyScale, Quaternion.identity, gridColliders.transform);
                        newGridCol.layer = LayerMask.NameToLayer("Grid");
                        SpriteRenderer backgroundRend = newGridCol.AddComponent<SpriteRenderer>();
                        backgroundRend.sortingLayerName = "Background";
                        backgroundRend.sprite = builderSettings.backgroundImage;
                        backgroundRend.sortingOrder = -1;

                        backgroundRend.color = Color.grey;

                        newGridCol.AddComponent<BackgroundMode>();

                        if (gridData.anchorTiles.Contains(new Vector2Int(x, y)))
                        {
                            LevelTile anchorTile = new GameObject("Anchor_Tile").AddComponent<LevelTile>();
                            anchorTile.transform.position = new Vector2(x + 0.5f, y + 0.5f) * builderSettings.tileSize * gridTilemap.transform.lossyScale;
                            anchorTile.gameObject.layer = LayerMask.NameToLayer("Background");

                            anchorTile.tileType = TileCreator.TileTypes.AllSides;

                            SpriteRenderer tileRend = anchorTile.gameObject.AddComponent<SpriteRenderer>();
                            tileRend.sortingLayerName = "Level";
                            tileRend.sprite = builderSettings.anchorTileSprite;

                            anchorTiles.Add(anchorTile);
                        }

                    }
                    else
                    {
                        DrawBox(borderTilemap, new Vector2Int(x, y) * builderSettings.tileSize, new Vector2Int((x + 1) * builderSettings.tileSize - 1, (y + 1) * builderSettings.tileSize - 1), builderSettings.borderTile);
                    }
                }
            }

            DrawBorder(gridData);

            startDirection = GetStartDirection();
            Vector2 startPos = DrawStartOrEnd(gridData.start, startDirection);

            startText.transform.position = startPos;
            General.GameBehavior.instance.player.transform.position = startPos;

            DestroyImmediate(GameObject.Find("Start_Collider"));
            GameObject startCollider = new GameObject("Start_Collider");
            startCollider.gameObject.layer = LayerMask.NameToLayer("Grid");
            startCollider.transform.position = startPos;
            startCollider.AddComponent<BoxCollider2D>().isTrigger = true;
            startCollider.GetComponent<BoxCollider2D>().size = new Vector2(2, 2);
            startCollider.AddComponent<BackgroundMode>().playerMode = Player.PlayerController.PlayerModes.Rectangle;

            endDirection = GetEndDirection();
            Vector2 endPos = DrawStartOrEnd(gridData.end, endDirection);

            endText.transform.position = endPos;

            DestroyImmediate(GameObject.Find("Success_Collider"));
            GameObject endCollider = new GameObject("Success_Collider");
            endCollider.gameObject.layer = LayerMask.NameToLayer("Grid");
            endCollider.transform.position = endPos;
            endCollider.AddComponent<BoxCollider2D>().isTrigger = true;
            endCollider.GetComponent<BoxCollider2D>().size = new Vector2(2, 2);
            endCollider.AddComponent<BackgroundMode>().playerMode = Player.PlayerController.PlayerModes.Rectangle;
            SuccessTrigger success = endCollider.AddComponent<SuccessTrigger>();
            success.successPanel = General.GameBehavior.instance.sucessPanel;
            success.timerUI = FindObjectOfType<UI.TimerUI>();


            gridTilemap.RefreshAllTiles();
            borderTilemap.RefreshAllTiles();

            List<TileCreator.TileTypes> tileTypes = new();

            for(int i = 0; i < anchorTiles.Count; i++)
            {
                tileTypes.AddRange(CalculatePathToAnchor(gridData.anchorTiles[i]));
                anchorTiles[i].collectableTiles = new(CalculatePathToEnd(gridData.anchorTiles[i]));

                List<Player.PlayerController.PlayerModes> tileModes = new();

                for(int n = 0; n < anchorTiles[i].collectableTiles.Count; n++)
                {
                    tileModes.Add(General.GameBehavior.instance.CheckTileInventoryModes(anchorTiles[i].collectableTiles[n]));
                }

                Instantiate(anchorTextPrefab, anchorTiles[i].transform.position, Quaternion.identity).GetComponent<UI.AnchorText>().SetTileNumbers(tileModes);
            }

            General.GameBehavior.instance.InitLevelTiles(tileTypes);

            DestroyImmediate(gridCollider);


        }

        public bool CheckLevelPath(Vector2Int startPosition)
        {
            placedTiles.Clear();

            Vector2Int currentDirection = Vector2Int.zero;
            Vector2Int currentPosition = startPosition;

            if (Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.up), builderSettings.backgroundLayer) != null)
            {
                if(Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.up), builderSettings.backgroundLayer).TryGetComponent(out LevelTile tile) && !tile.locked && GetNextDirection(tile.tileType, Vector2Int.up) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.up;
                }
            }
            if (Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.right), builderSettings.backgroundLayer) != null)
            {
                if (Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.right), builderSettings.backgroundLayer).TryGetComponent(out LevelTile tile) && !tile.locked && GetNextDirection(tile.tileType, Vector2Int.right) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.right;
                }
            }
            if (Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.down), builderSettings.backgroundLayer) != null)
            {
                if (Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.down), builderSettings.backgroundLayer).TryGetComponent(out LevelTile tile) && !tile.locked && GetNextDirection(tile.tileType, Vector2Int.down) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.down;
                }
            }
            if (Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.left), builderSettings.backgroundLayer) != null)
            {
                if (Physics2D.OverlapPoint(CoordinateToWorldPosition(startPosition + Vector2Int.left), builderSettings.backgroundLayer).TryGetComponent(out LevelTile tile) && !tile.locked && GetNextDirection(tile.tileType, Vector2Int.left) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.left;
                }
            }

            if(currentDirection == Vector2Int.zero)
            {
                return false;
            }


            do
            {
                currentPosition += currentDirection;

                if (Physics2D.OverlapPoint(((Vector2)currentPosition + new Vector2(0.5f, 0.5f)) * builderSettings.tileSize * gridTilemap.transform.lossyScale.x, builderSettings.backgroundLayer) == null)
                {
                    return false;
                }
                else
                {
                    LevelTile tile = Physics2D.OverlapPoint(CoordinateToWorldPosition(currentPosition), builderSettings.backgroundLayer).GetComponent<LevelTile>();

                    if(!placedTiles.Contains(tile))
                    {
                        placedTiles.Add(tile);
                    }

                    currentDirection = GetNextDirection(tile.tileType, currentDirection);
                    if(currentDirection == Vector2Int.zero)
                    {
                        return false;
                    }

                }
            } while (!(currentPosition + currentDirection == gridData.end) && !(gridData.anchorTiles.Contains(currentPosition + currentDirection)));

            return true;
        }
        private LevelGrid GetLevelGrid()
        {
            LevelGrid gridData = new();

            gridData.grid = new Dictionary<Vector2Int, bool>();
            gridData.anchorTiles = new();

            Texture2D levelLayout = levelLayouts[Random.Range(0, levelLayouts.Length)];

            gridData.width = levelLayout.width - 2;
            gridData.height = levelLayout.height - 2;

            for (int y = 1; y < levelLayout.height - 1; y++)
            {
                for (int x = 1; x < levelLayout.width - 1; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y) - Vector2Int.one;

                    if (levelLayout.GetPixel(x, y) == Color.white)
                    {
                        gridData.grid.Add(gridPos, true);
                    }
                    else
                    {
                        gridData.grid.Add(gridPos, false);
                    }

                }
            }

            for (int y = 0; y < levelLayout.height; y++)
            {
                for (int x = 0; x < levelLayout.width; x++)
                {
                    if (levelLayout.GetPixel(x, y) == Color.green)
                    {
                        gridData.start = new Vector2Int(x, y) - Vector2Int.one;
                    }
                    else if (levelLayout.GetPixel(x, y) == Color.blue)
                    {
                        gridData.end = new Vector2Int(x, y) - Vector2Int.one;
                    }
                    else if(levelLayout.GetPixel(x, y) == Color.cyan)
                    {
                        gridData.anchorTiles.Add(new Vector2Int(x, y) - Vector2Int.one);
                        gridData.grid[new Vector2Int(x, y) - Vector2Int.one] = true;
                    }
                }
            }

            return gridData;
        }

        private void DrawBorder(LevelGrid gridData)
        {
            DrawBox(borderTilemap, new Vector2Int(-2, builderSettings.tileSize * gridData.height), new Vector2Int(builderSettings.tileSize * gridData.width - 1, builderSettings.tileSize * gridData.height + 1), builderSettings.borderTile);
            DrawBox(borderTilemap, new Vector2Int(builderSettings.tileSize * gridData.width, builderSettings.tileSize * gridData.height + 1), new Vector2Int(builderSettings.tileSize * gridData.width + 1, 0), builderSettings.borderTile);
            DrawBox(borderTilemap, new Vector2Int(0, -1), new Vector2Int(builderSettings.tileSize * gridData.width + 1, -2), builderSettings.borderTile);
            DrawBox(borderTilemap, new Vector2Int(-1, -2), new Vector2Int(-2, builderSettings.tileSize * gridData.height - 1), builderSettings.borderTile);
        }

        private Vector2 DrawStartOrEnd(Vector2Int position, Vector2Int direction)
        {

            int size = builderSettings.tileSize;

            if (direction == Vector2Int.right)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size + (size - 6), position.y * size + (size / 2 - 4)), new Vector2Int(position.x * size + (size - 1), position.y * size + (size / 2 + 3)), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size + (size - 4), position.y * size + (size / 2 - 2)), new Vector2Int(position.x * size + (size - 1), position.y * size + (size / 2 + 1)), null);

                return new Vector2((position.x + 1) * size * borderTilemap.transform.lossyScale.x - (2 * borderTilemap.transform.lossyScale.x), (position.y + 0.5f) * size * borderTilemap.transform.lossyScale.y);
            }
            else if (direction == Vector2Int.left)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size, position.y * size + (size / 2 - 4)), new Vector2Int(position.x * size + 5, position.y * size + (size / 2 + 3)), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size, position.y * size + (size / 2 - 2)), new Vector2Int(position.x * size + 3, position.y * size + (size / 2 + 1)), null);

                return new Vector2(position.x * size * borderTilemap.transform.lossyScale.x + (2 * borderTilemap.transform.lossyScale.x), (position.y + 0.5f) * size * borderTilemap.transform.lossyScale.y);
            }
            else if (direction == Vector2Int.down)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size + (size / 2 - 4), position.y * size), new Vector2Int(position.x * size + (size / 2 + 3), position.y * size + 5), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size + (size / 2 - 2), position.y * size), new Vector2Int(position.x * size + (size / 2 + 1), position.y * size + 3), null);

                return new Vector2((position.x + 0.5f) * size * borderTilemap.transform.lossyScale.x, position.y * size * borderTilemap.transform.lossyScale.y + (2 * borderTilemap.transform.lossyScale.y));

            }
            else if (direction == Vector2Int.up)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size + (size / 2 - 4), position.y * size + (size - 6)), new Vector2Int(position.x * size + (size / 2 + 3), position.y * size + (size - 1)), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size + (size / 2 - 2), position.y * size + (size - 4)), new Vector2Int(position.x * size + (size / 2 + 1), position.y * size + (size - 1)), null);

                return new Vector2((position.x + 0.5f) * size * borderTilemap.transform.lossyScale.x, (position.y + 1) * size * borderTilemap.transform.lossyScale.y - (2 * borderTilemap.transform.lossyScale.y))
;
            }

            return Vector2.zero;

        }

        private Vector2Int GetStartDirection()
        {
            Vector2Int direction = Vector2Int.zero;

            if (gridData.grid.ContainsKey(gridData.start + Vector2Int.right) ? gridData.grid[gridData.start + Vector2Int.right] : false)
            {
                direction = Vector2Int.right;
            }
            else if (gridData.grid.ContainsKey(gridData.start + Vector2Int.left) ? gridData.grid[gridData.start + Vector2Int.left] : false)
            {
                direction = Vector2Int.left;
            }
            else if (gridData.grid.ContainsKey(gridData.start + Vector2Int.down) ? gridData.grid[gridData.start + Vector2Int.down] : false)
            {
                direction = Vector2Int.down;
            }
            else if (gridData.grid.ContainsKey(gridData.start + Vector2Int.up) ? gridData.grid[gridData.start + Vector2Int.up] : false)
            {
                direction = Vector2Int.up;
            }

            return direction;
        }

        private Vector2Int GetEndDirection()
        {
            Vector2Int direction = Vector2Int.zero;

            if (gridData.grid.ContainsKey(gridData.end + Vector2Int.left) ? gridData.grid[gridData.end + Vector2Int.left] : false)
            {
                direction = Vector2Int.left;
            }
            else if (gridData.grid.ContainsKey(gridData.end + Vector2Int.right) ? gridData.grid[gridData.end + Vector2Int.right] : false)
            {
                direction = Vector2Int.right;
            }
            else if (gridData.grid.ContainsKey(gridData.end + Vector2Int.up) ? gridData.grid[gridData.end + Vector2Int.up] : false)
            {
                direction = Vector2Int.up;
            }
            else if (gridData.grid.ContainsKey(gridData.end + Vector2Int.down) ? gridData.grid[gridData.end + Vector2Int.down] : false)
            {
                direction = Vector2Int.down;
            }


            return direction;
        }

        private List<TileCreator.TileTypes> CalculatePathToAnchor(Vector2Int anchorPosition)
        {
            blockedPositions = new();
            List<Vector2Int> pathPositions = new();
            List<TileCreator.TileTypes> pathTiles = new();


            Vector2Int currentDirection = startDirection;
            Vector2Int currentPosition = gridData.start + startDirection;
            pathPositions.Add(currentPosition);
            blockedPositions.Add(currentPosition);
            do
            {
                List<Vector2Int> freeDirections = GetFreeDirections(currentPosition, gridData.start.x, anchorPosition.x);
                if (freeDirections.Count == 0 || gridData.anchorTiles.Contains(currentPosition))
                {
                    blockedPositions.Add(currentPosition);

                    pathTiles.RemoveAt(pathTiles.Count - 1);
                    pathPositions.Remove(currentPosition);
                    currentPosition = pathPositions[pathPositions.Count - 1];

                    if (pathPositions.Count - 2 < 0)
                    {
                        currentDirection = currentPosition - gridData.start;
                    }
                    else
                    {
                        currentDirection = currentPosition - pathPositions[pathPositions.Count - 2];
                    }

                    continue;
                }
                Vector2Int lastDirection = currentDirection;

                currentDirection = freeDirections[Random.Range(0, freeDirections.Count)];
                currentPosition += currentDirection;

                blockedPositions.Add(currentPosition);
                pathPositions.Add(currentPosition);
                pathTiles.Add(GetTileType(lastDirection, currentDirection));

            } while (currentPosition != anchorPosition);

            return pathTiles;
        }

        private List<TileCreator.TileTypes> CalculatePathToEnd(Vector2Int start)
        {
            blockedPositions = new();
            List<Vector2Int> pathPositions = new();
            List<TileCreator.TileTypes> pathTiles = new();

            gridData.grid[start] = false;

            List<Vector2Int> freeDirections = GetFreeDirections(start, start.x, gridData.end.x);

            int randomIndex = Random.Range(0, freeDirections.Count);

            Vector2Int currentDirection = freeDirections[randomIndex];

            Vector2Int currentPosition = start + currentDirection;
            pathPositions.Add(start);
            pathPositions.Add(currentPosition);
            blockedPositions.Add(currentPosition);
            do
            {
                freeDirections = GetFreeDirections(currentPosition, start.x, gridData.end.x);
                if (freeDirections.Count == 0 || gridData.anchorTiles.Contains(currentPosition))
                {

                    pathTiles.RemoveAt(pathTiles.Count - 1);
                    pathPositions.Remove(currentPosition);
                    currentPosition = pathPositions[pathPositions.Count - 1];

                    if (pathPositions.Count - 2 < 0)
                    {
                        currentDirection = currentPosition - gridData.start;
                    }
                    else
                    {
                        currentDirection = currentPosition - pathPositions[pathPositions.Count - 2];
                    }

                    continue;
                }
                Vector2Int lastDirection = currentDirection;

                currentDirection = freeDirections[Random.Range(0, freeDirections.Count)];
                currentPosition += currentDirection;

                blockedPositions.Add(currentPosition);
                pathPositions.Add(currentPosition);
                pathTiles.Add(GetTileType(lastDirection, currentDirection));

            } while (currentPosition - endDirection != gridData.end);

            pathTiles.Add(GetTileType(currentDirection, -endDirection));
            return pathTiles;
        }
        private List<Vector2Int> GetFreeDirections(Vector2Int currentPos, int startX, int endX)
        {
            List<Vector2Int> freeDirections = new();

            int minX;
            int maxX;

            if(startX > endX)
            {
                minX = endX;
                maxX = startX;
            }
            else
            {
                minX = startX;
                maxX = endX;
            }

            Vector2Int checkPosition;

            checkPosition = currentPos + Vector2Int.up;

            if (gridData.grid.ContainsKey(checkPosition))
            {
                if (gridData.grid[checkPosition] && !blockedPositions.Contains(checkPosition))
                {
                    freeDirections.Add(Vector2Int.up);
                }
            }

            checkPosition = currentPos + Vector2Int.right;
            if (gridData.grid.ContainsKey(checkPosition) && currentPos.x + 1 <= maxX)
            {
                if (gridData.grid[checkPosition] && !blockedPositions.Contains(checkPosition))
                {
                    freeDirections.Add(Vector2Int.right);
                }
            }

            checkPosition = currentPos + Vector2Int.down;
            if (gridData.grid.ContainsKey(checkPosition))
            {
                if (gridData.grid[checkPosition] && !blockedPositions.Contains(checkPosition))
                {
                    freeDirections.Add(Vector2Int.down);
                }
            }

            checkPosition = currentPos + Vector2Int.left;
            if (gridData.grid.ContainsKey(checkPosition) && currentPos.x - 1 >= minX)
            {
                if (gridData.grid[checkPosition] && !blockedPositions.Contains(checkPosition))
                {
                    freeDirections.Add(Vector2Int.left);
                }
            }

            return freeDirections;
        }

        private TileCreator.TileTypes GetTileType(Vector2Int input, Vector2Int output)
        {
            TileCreator.TileTypes tileType = TileCreator.TileTypes.undefined;


            if (input == Vector2Int.up)
            {
                if (output == Vector2Int.right)
                {
                    return TileCreator.TileTypes.DownAndRight;
                }
                else if (output == Vector2Int.up)
                {
                    return TileCreator.TileTypes.UpAndDown;
                }
                else if (output == Vector2Int.left)
                {
                    return TileCreator.TileTypes.LeftAndDown;
                }

            }
            else if (input == Vector2Int.right)
            {
                if (output == Vector2Int.up)
                {
                    return TileCreator.TileTypes.LeftAndUp;
                }
                else if (output == Vector2Int.down)
                {
                    return TileCreator.TileTypes.LeftAndDown;
                }
                else if (output == Vector2Int.right)
                {
                    return TileCreator.TileTypes.LeftAndRight;
                }
            }
            else if (input == Vector2Int.down)
            {
                if (output == Vector2Int.down)
                {
                    return TileCreator.TileTypes.UpAndDown;
                }
                else if (output == Vector2Int.right)
                {
                    return TileCreator.TileTypes.UpAndRight;
                }
                else if (output == Vector2Int.left)
                {
                    return TileCreator.TileTypes.LeftAndUp;
                }
            }
            else if (input == Vector2Int.left)
            {
                if (output == Vector2Int.up)
                {
                    return TileCreator.TileTypes.UpAndRight;
                }
                else if (output == Vector2Int.left)
                {
                    return TileCreator.TileTypes.LeftAndRight;
                }
                else if (output == Vector2Int.down)
                {
                    return TileCreator.TileTypes.DownAndRight;
                }
            }

            return tileType;
        }

        public void DrawBox(Tilemap tilemap, Vector2Int pos1, Vector2Int pos2, TileBase tile)
        {
            Vector2 diff = pos2 - pos1;

            for (int y = 0; y <= Mathf.Abs(diff.y); y++)
            {
                for (int x = 0; x <= Mathf.Abs(diff.x); x++)
                {
                    if (diff.x < 0 && diff.y < 0)
                    {
                        tilemap.SetTile((Vector3Int)pos1 - new Vector3Int(x, y, 0), tile);
                    }
                    else if (diff.x < 0)
                    {
                        tilemap.SetTile((Vector3Int)pos1 + new Vector3Int(-x, y, 0), tile);
                    }
                    else if (diff.y < 0)
                    {
                        tilemap.SetTile((Vector3Int)pos1 + new Vector3Int(x, -y, 0), tile);
                    }
                    else
                    {
                        tilemap.SetTile((Vector3Int)pos1 + new Vector3Int(x, y, 0), tile);
                    }
                }
            }
        }

        private Vector2Int GetNextDirection(TileCreator.TileTypes tileType, Vector2Int input)
        {
            switch(tileType)
            {
                case TileCreator.TileTypes.LeftAndRight:
                    if(input == Vector2.left)
                    {
                        return Vector2Int.left;
                    }
                    else if(input == Vector2Int.right)
                    {
                        return Vector2Int.right;
                    }
                    break;
                case TileCreator.TileTypes.LeftAndDown:
                    if (input == Vector2.right)
                    {
                        return Vector2Int.down;
                    }
                    else if (input == Vector2Int.up)
                    {
                        return Vector2Int.left;
                    }
                    break;
                case TileCreator.TileTypes.LeftAndUp:
                    if (input == Vector2.right)
                    {
                        return Vector2Int.up;
                    }
                    else if (input == Vector2Int.down)
                    {
                        return Vector2Int.right;
                    }
                    break;
                case TileCreator.TileTypes.DownAndRight:
                    if (input == Vector2.left)
                    {
                        return Vector2Int.down;
                    }
                    else if (input == Vector2Int.up)
                    {
                        return Vector2Int.right;
                    }
                    break;
                case TileCreator.TileTypes.UpAndRight:
                    if (input == Vector2.left)
                    {
                        return Vector2Int.up;
                    }
                    else if (input == Vector2Int.down)
                    {
                        return Vector2Int.right;
                    }
                    break;
                case TileCreator.TileTypes.UpAndDown:
                    if (input == Vector2.down)
                    {
                        return Vector2Int.down;
                    }
                    else if (input == Vector2Int.up)
                    {
                        return Vector2Int.up;
                    }
                    break;

            }

            return Vector2Int.zero;
        }

        public Vector2 CoordinateToWorldPosition(Vector2Int coordinate)
        {
            return ((Vector2)coordinate + new Vector2(0.5f, 0.5f)) * builderSettings.tileSize * gridTilemap.transform.lossyScale.x;
        }

        public Vector2Int WorldPositionToCoordinate(Vector2 position)
        {
            return Vector2Int.FloorToInt(position / builderSettings.tileSize / gridTilemap.transform.lossyScale.x);
        }
    }

    public class LevelGrid
    {
        public Dictionary<Vector2Int, bool> grid;
        public List<Vector2Int> anchorTiles;
        public int height;
        public int width;
        public Vector2Int start;
        public Vector2Int end;
    }

}
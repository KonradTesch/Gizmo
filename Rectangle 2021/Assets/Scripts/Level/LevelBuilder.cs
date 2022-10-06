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

        /// <summary>
        /// The UI panel with the tiles.
        /// </summary>
        [Tooltip("The UI panel with the tiles.")]
        [SerializeField] private UI.TilePanel tilePanel;

        [HideInInspector] public List<LevelTile> placedTiles;

        private LevelGrid gridData;
        private Vector2Int startDirection;
        private Vector2Int endDirection;


        public void BuildLevel()
        {
            DestroyImmediate(GameObject.Find("GridColliders"));

            gridData = GetLevelGrid();

            GameObject gridColliders = new GameObject("GridColliders");

            GameObject gridCollider = new GameObject("GridCollider");

            BoxCollider2D col = gridCollider.AddComponent<BoxCollider2D>();
            col.size = Vector2.one * builderSettings.tileSize * gridTilemap.transform.lossyScale;
            col.isTrigger = true;

            gridCollider.AddComponent<IsGridUsed>();

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
                        newGridCol.gameObject.layer = LayerMask.NameToLayer("Grid");
                        SpriteRenderer backgroundRend = newGridCol.AddComponent<SpriteRenderer>();
                        backgroundRend.sortingLayerName = "Background";
                        backgroundRend.sprite = builderSettings.backgroundImage;

                        int randomModeIndex = Random.Range(1, 2);
                        newGridCol.AddComponent<BackgroundMode>().playerMode = (Player.ModeController.PlayerModes)randomModeIndex;

                        switch (randomModeIndex)
                        {
                            case 1:
                                backgroundRend.color = builderSettings.rectangleColor;
                                break;
                            case 2:
                                backgroundRend.color = builderSettings.bubbleColor;
                                break;
                            case 3:
                                backgroundRend.color = builderSettings.spikeyColor;
                                break;
                            case 4:
                                backgroundRend.color = builderSettings.littleColor;
                                break;
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
            startCollider.AddComponent<BackgroundMode>().playerMode = Player.ModeController.PlayerModes.Rectangle;

            endDirection = GetEndDirection();
            Vector2 endPos = DrawStartOrEnd(gridData.end, endDirection);

            endText.transform.position = endPos;

            DestroyImmediate(GameObject.Find("Success_Collider"));
            GameObject endCollider = new GameObject("Success_Collider");
            endCollider.gameObject.layer = LayerMask.NameToLayer("Grid");
            endCollider.transform.position = endPos;
            endCollider.AddComponent<BoxCollider2D>().isTrigger = true;
            endCollider.GetComponent<BoxCollider2D>().size = new Vector2(2, 2);
            endCollider.AddComponent<BackgroundMode>().playerMode = Player.ModeController.PlayerModes.Rectangle;
            SuccessTrigger success = endCollider.AddComponent<SuccessTrigger>();
            success.successCanvas = GameObject.Find("Success_Canvas");
            success.timerUI = FindObjectOfType<UI.TimerUI>();


            gridTilemap.RefreshAllTiles();
            borderTilemap.RefreshAllTiles();

            List<TileCreator.TileTypes> tileTypes =  CalculateLevelPath();

            tilePanel.InitTileButtons(tileTypes, builderSettings);

            DestroyImmediate(gridCollider);

        }

        public bool CheckLevelPath()
        {
            Vector2Int currentDirection = startDirection;
            Vector2Int currentPosition = gridData.start;

            do
            {
                currentPosition += currentDirection;

                if(Physics2D.OverlapPoint(((Vector2)currentPosition + new Vector2(0.5f, 0.5f)) * builderSettings.tileSize * gridTilemap.transform.lossyScale.x, builderSettings.backgroundLayer) == null)
                {
                    return false;
                }
                else
                {
                    LevelTile tile = Physics2D.OverlapPoint(((Vector2)currentPosition + new Vector2(0.5f, 0.5f)) * builderSettings.tileSize * gridTilemap.transform.lossyScale.x, builderSettings.backgroundLayer).GetComponent<LevelTile>();

                    placedTiles.Add(tile);

                    currentDirection = GetNextDirection(tile.tileType, currentDirection);
                    if(currentDirection == Vector2Int.zero)
                    {
                        return false;
                    }
                }
            } while (!(currentPosition + currentDirection == gridData.end));

            return true;
        }
        private LevelGrid GetLevelGrid()
        {
            LevelGrid gridData = new();

            gridData.grid = new Dictionary<Vector2Int, bool>();

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

        private List<TileCreator.TileTypes> CalculateLevelPath()
        {
            List<Vector2Int> pathPositions = new();
            List<TileCreator.TileTypes> pathTiles = new();

            endDirection = -endDirection;

            Vector2Int currentDirection = startDirection;
            Vector2Int currentPosition = gridData.start + startDirection;
            pathPositions.Add(currentPosition);
            do
            {

                List<Vector2Int> freeDirections = GetFreeDirections(currentPosition, pathPositions);
                if (freeDirections.Count == 0)
                {
                    pathTiles.RemoveAt(pathTiles.Count - 1);
                    pathPositions.Remove(currentPosition);
                    currentPosition = pathPositions[pathPositions.Count - 1];

                    currentDirection = currentPosition - pathPositions[pathPositions.Count - 2];


                    continue;
                }
                Vector2Int lastDirection = currentDirection;

                currentDirection = freeDirections[Random.Range(0, freeDirections.Count)];
                currentPosition += currentDirection;

                pathPositions.Add(currentPosition);
                pathTiles.Add(GetTileType(lastDirection, currentDirection));

            } while (!(currentPosition + endDirection == gridData.end));

            pathTiles.Add(GetTileType(currentDirection, endDirection));

            return pathTiles;
        }

        private List<Vector2Int> GetFreeDirections(Vector2Int currentPos, List<Vector2Int> currentPath)
        {
            List<Vector2Int> freeDirections = new();

            if (gridData.grid.ContainsKey(currentPos + Vector2Int.up))
            {
                if (gridData.grid[currentPos + Vector2Int.up] && !currentPath.Contains(currentPos + Vector2Int.up))
                {
                    freeDirections.Add(Vector2Int.up);
                }
            }
            if (gridData.grid.ContainsKey(currentPos + Vector2Int.right))
            {
                if (gridData.grid[currentPos + Vector2Int.right] && !currentPath.Contains(currentPos + Vector2Int.right))
                {
                    freeDirections.Add(Vector2Int.right);
                }
            }
            if (gridData.grid.ContainsKey(currentPos + Vector2Int.down))
            {
                if (gridData.grid[currentPos + Vector2Int.down] && !currentPath.Contains(currentPos + Vector2Int.down))
                {
                    freeDirections.Add(Vector2Int.down);
                }
            }
            if (gridData.grid.ContainsKey(currentPos + Vector2Int.left))
            {
                if (gridData.grid[currentPos + Vector2Int.left] && !currentPath.Contains(currentPos + Vector2Int.left))
                {
                    freeDirections.Add(Vector2Int.left);
                }
            }

            if (freeDirections.Count == 0)
            {
                gridData.grid[currentPos] = false;
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

        private void DrawBox(Tilemap tilemap, Vector2Int pos1, Vector2Int pos2, TileBase tile)
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
    }

    public class LevelGrid
    {
        public Dictionary<Vector2Int, bool> grid;
        public int height;
        public int width;
        public Vector2Int start;
        public Vector2Int end;
    }

}
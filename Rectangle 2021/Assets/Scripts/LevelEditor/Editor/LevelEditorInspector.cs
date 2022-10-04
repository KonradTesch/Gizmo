using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Rectangle.TileBuilder;
using Rectangle.Level;

namespace Rectangle.LevelEditor
{
    [CustomEditor(typeof(LevelEditor))]
    public class LevelEditorInspector : Editor
    {
        LevelEditor builder;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            builder = (LevelEditor)target;

            if(GUILayout.Button("Build Level"))
            {
                BuildLevel();
            }
        }

        private void BuildLevel()
        {
            DestroyImmediate(GameObject.Find("GridColliders"));

            LevelGrid gridData = GetLevelGrid();

            GameObject gridColliders = new GameObject("GridColliders");

            GameObject gridCollider = new GameObject("GridCollider");

            gridCollider.AddComponent<BoxCollider2D>().size = Vector2.one * builder.builderSettings.tileSize * builder.gridTilemap.transform.lossyScale;
            gridCollider.AddComponent<IsGridUsed>();

            builder.gridTilemap.ClearAllTiles();
            builder.borderTilemap.ClearAllTiles();

            for (int y = 0; y < gridData.height; y++)
            {
                for(int x = 0; x < gridData.width; x++)
                {
                    if (gridData.grid[new Vector2Int(x, y)])
                    {
                        foreach (ChangeData change in builder.builderSettings.gridTiles.groundTileChanges)
                        {
                            TileChangeData tileChange = new()
                            {
                                tile = change.tile,
                                position = change.position + (new Vector3Int(x, y, 0) * builder.builderSettings.tileSize),
                                transform = change.transform
                            };

                            builder.gridTilemap.SetTile(tileChange, true);
                        }

                        GameObject newGridCol = Instantiate(gridCollider, new Vector2(x + 0.5f, y + 0.5f) * builder.builderSettings.tileSize * builder.gridTilemap.transform.lossyScale, Quaternion.identity, gridColliders.transform);
                        SpriteRenderer backgroundRend = newGridCol.AddComponent<SpriteRenderer>();
                        backgroundRend.sprite = builder.builderSettings.backgroundImage;

                        int randomModeIndex = Random.Range(1, 4);
                        newGridCol.AddComponent<BackgroundMode>().playerMode = (Player.ModeController.PlayerModes)randomModeIndex;

                        switch (randomModeIndex)
                        {
                            case 1:
                                backgroundRend.color = builder.builderSettings.rectangleColor;
                                break;
                            case 2:
                                backgroundRend.color = builder.builderSettings.bubbleColor;
                                break;
                            case 3:
                                backgroundRend.color = builder.builderSettings.spikeyColor;
                                break;
                            case 4:
                                backgroundRend.color = builder.builderSettings.littleColor;
                                break;
                        }
                    }
                    else
                    {
                        DrawBox(builder.borderTilemap, new Vector2Int(x, y) * builder.builderSettings.tileSize, new Vector2Int((x + 1) * builder.builderSettings.tileSize - 1, (y + 1) * builder.builderSettings.tileSize - 1), builder.builderSettings.borderTile);
                    }
                }
            }

            DrawBorder(gridData);

            Vector2Int startDirection = GetStartDirection(gridData);
            Vector2 startPos = DrawStartOrEnd(gridData.start, startDirection);

            DestroyImmediate(GameObject.Find("Start_Collider"));
            GameObject startCollider = new GameObject("Start_Collider");
            startCollider.transform.position = startPos;
            startCollider.AddComponent<BoxCollider2D>().isTrigger = true;
            startCollider.GetComponent<BoxCollider2D>().size = new Vector2(2, 2);
            startCollider.AddComponent<BackgroundMode>().playerMode = Player.ModeController.PlayerModes.Rectanle;

            Vector2Int endDirection = GetEndDirection(gridData);
            Vector2 endPos = DrawStartOrEnd(gridData.end, endDirection);

            DestroyImmediate(GameObject.Find("Success_Collider"));
            GameObject endCollider = new GameObject("Success_Collider");
            endCollider.transform.position = endPos;
            endCollider.AddComponent<BoxCollider2D>().isTrigger = true;
            endCollider.GetComponent<BoxCollider2D>().size = new Vector2(2, 2);
            endCollider.AddComponent<BackgroundMode>().playerMode = Player.ModeController.PlayerModes.Rectanle;
            SuccessTrigger success = endCollider.AddComponent<SuccessTrigger>();
            success.successCanvas = GameObject.Find("Success_Canvas");
            success.timerUI = FindObjectOfType<UI.TimerUI>();


            builder.gridTilemap.RefreshAllTiles();
            builder.borderTilemap.RefreshAllTiles();

            CalculateLevelPath(gridData, startDirection, endDirection);

            DestroyImmediate(gridCollider);

        }

        private LevelGrid GetLevelGrid()
        {
            LevelGrid gridData = new();

            gridData.grid = new Dictionary<Vector2Int, bool>();

            gridData.width = builder.levelLayout.width - 2;
            gridData.height = builder.levelLayout.height - 2;

            for (int y = 1; y < builder.levelLayout.height - 1; y++)
            {
                for (int x = 1; x < builder.levelLayout.width - 1; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x , y) - Vector2Int.one;

                    if(builder.levelLayout.GetPixel(x, y) == Color.white)
                    {
                        gridData.grid.Add(gridPos, true);
                    }
                    else
                    {
                        gridData.grid.Add(gridPos, false);
                    }

                }
            }

            for (int y = 0; y < builder.levelLayout.height ; y++)
            {
                for (int x = 0; x < builder.levelLayout.width ; x++)
                {
                    if (builder.levelLayout.GetPixel(x, y) == Color.green)
                    {
                        gridData.start = new Vector2Int(x, y) - Vector2Int.one;
                    }
                    else if (builder.levelLayout.GetPixel(x, y) == Color.blue)
                    {
                        gridData.end = new Vector2Int(x, y) - Vector2Int.one;
                    }
                }
            }

            return gridData;
        }

        private void DrawBorder(LevelGrid gridData)
        {
            DrawBox(builder.borderTilemap, new Vector2Int(-2, builder.builderSettings.tileSize * gridData.height), new Vector2Int(builder.builderSettings.tileSize * gridData.width - 1, builder.builderSettings.tileSize * gridData.height + 1), builder.builderSettings.borderTile);
            DrawBox(builder.borderTilemap, new Vector2Int(builder.builderSettings.tileSize * gridData.width, builder.builderSettings.tileSize * gridData.height + 1), new Vector2Int(builder.builderSettings.tileSize * gridData.width + 1, 0), builder.builderSettings.borderTile);
            DrawBox(builder.borderTilemap, new Vector2Int(0, -1), new Vector2Int(builder.builderSettings.tileSize * gridData.width + 1, -2), builder.builderSettings.borderTile);
            DrawBox(builder.borderTilemap, new Vector2Int(-1, -2), new Vector2Int(- 2, builder.builderSettings.tileSize * gridData.height - 1), builder.builderSettings.borderTile);
        }

        private Vector2 DrawStartOrEnd(Vector2Int position, Vector2Int direction)
        {

            int size = builder.builderSettings.tileSize;

            if(direction == Vector2Int.right)
            {
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size + (size - 6), position.y * size + (size / 2 - 4)), new Vector2Int(position.x * size + (size - 1), position.y * size + (size / 2 + 3)), builder.builderSettings.borderTile);
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size + (size  - 4), position.y * size + (size / 2 - 2)), new Vector2Int(position.x * size + (size - 1), position.y * size + (size / 2 + 1)), null);

                return new Vector2((position.x + 1) * size * builder.borderTilemap.transform.lossyScale.x - (2 * builder.borderTilemap.transform.lossyScale.x), (position.y + 0.5f) * size * builder.borderTilemap.transform.lossyScale.y);
            }
            else if (direction == Vector2Int.left)
            {
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size, position.y * size + (size / 2 - 4)), new Vector2Int(position.x * size + 5, position.y * size + (size / 2 + 3)), builder.builderSettings.borderTile);
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size, position.y * size + (size / 2 - 2)), new Vector2Int(position.x * size + 3, position.y * size + (size / 2 + 1)), null);

                return new Vector2(position.x * size * builder.borderTilemap.transform.lossyScale.x + (2 * builder.borderTilemap.transform.lossyScale.x), (position.y + 0.5f) * size * builder.borderTilemap.transform.lossyScale.y);
            }
            else if (direction == Vector2Int.down)
            {
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size + (size / 2 - 4), position.y * size), new Vector2Int(position.x * size + (size / 2 + 3), position.y * size + 5), builder.builderSettings.borderTile);
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size + (size / 2 - 2), position.y * size), new Vector2Int(position.x * size + (size / 2 + 1), position.y * size + 3), null);

                return new Vector2((position.x + 0.5f) * size * builder.borderTilemap.transform.lossyScale.x, position.y * size * builder.borderTilemap.transform.lossyScale.y + (2 * builder.borderTilemap.transform.lossyScale.y));

            }
            else if (direction == Vector2Int.up)
            {
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size + (size / 2 - 4), position.y * size + (size - 6)), new Vector2Int(position.x * size + (size / 2 + 3), position.y * size + (size - 1)), builder.builderSettings.borderTile);
                DrawBox(builder.borderTilemap, new Vector2Int(position.x * size + (size / 2 - 2), position.y * size + (size - 4)), new Vector2Int(position.x * size + (size / 2 + 1), position.y * size + (size - 1)), null);

                return new Vector2((position.x + 0.5f) * size * builder.borderTilemap.transform.lossyScale.x, (position.y + 1) * size * builder.borderTilemap.transform.lossyScale.y - (2 * builder.borderTilemap.transform.lossyScale.y))
;
            }

            return Vector2.zero;

        }

        private Vector2Int GetStartDirection(LevelGrid gridData)
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

        private Vector2Int GetEndDirection(LevelGrid gridData)
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

        private List<LevelTileBuilder.TileTypes> CalculateLevelPath(LevelGrid gridData, Vector2Int startDirection, Vector2Int endDirection)
        {
            List<Vector2Int> pathPositions = new();
            List<LevelTileBuilder.TileTypes> pathTiles = new();

            endDirection = -endDirection;

            Vector2Int currentDirection = startDirection;
            Vector2Int currentPosition = gridData.start + startDirection;
            pathPositions.Add(currentPosition);
            do
            {
                List<Vector2Int> freeDirections = GetFreeDirections(gridData, currentPosition, pathPositions);
                if(freeDirections.Count == 0)
                {
                    pathTiles.RemoveAt(pathPositions.IndexOf(currentPosition));
                    pathPositions.Remove(currentPosition);
                    currentPosition = pathPositions[pathPositions.Count - 1];
                    continue;
                }

                if (pathTiles.Count == 0)
                {
                    pathTiles.Add(GetTileType(startDirection, currentDirection));
                }

                Vector2Int lastDirection = currentDirection;

                currentDirection = freeDirections[Random.Range(0, freeDirections.Count)];
                currentPosition += currentDirection;


                pathPositions.Add(currentPosition);
                pathTiles.Add(GetTileType(lastDirection, currentDirection));

            } while (!(currentPosition + endDirection == gridData.end));

            pathTiles[pathTiles.Count - 1] = GetTileType(currentDirection, endDirection);

            return pathTiles;
        }

        private List<Vector2Int> GetFreeDirections(LevelGrid gridData, Vector2Int currentPos, List<Vector2Int> currentPath)
        {
            List<Vector2Int> freeDirections = new();

            if(gridData.grid.ContainsKey(currentPos + Vector2Int.up))
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

            if(freeDirections.Count == 0)
            {
                gridData.grid[currentPos] = false;
            }

            return freeDirections;
        }

        private LevelTileBuilder.TileTypes GetTileType(Vector2Int input, Vector2Int output)
        {
            LevelTileBuilder.TileTypes tileType = LevelTileBuilder.TileTypes.undefined;

            if(input == Vector2Int.up)
            {
                if (output == Vector2Int.right)
                {
                    return LevelTileBuilder.TileTypes.UpAndRight;
                }
                else if (output == Vector2Int.down)
                {
                    return LevelTileBuilder.TileTypes.DownAndRight;
                }
                else if (output == Vector2Int.left)
                {
                    return LevelTileBuilder.TileTypes.LeftAndRight;
                }

            }
            else if (input == Vector2Int.right)
            {
                if (output == Vector2Int.up)
                {
                    return LevelTileBuilder.TileTypes.UpAndRight;
                }
                else if (output == Vector2Int.down)
                {
                    return LevelTileBuilder.TileTypes.DownAndRight;
                }
                else if (output == Vector2Int.left)
                {
                    return LevelTileBuilder.TileTypes.LeftAndRight;
                }
            }
            else if (input == Vector2Int.down)
            {
                if (output == Vector2Int.up)
                {
                    return LevelTileBuilder.TileTypes.UpAndDown;
                }
                else if (output == Vector2Int.right)
                {
                    return LevelTileBuilder.TileTypes.UpAndRight;
                }
                else if (output == Vector2Int.left)
                {
                    return LevelTileBuilder.TileTypes.LeftAndDown;
                }
            }
            else if (input == Vector2Int.left)
            {
                if (output == Vector2Int.up)
                {
                    return LevelTileBuilder.TileTypes.LeftAndUp;
                }
                else if (output == Vector2Int.right)
                {
                    return LevelTileBuilder.TileTypes.LeftAndRight;
                }
                else if (output == Vector2Int.down)
                {
                    return LevelTileBuilder.TileTypes.LeftAndDown;
                }
            }


            return tileType;
        }

        private void DrawBox(Tilemap tilemap ,Vector2Int pos1, Vector2Int pos2, TileBase tile)
        {
            Vector2 diff = pos2 - pos1;

            for(int y = 0; y <= Mathf.Abs(diff.y); y++)
            {
                for(int x = 0; x <= Mathf.Abs(diff.x); x++)
                {
                    if(diff.x < 0 && diff.y < 0)
                    {
                        tilemap.SetTile((Vector3Int)pos1 - new Vector3Int(x, y, 0), tile);
                    }
                    else if(diff.x < 0)
                    {
                        tilemap.SetTile((Vector3Int)pos1 + new Vector3Int(-x, y, 0), tile);
                    }
                    else if(diff.y < 0)
                    {
                        tilemap.SetTile((Vector3Int)pos1 +  new Vector3Int(x, -y, 0), tile);
                    }
                    else
                    {
                        tilemap.SetTile((Vector3Int)pos1 +  new Vector3Int(x, y, 0), tile);
                    }
                }
            }
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

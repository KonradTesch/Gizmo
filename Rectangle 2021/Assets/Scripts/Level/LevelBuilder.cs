using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.LevelCreation;

namespace Rectangle.Level
{
    public class LevelBuilder : MonoBehaviour
    {
        [Header("Builder Data")]
        public LevelData levelData;
        [SerializeField] private LevelBuilderSettings builderSettings;

        [Header("Tilemaps")]
        [SerializeField] private Tilemap gridTilemap;
        [SerializeField] private Tilemap borderTilemap;
        [SerializeField] private Tilemap platformTilemap;

        [Header("Background")]
        [SerializeField] private BackgroundController background;

        [Header("LevelText")]
        [SerializeField] private GameObject startText;
        [SerializeField] private GameObject endText;

        [Space()]

        [SerializeField] private GameObject anchorBoxPrefab;

        [HideInInspector] public List<LevelTile> pathTiles;
        [HideInInspector] public List<LevelTile> anchorTiles;

        [HideInInspector] public LevelGrid gridData;

        public void BuildLevel()
        {
            DestroyImmediate(GameObject.Find("GridColliders"));

            gridData = levelData.gridData;

            //Create parent for the grid backgrounds.
            GameObject gridBackground = new GameObject("GridBackground");
            General.GameBehavior.instance.gridBackground = gridBackground;

            //Create parent for the grid colliders.
            GameObject gridColliders = new GameObject("GridColliders");

            //Create reference for the grid colliders.
            GameObject gridCollider = new GameObject("GridCollider");

            BoxCollider2D col = gridCollider.AddComponent<BoxCollider2D>();
            col.size = Vector2.one * builderSettings.tileSize * gridTilemap.transform.lossyScale;
            col.isTrigger = true;

            gridCollider.AddComponent<GridField>();

            gridTilemap.ClearAllTiles();
            borderTilemap.ClearAllTiles();

            for (int y = 0; y < gridData.height; y++)
            {
                for (int x = 0; x < gridData.width; x++)
                {
                    if (!levelData.GetGridSpot(new Vector2Int(x, y)).blocked)
                    {
                        foreach (ChangeData change in builderSettings.gridTiles.groundTileChanges)
                        {
                            TileChangeData tileChange = new()
                            {
                                tile = change.tile,
                                position = change.position + (Vector3Int)(new Vector2Int(x, y) * builderSettings.tileSize),
                                transform = change.transform
                            };

                            gridTilemap.SetTile(tileChange, true);
                        }

                        GameObject newGridCol = Instantiate(gridCollider, new Vector2(x + 0.5f, y + 0.5f) * builderSettings.tileSize * gridTilemap.transform.lossyScale, Quaternion.identity, gridColliders.transform);
                        newGridCol.layer = LayerMask.NameToLayer("Grid");
                        newGridCol.AddComponent<BackgroundMode>();


                        GameObject fieldBackgound = new GameObject("Field Background (" + x + "/" + y + ")");
                        fieldBackgound.transform.SetParent(gridBackground.transform);
                        fieldBackgound.transform.position = newGridCol.transform.position;

                        SpriteRenderer backgroundRend = fieldBackgound.AddComponent<SpriteRenderer>();
                        backgroundRend.sortingLayerName = "Background";
                        backgroundRend.sprite = builderSettings.backgroundImage;
                        backgroundRend.sortingOrder = -1;

                        if (levelData.GetGridSpot(new Vector2Int(x, y)).star)
                        {
                            //Setup star
                            newGridCol.GetComponent<GridField>().star = true; ;

                            GameObject starSprite = new GameObject("StarSprite");
                            starSprite.transform.SetParent(fieldBackgound.transform);
                            starSprite.transform.localPosition = Vector3.zero;
                            starSprite.transform.localScale = Vector3.one * 4;

                            SpriteRenderer starRend = starSprite.AddComponent<SpriteRenderer>();
                            starRend.sprite = builderSettings.starSprite;
                            starRend.sortingLayerName = "Background";
                            starRend.sortingOrder = 0;

                            newGridCol.GetComponent<BackgroundMode>().hasStar = true;
                        }

                        newGridCol.GetComponent<GridField>().backgroundRend = backgroundRend;

                        if (levelData.GetGridSpot(new Vector2Int(x, y)).anchor)
                        {
                            //Seeuzp anchor
                            newGridCol.GetComponent<GridField>().isUsed = true;

                            LevelTile anchor = Instantiate(builderSettings.anchorTilePrefab).GetComponent<LevelTile>();

                            anchor.gameObject.name = "Anchor_Tile(" + x + "/" + y + ")";
                            anchor.transform.position = new Vector2(x + 0.5f, y + 0.5f) * builderSettings.tileSize * gridTilemap.transform.lossyScale;
                            anchor.gameObject.layer = LayerMask.NameToLayer("Background");

                            anchor.tileType = TileCreator.TileTypes.Anchor;
                            anchor.playerMode = Player.PlayerController.PlayerModes.Rectangle;

                            anchor.GetComponent<AnchorTile>().InitAnchorTiles(levelData.GetAnchorByCoordinates(new Vector2Int(x, y)).collectableTiles);
                            
                            SpriteRenderer tileRend = anchor.GetComponent<SpriteRenderer>();
                            tileRend.sortingLayerName = "Level";

                            anchorTiles.Add(anchor);

                            //Setup anchor box
                            Instantiate(anchorBoxPrefab, anchor.transform.position, Quaternion.identity).GetComponentInChildren<UI.AnchorBox>().SetAnchorTiles(levelData.GetAnchorByCoordinates(new Vector2Int(x, y)).collectableTiles);
                        }

                    }
                    else
                    {
                        //block the spot with border tiles.
                        DrawBox(borderTilemap, new Vector2Int(x, y) * builderSettings.tileSize, new Vector2Int(x + 1, y + 1) * builderSettings.tileSize - Vector2Int.one, builderSettings.borderTile);
                    }
                }
            }
            //Set the background to the center of the level
            background.transform.position = new Vector3(gridData.width, gridData.height, 0) / 2 * (Vector2)builderSettings.tileSize;

            DrawBorder(gridData);

            //Setup start box
            Vector2 startPos = DrawStartOrEnd(gridData.start.coordinates, gridData.start.direction);

            startText.transform.position = startPos;
            General.GameBehavior.instance.player.transform.position = startPos;

            DestroyImmediate(GameObject.Find("Start_Collider"));
            GameObject startCollider = new GameObject("Start_Collider");
            startCollider.gameObject.layer = LayerMask.NameToLayer("Grid");
            startCollider.transform.position = startPos;
            startCollider.AddComponent<BoxCollider2D>().isTrigger = true;
            startCollider.GetComponent<BoxCollider2D>().size = new Vector2(4, 4);
            startCollider.AddComponent<BackgroundMode>().playerMode = Player.PlayerController.PlayerModes.Rectangle;
            startCollider.AddComponent<GridField>().isUsed = true;

            //Setup end box
            Vector2 endPos = DrawStartOrEnd(gridData.end.coordinates, gridData.end.direction);

            endText.transform.position = endPos;

            DestroyImmediate(GameObject.Find("Success_Collider"));
            GameObject endCollider = new GameObject("Success_Collider");
            endCollider.gameObject.layer = LayerMask.NameToLayer("Grid");
            endCollider.transform.position = endPos;
            endCollider.AddComponent<BoxCollider2D>().isTrigger = true;
            endCollider.GetComponent<BoxCollider2D>().size = new Vector2(4, 4);
            endCollider.AddComponent<BackgroundMode>().playerMode = Player.PlayerController.PlayerModes.Rectangle;
            endCollider.AddComponent<GridField>().isUsed = true;
            SuccessTrigger success = endCollider.AddComponent<SuccessTrigger>();
            success.successPanel = General.GameBehavior.instance.sucessPanel;
            success.timerUI = GameObject.FindObjectOfType<UI.TimerUI>().GetComponent<UI.TimerUI>();

            gridTilemap.RefreshAllTiles();
            borderTilemap.RefreshAllTiles();

            //Setup all collectable tiles in the anchor tiles
            List<TileCreator.TileTypes> tileTypes = new();

            General.GameBehavior.instance.InitLevelTiles(levelData.plannedTiles);

            DestroyImmediate(gridCollider);
        }

        public bool CheckLevelPath(Vector2Int startPosition)
        {
            Debug.Log($"LevelBuilder: ->CheckLevePath({startPosition})");
            pathTiles.Clear();

            Vector2Int currentDirection = Vector2Int.zero;
            Vector2Int currentPosition = startPosition;

            if (levelData.GetGridSpot(startPosition + Vector2Int.up) != null && levelData.GetGridSpot(startPosition + Vector2Int.up).placedTile != null)
            {
                LevelTile startTile = levelData.GetGridSpot(startPosition + Vector2Int.up).placedTile;
                if (GetNextDirection(startTile.tileType, Vector2Int.up) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.up;
                }
            }
            if (levelData.GetGridSpot(startPosition + Vector2Int.right) != null && levelData.GetGridSpot(startPosition + Vector2Int.right).placedTile != null)
            {
                LevelTile startTile = levelData.GetGridSpot(startPosition + Vector2Int.right).placedTile;
                if (GetNextDirection(startTile.tileType, Vector2Int.right) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.right;
                }
            }
            if (levelData.GetGridSpot(startPosition + Vector2Int.down) != null && levelData.GetGridSpot(startPosition + Vector2Int.down).placedTile != null)
            {
                LevelTile startTile = levelData.GetGridSpot(startPosition + Vector2Int.down).placedTile;
                if (GetNextDirection(startTile.tileType, Vector2Int.down) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.down;
                }
            }
            if (levelData.GetGridSpot(startPosition + Vector2Int.left) != null && levelData.GetGridSpot(startPosition + Vector2Int.left).placedTile != null)
            {
                LevelTile startTile = levelData.GetGridSpot(startPosition + Vector2Int.left).placedTile;
                if (GetNextDirection(startTile.tileType, Vector2Int.left) != Vector2Int.zero)
                {
                    currentDirection = Vector2Int.left;
                }
            }

            if (currentDirection == Vector2Int.zero)
            {
                Debug.Log($"LevelBuilder: <- CheckLevePath() false at pos: {currentPosition}, dir:{currentDirection} (wrong beginning Tile Block))");
                return false;
            }


            do
            {
                currentPosition += currentDirection;

                if (levelData.GetGridSpot(currentPosition) != null && levelData.GetGridSpot(currentPosition).placedTile == null)
                {
                    Debug.Log($"LevelBuilder: <- CheckLevePath() false at pos: {currentPosition}, dir:{currentDirection} (no next TileBlock)");

                    return false;
                }
                else
                {
                    LevelTile tile = levelData.GetGridSpot(currentPosition).placedTile;

                    if(!pathTiles.Contains(tile))
                    {
                        pathTiles.Add(tile);
                    }

                    currentDirection = GetNextDirection(tile.tileType, currentDirection);
                    if(currentDirection == Vector2Int.zero)
                    {
                        Debug.Log($"LevelBuilder: <- CheckLevePath() false at pos: {currentPosition}, dir:{currentDirection} (wrong next Tile Block)");

                        return false;
                    }

                }
            } while (!(currentPosition + currentDirection == gridData.end.coordinates) && levelData.GetAnchorByCoordinates(currentPosition + currentDirection) == null);

            Debug.Log($"LevelBuilder: <- CheckLevePath() true at pos: {currentPosition}, dir:{currentDirection}");

            return true;
        }

        private void DrawBorder(LevelGrid gridData)
        {
            //top border
            DrawBox(borderTilemap, new Vector2Int(-2, builderSettings.tileSize.y * gridData.height), new Vector2Int(builderSettings.tileSize.x * gridData.width - 1, builderSettings.tileSize.y * gridData.height + 1), builderSettings.borderTile);
            //right border
            DrawBox(borderTilemap, new Vector2Int(builderSettings.tileSize.x * gridData.width, builderSettings.tileSize.y * gridData.height + 1), new Vector2Int(builderSettings.tileSize.x * gridData.width + 1, 0), builderSettings.borderTile);
            //bottom border
            DrawBox(borderTilemap, new Vector2Int(0, -1), new Vector2Int(builderSettings.tileSize.x * gridData.width + 1, -2), builderSettings.borderTile);
            //right border
            DrawBox(borderTilemap, new Vector2Int(-1, -2), new Vector2Int(-2, builderSettings.tileSize.y * gridData.height - 1), builderSettings.borderTile);
        }

        private Vector2 DrawStartOrEnd(Vector2Int position, Vector2Int direction)
        {

            Vector2Int size = builderSettings.tileSize;

            if (direction == Vector2Int.right)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x + (size.x - 6), position.y * size.y + (size.y / 2 - 4)), new Vector2Int(position.x * size.x + (size.x - 1), position.y * size.y + (size.y / 2 + 3)), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x + (size.x - 4), position.y * size.y + (size.y / 2 - 2)), new Vector2Int(position.x * size.x + (size.x - 1), position.y * size.y + (size.y / 2 + 1)), null);

                return new Vector2((position.x + 1) * size.x - 2, (position.y + 0.5f) * size.y);
            }
            else if (direction == Vector2Int.left)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x, position.y * size.y + (size.y / 2 - 4)), new Vector2Int(position.x * size.x + 5, position.y * size.y + (size.y / 2 + 3)), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x, position.y * size.y + (size.y / 2 - 2)), new Vector2Int(position.x * size.x + 3, position.y * size.y + (size.y / 2 + 1)), null);

                return new Vector2(position.x * size.x + 2 , (position.y + 0.5f) * size.y);
            }
            else if (direction == Vector2Int.down)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x + (size.x / 2 - 4), position.y * size.y), new Vector2Int(position.x * size.x + (size.x / 2 + 3), position.y * size.y + 7), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x + (size.x / 2 - 2), position.y * size.y), new Vector2Int(position.x * size.x + (size.x / 2 + 1), position.y * size.y + 5), null);
                DrawBox(platformTilemap, new Vector2Int(position.x * size.x + (size.x / 2 - 2), position.y * size.y), new Vector2Int(position.x * size.x + (size.x / 2 + 1), position.y * size.y), builderSettings.platformTile);

                return new Vector2((position.x + 0.5f) * size.x, position.y * size.y + 2);

            }
            else if (direction == Vector2Int.up)
            {
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x + (size.x / 2 - 4), position.y * size.y + (size.y - 6)), new Vector2Int(position.x * size.x + (size.x / 2 + 3), position.y * size.y + (size.y - 1)), builderSettings.borderTile);
                DrawBox(borderTilemap, new Vector2Int(position.x * size.x + (size.x / 2 - 2), position.y * size.y + (size.y - 4)), new Vector2Int(position.x * size.x + (size.x / 2 + 1), position.y * size.y + (size.y - 1)), null);
                DrawBox(platformTilemap, new Vector2Int(position.x * size.x + (size.x / 2 - 2), (position.y * size.y) + (size.y - 2)), new Vector2Int(position.x * size.x + (size.x / 2 + 1), (position.y * size.y) + (size.y - 2)), builderSettings.platformTile);


                return new Vector2((position.x + 0.5f) * size.x, (position.y + 1) * size.y - 2)
;
            }

            return Vector2.zero;

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
                        return Vector2Int.left;
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
}
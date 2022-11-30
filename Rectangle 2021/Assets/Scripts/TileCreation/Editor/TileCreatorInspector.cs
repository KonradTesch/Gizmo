using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.Level;

namespace Rectangle.TileCreation
{
    [CustomEditor(typeof(TileCreator))]
    public class TileCreatorInspector : Editor
    {
        private LevelTileData tileData;


        bool canDrawTile = false;
        bool fileExists = false;
        bool clearTilemap = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TileCreator builder = (TileCreator)target;

            GUILayout.Space(10);

            GUILayout.Label("Moving Objects", EditorStyles.boldLabel);


            if (builder.movingObjects != null && builder.movingObjects.Count > 0)
            {
                bool rename = false;

                for(int i = 0; i < builder.movingObjects.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(builder.movingObjects[i], typeof(Tilemap), true);

                    if(GUILayout.Button("Edit"))
                    {
                        Selection.activeGameObject = builder.movingObjects[i].gameObject;
                    }

                    if(GUILayout.Button("Delete"))
                    {
                        if (builder.movingObjects[i] != null)
                        {
                            DestroyImmediate(builder.movingObjects[i].gameObject);
                        }
                        builder.movingObjects.Remove(builder.movingObjects[i]);
                        rename = true;
                        break;
                    }

                    EditorGUILayout.EndHorizontal();


                    GUILayout.Space(5);
                }

                if(rename)
                {
                    for (int i = 0; i < builder.movingObjects.Count; i++)
                    {
                        builder.movingObjects[i].gameObject.name = $"LevelTilemap_MovingObject({i})";
                    }
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Add Moving Object"))
            {
                CreateMovingPlatform(builder);
            }

            if (GUILayout.Button("Delete All Moving Objects"))
            {
                foreach (Tilemap platform in builder.movingObjects)
                {
                    if (platform != null)
                    {
                        DestroyImmediate(platform.gameObject);
                    }
                }
                builder.movingObjects.Clear();
            }

            GUILayout.Space(15);

            if (GUILayout.Button("Save Tile"))
            {
                if (AssetDatabase.LoadAssetAtPath($"{builder.saveFolderPath}/{builder.playerMode}/{builder.tileName}.asset", typeof(LevelTileData)) != null)
                {
                    fileExists = true;
                }
                else
                {
                    SaveTile(builder);
                }

            }

            if (fileExists)
            {
                EditorGUILayout.HelpBox("There is already a file with the given name.", MessageType.Warning);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Override"))
                {
                    AssetDatabase.DeleteAsset($"{builder.saveFolderPath}/{builder.playerMode}/{builder.tileName}.asset");

                    SaveTile(builder);
                    fileExists = false;
                }
                if (GUILayout.Button("Cancel"))
                {
                    fileExists = false;
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(15);

            if(GUILayout.Button("Clear Tilemap"))
            {
                clearTilemap = true;
            }

            if (clearTilemap)
            {
                EditorGUILayout.HelpBox("Are you sure?.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Yes"))
                {
                    ClearTilemap(builder);
                    clearTilemap = false;
                }
                if (GUILayout.Button("No"))
                {
                    clearTilemap = false;
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();


            tileData = (LevelTileData)EditorGUILayout.ObjectField(tileData, typeof(LevelTileData), false);

            if(GUILayout.Button("Draw Tile"))
            {
                if (tileData != null)
                {
                    canDrawTile = true;
                }
            }
            GUILayout.EndHorizontal();
            if (canDrawTile && tileData != null)
            {
                EditorGUILayout.HelpBox("Are you sure?", MessageType.Warning);

                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Yes"))
                {
                    DrawTile(builder, tileData);
                    canDrawTile = false;
                }

                if (GUILayout.Button("No"))
                {
                    canDrawTile = false;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(15);

            if (GUILayout.Button("Test Level"))
            {
                StartTest(builder);
            }
        }

        private void SaveTile(TileCreator builder)
        {
            if (!AssetDatabase.IsValidFolder("Assets/LevelTiles"))
            {
                AssetDatabase.CreateFolder("Assets", "LevelTiles");
            }

            if (builder.tileName == "")
            {
                Debug.LogWarning("Please enter a name for the tile in the Inspector.");
                return;
            }
            if (builder.tileType == TileCreator.TileTypes.undefined)
            {
                Debug.LogWarning("Please select a Tile Type for the tile in the Inspector.");
                return;
            }
            if (builder.playerMode == Player.PlayerController.PlayerModes.None)
            {
                Debug.LogWarning("Please select a Player Mode for the tile in the Inspector.");
                return;
            }
            if(builder.hasCollactables && builder.collectableParent.childCount < 6)
            {
                Debug.LogWarning("The tile must have at least six collectable spots.");
                return;
            }

            LevelTileData tile = (LevelTileData)CreateInstance(typeof(LevelTileData));

            tile.backgroundTileChanges = new();
            tile.groundTileChanges = new();
            tile.rampTileChanges = new();
            tile.platformTileChanges = new();
            tile.spikesTileChanges = new();
            tile.movingObjects = new();

            for(int i = 0; i < builder.movingObjects.Count; i++)
            {
                WaypointFollower movingObject = builder.movingObjects[i].GetComponent<WaypointFollower>();

                tile.movingObjects.Add(new MovingObjectData(movingObject.moveSpeed, movingObject.waypoints, movingObject.movingType, movingObject.vanishing, movingObject.vanishingTime));
            }


            for (int x = 0; x < builder.tileSize.x; x++)
            {
                for(int y = 0; y < builder.tileSize.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if(builder.backgroundTilemap.HasTile(pos))
                    {
                        ChangeData change = new()
                        {
                            position = pos,
                            tile = builder.backgroundTilemap.GetTile(pos),
                            transform = builder.backgroundTilemap.GetTransformMatrix(pos)
                        };

                        tile.backgroundTileChanges.Add(change);
                    }

                    if (builder.groundTilemap.HasTile(pos))
                    {
                        ChangeData change = new()
                        {
                            position = pos,
                            tile = builder.groundTilemap.GetTile(pos),
                            transform = builder.groundTilemap.GetTransformMatrix(pos)
                        };

                        tile.groundTileChanges.Add(change);
                    }

                    if (builder.rampTilemap.HasTile(pos))
                    {
                        ChangeData change = new()
                        {
                            position = pos,
                            tile = builder.rampTilemap.GetTile(pos),
                            transform = builder.rampTilemap.GetTransformMatrix(pos)
                        };

                        tile.rampTileChanges.Add(change);
                    }

                    if (builder.onewayPlatformTilemap.HasTile(pos))
                    {
                        ChangeData change = new()
                        {
                            position = pos,
                            tile = builder.onewayPlatformTilemap.GetTile(pos),
                            transform = builder.onewayPlatformTilemap.GetTransformMatrix(pos)
                        };

                        tile.platformTileChanges.Add(change);
                    }

                    if (builder.spikesTilemap.HasTile(pos))
                    {
                        ChangeData change = new()
                        {
                            position = pos,
                            tile = builder.spikesTilemap.GetTile(pos),
                            transform = builder.spikesTilemap.GetTransformMatrix(pos)
                        };

                        tile.spikesTileChanges.Add(change);
                    }

                    if (builder.movingObjects != null && builder.movingObjects.Count > 0)
                    {
                        for (int i = 0; i < builder.movingObjects.Count; i++)
                        {
                            if (builder.movingObjects[i].HasTile(pos))
                            {
                                ChangeData change = new()
                                {
                                    position = pos,
                                    tile = builder.movingObjects[i].GetTile(pos),
                                    transform = builder.movingObjects[i].GetTransformMatrix(pos)
                                };

                                tile.movingObjects[i].tileChanges.Add(change);
                            }
                        }


                    }
                }
            }



            if(builder.hasCollactables)
            {
                tile.collectablePositions = new Vector2[builder.collectableParent.childCount];
                int i = 0;
                foreach(Transform spot in builder.collectableParent)
                {
                    tile.collectablePositions[i] = spot.localPosition;
                    i++;
                }
            }

            tile.tileSize = builder.tileSize;
            tile.tileType = builder.tileType;
            tile.playerMode = builder.playerMode;


            if (!AssetDatabase.IsValidFolder(builder.saveFolderPath + "/" + tile.playerMode.ToString()))
            {
                AssetDatabase.CreateFolder(builder.saveFolderPath, tile.playerMode.ToString());
            }

            AssetDatabase.CreateAsset(tile, $"{builder.saveFolderPath}/{tile.playerMode}/{builder.tileName}.asset");
            AssetDatabase.SaveAssets();

            Debug.Log($"New TileData saved at '{builder.saveFolderPath}/{tile.playerMode}/{builder.tileName}.asset'.");
        }

        private void DrawTile(TileCreator builder, LevelTileData tile)
        {
            builder.backgroundTilemap.ClearAllTiles();
            builder.groundTilemap.ClearAllTiles();
            builder.rampTilemap.ClearAllTiles();
            builder.onewayPlatformTilemap.ClearAllTiles();
            builder.spikesTilemap.ClearAllTiles();
            builder.tileSize = tile.tileSize;
            builder.tileName = tile.name;


            foreach(ChangeData change in tile.groundTileChanges)
            {
                TileChangeData tileChane = new()
                {
                    position = change.position,
                    tile = change.tile,
                    transform = change.transform
                };

                builder.groundTilemap.SetTile(tileChane, true);
            }
            builder.groundTilemap.RefreshAllTiles();

            foreach (ChangeData change in tile.rampTileChanges)
            {
                TileChangeData tileChane = new()
                {
                    position = change.position,
                    tile = change.tile,
                    transform = change.transform
                };

                builder.rampTilemap.SetTile(tileChane, true);
            }
            builder.rampTilemap.RefreshAllTiles();

            foreach (ChangeData change in tile.platformTileChanges)
            {
                TileChangeData tileChane = new()
                {
                    position = change.position,
                    tile = change.tile,
                    transform = change.transform
                };

                builder.onewayPlatformTilemap.SetTile(tileChane, true);
            }
            builder.onewayPlatformTilemap.RefreshAllTiles();

            foreach (ChangeData change in tile.spikesTileChanges)
            {
                TileChangeData tileChane = new()
                {
                    position = change.position,
                    tile = change.tile,
                    transform = change.transform
                };

                builder.spikesTilemap.SetTile(tileChane, true);
            }
            builder.spikesTilemap.RefreshAllTiles();


            foreach (Tilemap platformTilemap in builder.movingObjects)
            {
                if(platformTilemap != null)
                    DestroyImmediate(platformTilemap.gameObject);
            }
            builder.movingObjects.Clear();

            foreach(MovingObjectData platformData in tile.movingObjects)
            {
                Tilemap newPlatform = CreateMovingPlatform(builder);

                WaypointFollower waypointFollower = newPlatform.GetComponent<WaypointFollower>();

                waypointFollower.moveSpeed = platformData.moveSpeed;
                waypointFollower.waypoints = platformData.waypoints;

                foreach (ChangeData change in platformData.tileChanges)
                {
                    TileChangeData tileChane = new()
                    {
                        position = change.position,
                        tile = change.tile,
                        transform = change.transform
                    };

                    newPlatform.SetTile(tileChane, true);
                }
                newPlatform.RefreshAllTiles();
            }

        }

        private Tilemap CreateMovingPlatform(TileCreator builder)
        {
            int platformIndex;
            if(builder.movingObjects == null)
            {
                builder.movingObjects = new();
                platformIndex = 1;
            }
            else
            {
                platformIndex = builder.movingObjects.Count;
            }

            GameObject platformTilemap = new GameObject("LevelTilemap_MovingObject(" + platformIndex + ")");

            platformTilemap.transform.SetParent(builder.groundTilemap.transform.parent);
            platformTilemap.AddComponent<Tilemap>();
            platformTilemap.AddComponent<TilemapRenderer>();
            platformTilemap.AddComponent<TilemapCollider2D>();
            platformTilemap.AddComponent<WaypointFollower>();
            platformTilemap.AddComponent<StickyPlayer>();

            platformTilemap.transform.localScale = builder.groundTilemap.transform.localScale;
            platformTilemap.layer = builder.groundTilemap.gameObject.layer;

            builder.movingObjects.Add(platformTilemap.GetComponent<Tilemap>());

            return platformTilemap.GetComponent<Tilemap>();
        }

        private void ClearTilemap(TileCreator builder)
        {
            builder.backgroundTilemap.ClearAllTiles();
            builder.backgroundTilemap.RefreshAllTiles();

            builder.groundTilemap.ClearAllTiles();
            builder.groundTilemap.RefreshAllTiles();

            builder.rampTilemap.ClearAllTiles();
            builder.rampTilemap.RefreshAllTiles();

            builder.onewayPlatformTilemap.ClearAllTiles();
            builder.onewayPlatformTilemap.RefreshAllTiles();

            builder.spikesTilemap.ClearAllTiles();
            builder.spikesTilemap.RefreshAllTiles();
        }
        private void StartTest(TileCreator builder)
        {
            EditorApplication.EnterPlaymode();

            foreach (Transform child in GameObject.Find("TestObjects").transform)
            {
                child.gameObject.SetActive(true);
            }

            SpriteRenderer backgroundSprite = GameObject.Find("Background").GetComponent<SpriteRenderer>();

            backgroundSprite.GetComponent<BackgroundMode>().playerMode = builder.playerMode;

            backgroundSprite.color = builder.builderSettings.GetModeColor(builder.playerMode);

        }
    }
}

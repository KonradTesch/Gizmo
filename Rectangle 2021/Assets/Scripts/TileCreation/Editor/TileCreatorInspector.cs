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

            GUILayout.Label("Moving Platforms", EditorStyles.boldLabel);

            //Debug.Log("moving Platform Count: " + movingPlatforms.Count);

            if (builder.movingPlatforms != null && builder.movingPlatforms.Count > 0)
            {
                bool rename = false;

                for(int i = 0; i < builder.movingPlatforms.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(builder.movingPlatforms[i], typeof(Tilemap), true);

                    if(GUILayout.Button("Edit"))
                    {
                        Selection.activeGameObject = builder.movingPlatforms[i].gameObject;
                    }

                    if(GUILayout.Button("Delete"))
                    {
                        DestroyImmediate(builder.movingPlatforms[i].gameObject);
                        builder.movingPlatforms.Remove(builder.movingPlatforms[i]);
                        rename = true;
                        break;
                    }

                    EditorGUILayout.EndHorizontal();


                    GUILayout.Space(5);
                }

                if(rename)
                {
                    for (int i = 0; i < builder.movingPlatforms.Count; i++)
                    {
                        builder.movingPlatforms[i].gameObject.name = $"LevelTilemap_MovingPLatform({i})";
                    }
                }
            }

            GUILayout.Space(10);

            if(GUILayout.Button("Delete All"))
            {
                foreach(Tilemap platform in builder.movingPlatforms)
                {
                    DestroyImmediate(platform.gameObject);
                }
                builder.movingPlatforms.Clear();
            }

            if (GUILayout.Button("Add Moving Platform"))
            {
                CreateMovingPlatform(builder);
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
            tile.movingPlatforms = new();

            for(int i = 0; i < builder.movingPlatforms.Count; i++)
            {
                WaypointFollower platform = builder.movingPlatforms[i].GetComponent<WaypointFollower>();

                tile.movingPlatforms.Add(new MovingPlatformData(platform.moveSpeed, platform.waypoints));
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

                    if (builder.platformTilemap.HasTile(pos))
                    {
                        ChangeData change = new()
                        {
                            position = pos,
                            tile = builder.platformTilemap.GetTile(pos),
                            transform = builder.platformTilemap.GetTransformMatrix(pos)
                        };

                        tile.platformTileChanges.Add(change);
                    }

                    if(builder.movingPlatforms != null && builder.movingPlatforms.Count > 0)
                    {
                        for (int i = 0; i < builder.movingPlatforms.Count; i++)
                        {
                            if (builder.movingPlatforms[i].HasTile(pos))
                            {
                                ChangeData change = new()
                                {
                                    position = pos,
                                    tile = builder.movingPlatforms[i].GetTile(pos),
                                    transform = builder.movingPlatforms[i].GetTransformMatrix(pos)
                                };

                                tile.movingPlatforms[i].platformTileChanges.Add(change);
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
            builder.platformTilemap.ClearAllTiles();
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

                builder.platformTilemap.SetTile(tileChane, true);
            }
            builder.platformTilemap.RefreshAllTiles();

            foreach(Tilemap platformTilemap in builder.movingPlatforms)
            {
                DestroyImmediate(platformTilemap.gameObject);
            }
            builder.movingPlatforms.Clear();

            foreach(MovingPlatformData platformData in tile.movingPlatforms)
            {
                Tilemap newPlatform = CreateMovingPlatform(builder);

                WaypointFollower waypointFollower = newPlatform.GetComponent<WaypointFollower>();

                waypointFollower.moveSpeed = platformData.moveSpeed;
                waypointFollower.waypoints = platformData.waypoints;

                foreach (ChangeData change in platformData.platformTileChanges)
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
            if(builder.movingPlatforms == null)
            {
                builder.movingPlatforms = new();
                platformIndex = 1;
            }
            else
            {
                platformIndex = builder.movingPlatforms.Count;
            }

            GameObject platformTilemap = new GameObject("LevelTilemap_MovingPlatform(" + platformIndex + ")");

            platformTilemap.transform.SetParent(builder.groundTilemap.transform.parent);
            platformTilemap.AddComponent<Tilemap>();
            platformTilemap.AddComponent<TilemapRenderer>();
            platformTilemap.AddComponent<TilemapCollider2D>();
            platformTilemap.AddComponent<WaypointFollower>();
            platformTilemap.AddComponent<StickyPlayer>();

            platformTilemap.transform.localScale = builder.groundTilemap.transform.localScale;
            platformTilemap.layer = builder.groundTilemap.gameObject.layer;

            builder.movingPlatforms.Add(platformTilemap.GetComponent<Tilemap>());

            return platformTilemap.GetComponent<Tilemap>();
        }

        private void ClearTilemap(TileCreator builder)
        {
            builder.groundTilemap.ClearAllTiles();
            builder.groundTilemap.RefreshAllTiles();

            builder.rampTilemap.ClearAllTiles();
            builder.rampTilemap.RefreshAllTiles();

            builder.platformTilemap.ClearAllTiles();
            builder.platformTilemap.RefreshAllTiles();
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

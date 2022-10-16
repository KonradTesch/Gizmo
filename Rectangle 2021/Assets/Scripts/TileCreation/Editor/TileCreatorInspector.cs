using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.Player;
using Rectangle.Level;

namespace Rectangle.TileCreation
{
    [CustomEditor(typeof(TileCreator))]
    public class TileCreatorInspector : Editor
    {
        private LevelTileData tileData;

        bool createTile = false;
        bool fileExists = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TileCreator builder = (TileCreator)target;

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


            GUILayout.BeginHorizontal();


            tileData = (LevelTileData)EditorGUILayout.ObjectField(tileData, typeof(LevelTileData), false);

            if(GUILayout.Button("Create Tile"))
            {
                if (tileData != null)
                {
                    createTile = true;
                }
            }
            GUILayout.EndHorizontal();
            if (createTile && tileData != null)
            {
                EditorGUILayout.HelpBox("Are you sure?", MessageType.Warning);

                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Yes"))
                {
                    CreateTile(builder, tileData);
                    createTile = false;
                }

                if (GUILayout.Button("No"))
                {
                    createTile = false;
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

            LevelTileData tile = (LevelTileData)CreateInstance(typeof(LevelTileData));

            tile.groundTileChanges = new();
            tile.rampTileChanges = new();

            for (int x = 0; x < builder.tileSize.x; x++)
            {
                for(int y = 0; y < builder.tileSize.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if(builder.groundTilemap.HasTile(pos))
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

        private void CreateTile(TileCreator builder, LevelTileData tile)
        {
            builder.groundTilemap.ClearAllTiles();
            builder.rampTilemap.ClearAllTiles();

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

using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.TileBuilder
{
    [CustomEditor(typeof(LevelTileBuilder))]
    public class TileBuilderInspector : Editor
    {
        private LevelTileData tileData;

        bool createTile = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelTileBuilder builder = (LevelTileBuilder)target;

            if (GUILayout.Button("Save Tile"))
            {
                SaveTile(builder);
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
        }

        private void SaveTile(LevelTileBuilder builder)
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
            if (AssetDatabase.LoadAssetAtPath($"Assets/LevelTiles/{builder.tileName}.asset", typeof(LevelTileData)) != null)
            {
                Debug.LogWarning("There is already a file with the given name.");
                return;
            }
            if (builder.tileType == LevelTileBuilder.TileTypes.undefined)
            {
                Debug.LogWarning("Please select a Tile Type for the tile in the Inspector.");
                return;
            }
            if (builder.playerMode == Player.ModeController.PlayerModes.None)
            {
                Debug.LogWarning("Please select a Player Mode for the tile in the Inspector.");
                return;
            }

            LevelTileData tile = (LevelTileData)CreateInstance(typeof(LevelTileData));

            tile.groundTileChanges = new();
            tile.nookTileChanges = new();
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

                    if (builder.nooksTilemap.HasTile(pos))
                    {
                        ChangeData change = new()
                        {
                            position = pos,
                            tile = builder.nooksTilemap.GetTile(pos),
                            transform = builder.nooksTilemap.GetTransformMatrix(pos)
                        };

                        tile.nookTileChanges.Add(change);
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

            AssetDatabase.CreateAsset(tile, $"Assets/LevelTiles/{builder.tileName}.asset");
            AssetDatabase.SaveAssets();

            Debug.Log($"New TileData saved at 'Assets/LevelTiles/{builder.tileName}.asset'.");
        }

        private void CreateTile(LevelTileBuilder builder, LevelTileData tile)
        {
            builder.groundTilemap.ClearAllTiles();
            builder.nooksTilemap.ClearAllTiles();
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

            foreach (ChangeData change in tile.nookTileChanges)
            {
                TileChangeData tileChane = new()
                {
                    position = change.position,
                    tile = change.tile,
                    transform = change.transform
                };

                builder.nooksTilemap.SetTile(tileChane, true);
            }
            builder.nooksTilemap.RefreshAllTiles();
        }
    }
}

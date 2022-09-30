using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rectangle.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        private Rect menuBar;
        private Rect mainPanel;

        private readonly float menuBarHeight = 55f;
        private readonly string path = "Assets/Resources/LevelData";

        private LevelData levelData;
        private string newLevelName;
        private bool createNew = false;
        private Vector2 grid;
        private int gridWidth;
        private int gridHeight;
        private int areaID;

        private Color red = new(0.945f, 0.306f, 0.306f);
        private Color blue = new(0.439f, 0.427f, 0.937f);
        private Color green = new(0.314f, 0.745f, 0.302f);
        private Color yellow = new(0.972f, 0.702f, 0.227f);
        private Color currentColor;
        private Tile.Mode currentMode;


        private GUIStyle borderStyle = new GUIStyle();

        private Vector2 offset;
        private Vector2 drag;
        private float zoom = 1;
        private GUISkin skin;

        private Tile[] tiles = new Tile[0];

        private List<List<Tile>> areas;

        [MenuItem("Rectangle/Level Editor")]
        public static void OpenWindow()
        {
            GetWindow<LevelEditorWindow>("Rectangle Level Editor");
        }

        private void OnEnable()
        {
            if (areas == null)
            {
                areas = new List<List<Tile>>();
                areas.Add(new List<Tile>());
            }

            skin = CreateInstance<GUISkin>();
            skin.button.alignment = TextAnchor.MiddleCenter;
            skin.button.normal.textColor = Color.white;
            skin.button.fontSize = (int)(40 * zoom);
            if (skin.button.fontSize < 12)
            {
                skin.button.fontSize = 12;
            }

            borderStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

        }

        private void OnGUI()
        {
            DrawMenuBar();
            DrawMainWinow();

            offset += drag;
            if (offset.x > position.width)
                offset.x = position.width;

            if (offset.x < -gridWidth * 100 * zoom)
                offset.x = -gridWidth * 100 * zoom;

            if (offset.y > position.height - menuBarHeight)
                offset.y = position.height - menuBarHeight;

            if (offset.y < -gridHeight * 100 * zoom)
                offset.y = -gridHeight * 100 * zoom;
        }

        private void DrawMenuBar()
        {
            menuBar = new Rect(0, 0, position.width, menuBarHeight);

            GUILayout.BeginArea(menuBar, EditorStyles.largeLabel);

            if (!createNew)
            {
                GUI.Label(new Rect(5, 2.5f, 80, 20), "Level Data:");

                EditorGUI.BeginChangeCheck();
                levelData = (LevelData)EditorGUI.ObjectField(new Rect(80, 2.5f, 135, 20), levelData, typeof(LevelData), false);
                if(EditorGUI.EndChangeCheck() && levelData != null)
                {
                    ClearArea();
                    UpdateGrid();
                }

                if (GUI.Button(new Rect(10, 30, 80, 20), "Create new"))
                {
                    createNew = true;
                    newLevelName = "";
                }
            }
            else
            {
                GUI.Label(new Rect(5, 2.5f, 80, 20), "Level Name:");

                newLevelName = GUI.TextField(new Rect(85, 2.5f, 120, 20), newLevelName);

                if (newLevelName == "")
                {
                    EditorGUI.HelpBox(new Rect(10, 30, 130, 20), "Level Name needed.", MessageType.Info);
                }
                else if (GUI.Button(new Rect(10, 30, 80, 20), "Create"))
                {
                    SaveData();
                    createNew = false;
                }

                if (GUI.Button(new Rect(150, 30, 80, 20), "Cancel"))
                {
                    createNew = false;
                }

            }

            if (GUI.Button(new Rect(260, 2.5f, 120, 20), "Create Tiles"))
            {
                CreateTiles();
                levelData.hasDeletedTiles = false;
            }

            GUI.Label(new Rect(240, 30, 45, 20), "width:");
            grid.x = EditorGUI.IntField(new Rect(280, 30, 30, 20), (int)grid.x);

            GUI.Label(new Rect(320, 30, 45, 20), "height:");
            grid.y = EditorGUI.IntField(new Rect(365, 30, 30, 20), (int)grid.y);

            if (GUI.Button(new Rect(430, 2.5f, 100, 20), "New Area"))
            {
                NewArea();
            }
            GUI.Label(new Rect(430, 30f, 55, 20), " Area ID:");
            areaID = EditorGUI.IntField(new Rect(490, 30, 40, 20), areaID);
            if (areaID > areas.Count + 1)
            {
                areaID = areas.Count + 1;
            }

            GUI.backgroundColor = red;
            if (currentColor == red)
            {
                if (GUI.Button(new Rect(565, 2.5f, 20, 20), "O"))
                {
                    currentColor = red;
                    currentMode = Tile.Mode.Little;
                }
            }
            else
            {
                if (GUI.Button(new Rect(565, 2.5f, 20, 20), ""))
                {
                    currentColor = red;
                    currentMode = Tile.Mode.Little;

                }
            }

            GUI.backgroundColor = green;
            if (currentColor == green)
            {
                if (GUI.Button(new Rect(590, 2.5f, 20, 20), "O"))
                {
                    currentColor = green;
                    currentMode = Tile.Mode.Sphere;

                }
            }
            else
            {
                if (GUI.Button(new Rect(590, 2.5f, 20, 20), ""))
                {
                    currentColor = green;
                    currentMode = Tile.Mode.Sphere;

                }
            }

            GUI.backgroundColor = blue;
            if (currentColor == blue)
            {
                if (GUI.Button(new Rect(565, 30, 20, 20), "O"))
                {
                    currentColor = blue;
                    currentMode = Tile.Mode.Rectangle;

                }
            }
            else
            {
                if (GUI.Button(new Rect(565, 30, 20, 20), ""))
                {
                    currentColor = blue;
                    currentMode = Tile.Mode.Rectangle;

                }
            }

            GUI.backgroundColor = yellow;
            if (currentColor == yellow)
            {
                if (GUI.Button(new Rect(590, 30, 20, 20), "O"))
                {
                    currentColor = yellow;
                    currentMode = Tile.Mode.Spickey;

                }
            }
            else
            {
                if (GUI.Button(new Rect(590, 30, 20, 20), ""))
                {
                    currentColor = yellow;
                    currentMode = Tile.Mode.Spickey;

                }
            }

            GUI.backgroundColor = Color.white;
            if (GUI.Button(new Rect(645, 2.5f, 50, 20), " Clear"))
            {
                ClearArea();
                levelData.hasDeletedTiles = false;
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0, menuBarHeight, position.width, 1), borderStyle);
            GUILayout.EndArea();

        }

        private void DrawMainWinow()
        {
            mainPanel = new Rect(0, menuBarHeight, position.width, position.height - menuBarHeight);

            GUILayout.BeginArea(mainPanel);

            DrawGrid(20 * zoom, 0.2f * zoom, Color.grey);
            DrawGrid(100 * zoom, 0.4f * zoom, Color.grey);

            ProcessEvents(Event.current);

            if (tiles != null)
            {
                DrawTiles();
            }
            GUILayout.EndArea();
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            Vector3 newOffset = new(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        drag = e.delta;
                        GUI.changed = true;
                    }
                    break;
                case EventType.ContextClick:
                    if(levelData != null)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("New Area"), false, NewArea);
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Red Area Color"), false, SetColor, red);
                        menu.AddItem(new GUIContent("Green Area Color"), false, SetColor, green);
                        menu.AddItem(new GUIContent("Blue Area Color"), false, SetColor, blue);
                        menu.AddItem(new GUIContent("Yellow Area Color"), false, SetColor, yellow);
                        menu.AddSeparator("");
                        if (levelData.hasDeletedTiles)
                            menu.AddItem(new GUIContent("Restore all deleted Tiles"), false, RestoreTiles);
                        else
                            menu.AddDisabledItem(new GUIContent("Restore all deleted Tiles"));
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Clear Area"), false, ClearArea);

                        menu.ShowAsContext();
                    }
                    break;
            }

            if (e.type == EventType.ScrollWheel)
            {
                zoom += -e.delta.y / 100;

                skin.button.fontSize = (int)(40 * zoom);
                if (skin.button.fontSize < 12)
                {
                    skin.button.fontSize = 12;
                }

                GUI.changed = true;
            }
        }

        private void DrawTiles()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == null)
                    tiles[i] = new Tile();

                if (tiles[i].active)
                {
                    tiles[i].coordinates = new Vector2(i % grid.x, grid.y - Mathf.Floor(i / grid.x) - 1);
                    tiles[i].rect = new Rect((i % gridWidth) * 100 * zoom + 2.5f + offset.x,
                            ((int)i / gridWidth) * 100 * zoom + 2.5f + offset.y,
                            95 * zoom,
                            95 * zoom);

                    GUI.backgroundColor = tiles[i].color;
                    GUI.skin = skin;
                    Event e = Event.current;
                    if (GUI.Button(tiles[i].rect, tiles[i].text))
                    {
                        if (e.button == 0)
                        {
                            int removeIndex = -1;
                            foreach (List<Tile> tileList in areas)
                            {
                                foreach (Tile tile in tileList)
                                {
                                    if (tile == tiles[i])
                                        removeIndex = tile.areaIndex;
                                }
                            }

                            if (removeIndex != -1)
                            {
                                areas[removeIndex].Remove(tiles[i]);
                            }

                            tiles[i].color = currentColor;
                            tiles[i].playerMode = currentMode;
                            tiles[i].areaIndex = areaID;
                            tiles[i].text = areaID.ToString();

                            if (areas.Count < areaID)
                            {
                                areas.Add(new List<Tile>());
                            }
                            areas[areaID].Add(tiles[i]);

                            UpdateData();

                        }
                        else if (e.button == 1)
                        {
                            GenericMenu tileMenu = new GenericMenu();

                            tileMenu.AddItem(new GUIContent("Delete Tile"), false, DeleteTile, tiles[i]);

                            tileMenu.ShowAsContext();

                        }
                    }
                }
            }
        }

        private void SaveData()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            LevelData newLevelData = CreateInstance<LevelData>();
            if (AssetDatabase.LoadAssetAtPath(path + "/" + newLevelName + ".asset", typeof(LevelData)) != null)
            {
                AssetDatabase.DeleteAsset(path + "/" + newLevelName + ".asset");
            }

            newLevelData.Init(areas);
            newLevelData.grid = grid;

            AssetDatabase.CreateAsset(newLevelData, path + "/" + newLevelName + ".asset");

            levelData = newLevelData;
            Debug.Log("RectangleBuilder: Created '" + newLevelName + ".asset' at '" + path + "'");
        }

        private void UpdateData()
        {
            levelData.Init(areas);
            levelData.grid = new Vector2(gridWidth, gridHeight);
        }

        private void UpdateGrid()
        {
            areas = levelData.GetAreaList();

            int count = 0;
            foreach (List<Tile> tileList in areas)
            {
                count += tileList.Count;
            }

            tiles = new Tile[count];

            foreach (List<Tile> tileList in areas)
            {
                foreach (Tile tile in tileList)
                {
                    tiles[tile.tileIndex] = tile;
                }
            }

            gridWidth = (int)levelData.grid.x;
            gridHeight = (int)levelData.grid.y;

        }

        private void CreateTiles()
        {
            gridWidth = (int)grid.x;
            gridHeight = (int)grid.y;

            tiles = new Tile[(int)grid.x * (int)grid.y];

            for (int i = 0; i < grid.y; i++)
            {
                for (int n = 0; n < grid.x; n++)
                {
                    int index = i * (int)grid.x + n;

                    tiles[index] = new Tile();

                    tiles[index].rect = new Rect(i * 100 * zoom + 2.5f + offset.x,
                    n * 100 * zoom + 2.5f + offset.y,
                    95 * zoom,
                    95 * zoom);

                    tiles[index].tileIndex = index;
                    tiles[index].text = tiles[index].areaIndex.ToString();
                }
            }

            UpdateData();
        }

        private void NewArea()
        {
            if (areas == null)
            {
                areas = new List<List<Tile>>();
                areas.Add(new List<Tile>());
            }


            if (areas[areaID].Count != 0)
            {
                areaID = areas.Count;
                areas.Add(new List<Tile>());
            }
        }

        private void SetColor(object color)
        {
            currentColor = (Color)color;
            if((Color)color == red)
            {
                currentMode = Tile.Mode.Little;
            }
            else if((Color)color == blue)
            {
                currentMode = Tile.Mode.Rectangle;
            }
            else if ((Color)color == green)
            {
                currentMode = Tile.Mode.Sphere;
            }
            else if ((Color)color == yellow)
            {
                currentMode = Tile.Mode.Spickey;
            }

        }

        private void ClearArea()
        {
            if (areas != null)
            {
                areas = new List<List<Tile>>();
                areas.Add(new List<Tile>());
                areaID = 0;
            }

            if ( tiles != null)
            {
                tiles = new Tile[0];
            }
        }

        private void DeleteTile(object tile)
        {
            Tile tileToDelete = (Tile)tile;

            tileToDelete.active = false;
            levelData.hasDeletedTiles = true;

        }

        private void RestoreTiles()
        {
            foreach(Tile tile in tiles)
            {
                tile.active = true;
            }
        }
    }
}
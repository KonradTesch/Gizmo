using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Rectangle.LevelCreation
{
    public class GridCreatorWindow : EditorWindow
    {
        private Rect menuBar;
        private Rect sideBar;
        private Rect mainPanel;

        private readonly float menuBarHeight = 55f;
        private readonly float sideBarWidth = 50f;
        private readonly string path = "Assets/LevelBuilder/LevelData";

        private LevelData levelData;
        private string newLevelName;
        private bool createNew = false;
        private int gridWidth;
        private int gridHeight;

        private Color blue = new(0.439f, 0.427f, 0.937f);
        private Color green = new(0.314f, 0.745f, 0.302f);
        private Color yellow = new(0.972f, 0.702f, 0.227f);
        private Color currentColor;

        private TileCreator.TileTypes currentTileType;
        private Player.PlayerController.PlayerModes currentMode;
        private AnchorTile currentAnchor;


        private GUIStyle borderStyle = new GUIStyle();

        private Vector2 offset;
        private Vector2 drag;
        private float zoom = 1;
        private GUISkin skin;

        [MenuItem("Rectangle/Level Builder")]
        public static void OpenWindow()
        {
            GetWindow<GridCreatorWindow>("Rectangle Level Builder");
        }

        private void OnEnable()
        {
            skin = CreateInstance<GUISkin>();
            skin.button.alignment = TextAnchor.MiddleCenter;
            skin.button.normal.textColor = Color.white;
            skin.button.fontSize = (int)(60 * zoom);
            if (skin.button.fontSize < 16)
            {
                skin.button.fontSize = 16;
            }

            borderStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
        }

        private void OnGUI()
        {
            DrawMenuBar();

            InitLevelData();

            DrawSideBar();
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
                if (EditorGUI.EndChangeCheck() && levelData != null)
                {
                    gridWidth = levelData.gridData.width;
                    gridHeight = levelData.gridData.height;
                }

                if (GUI.Button(new Rect(10, 30, 80, 20), "Create new"))
                {
                    createNew = true;
                    newLevelName = "";
                }

                if (GUI.Button(new Rect(100, 30, 80, 20), "Save"))
                {
                    SaveData();
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
                    CreateLevelData();
                    createNew = false;
                    currentAnchor = null;
                }

                if (GUI.Button(new Rect(150, 30, 80, 20), "Cancel"))
                {
                    createNew = false;
                }

            }

            if (GUI.Button(new Rect(260, 2.5f, 120, 20), "Create new Grid"))
            {
                ClearGrid();
                CreateGrid();
                SaveData();
            }

            GUI.Label(new Rect(240, 30, 45, 20), "width:");
            gridWidth = EditorGUI.IntField(new Rect(280, 30, 30, 20), gridWidth);

            GUI.Label(new Rect(320, 30, 45, 20), "height:");
            gridHeight = EditorGUI.IntField(new Rect(365, 30, 30, 20), gridHeight);


            GUI.Label(new Rect(410, 2.5f, 100, 20), "current Anchor:");

            GUI.backgroundColor = Color.black;
            if (currentAnchor == null)
            {
                if (GUI.Button(new Rect(415, 30, 20, 20), "O")) { };
            }
            else
            {
                if (GUI.Button(new Rect(415, 30, 20, 20), ""))
                {
                    currentAnchor = null;
                    SaveData();
                }
            }
            if(levelData != null && levelData.gridData != null && levelData.gridData.anchorTiles != null)
            {
                for (int i = 0; i < levelData.gridData.anchorTiles.Count; i++)
                {
                    GUI.backgroundColor = GetAnchorColor(levelData.gridData.anchorTiles[i].anchorCoordinates);

                    if (currentAnchor == levelData.gridData.anchorTiles[i])
                    {
                        if (GUI.Button(new Rect(440 + (i * 25), 30, 20, 20), "O")) { };
                    }
                    else
                    {
                        if (GUI.Button(new Rect(440 + (i * 25), 30, 20, 20), ""))
                        {
                            currentAnchor = levelData.gridData.anchorTiles[i];
                            SaveData();
                        }
                    }
                }
            }
            GUI.backgroundColor = Color.white;

            if (GUI.Button(new Rect(560, 2.5f, 50, 20), " Clear"))
            {
                ClearGrid();
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0, menuBarHeight, position.width, 1), borderStyle);
            GUILayout.EndArea();

        }

        private void DrawSideBar()
        {
            sideBar = new Rect(0, menuBarHeight, sideBarWidth, position.height - menuBarHeight);

            GUILayout.BeginArea(sideBar, EditorStyles.largeLabel);

            GUI.backgroundColor = blue;
            if (currentColor == blue)
            {
                if (GUI.Button(new Rect(10, 20, 30, 30), "O"))
                {
                    currentColor = blue;
                    currentMode = Player.PlayerController.PlayerModes.Rectangle;
                }
            }
            else
            {
                if (GUI.Button(new Rect(10, 20, 30, 30), ""))
                {
                    currentColor = blue;
                    currentMode = Player.PlayerController.PlayerModes.Rectangle;
                    SaveData();
                }
            }

            GUI.backgroundColor = green;
            if (currentColor == green)
            {
                if (GUI.Button(new Rect(10, 60, 30, 30), "O"))
                {
                    currentColor = green;
                    currentMode = Player.PlayerController.PlayerModes.Bubble;

                }
            }
            else
            {
                if (GUI.Button(new Rect(10, 60, 30, 30), ""))
                {
                    currentColor = green;
                    currentMode = Player.PlayerController.PlayerModes.Bubble;
                    SaveData();
                }
            }

            GUI.backgroundColor = yellow;
            if (currentColor == yellow)
            {
                if (GUI.Button(new Rect(10, 100, 30, 30), "O"))
                {
                    currentColor = yellow;
                    currentMode = Player.PlayerController.PlayerModes.Spikey;

                }
            }
            else
            {
                if (GUI.Button(new Rect(10, 100, 30, 30), ""))
                {
                    currentColor = yellow;
                    currentMode = Player.PlayerController.PlayerModes.Spikey;
                    SaveData();
                }
            }

            GUI.backgroundColor = currentColor;
            GUI.contentColor = Color.grey;

            if (currentTileType == TileCreator.TileTypes.LeftAndRight)
                GUI.contentColor = Color.black;

            if (GUI.Button(new Rect(10, 150, 30, 30), (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_l+r.png", typeof(Texture))))
            {
                currentTileType = TileCreator.TileTypes.LeftAndRight;
            }
            GUI.contentColor = Color.grey;

            if (currentTileType == TileCreator.TileTypes.LeftAndDown)
                GUI.contentColor = Color.black;

            if (GUI.Button(new Rect(10, 190, 30, 30), (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_l+d.png", typeof(Texture))))
            {
                currentTileType = TileCreator.TileTypes.LeftAndDown;
            }
            GUI.contentColor = Color.grey;

            if (currentTileType == TileCreator.TileTypes.LeftAndUp)
                GUI.contentColor = Color.black;

            if (GUI.Button(new Rect(10, 230, 30, 30), (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_l+u.png", typeof(Texture))))
            {
                currentTileType = TileCreator.TileTypes.LeftAndUp;
            }
            GUI.contentColor = Color.grey;

            if (currentTileType == TileCreator.TileTypes.DownAndRight)
                GUI.contentColor = Color.black;

            if (GUI.Button(new Rect(10, 270, 30, 30), (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_d+r.png", typeof(Texture))))
            {
                currentTileType = TileCreator.TileTypes.DownAndRight;
            }
            GUI.contentColor = Color.grey;

            if (currentTileType == TileCreator.TileTypes.UpAndRight)
                GUI.contentColor = Color.black;

            if (GUI.Button(new Rect(10, 310, 30, 30), (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_u+r.png", typeof(Texture))))
            {
                currentTileType = TileCreator.TileTypes.UpAndRight;
            }
            GUI.contentColor = Color.grey;

            if (currentTileType == TileCreator.TileTypes.UpAndDown)
                GUI.contentColor = Color.black;

            if (GUI.Button(new Rect(10, 350, 30, 30), (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_u+d.png", typeof(Texture))))
            {
                currentTileType = TileCreator.TileTypes.UpAndDown;
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(sideBarWidth, menuBarHeight, 1, position.height), borderStyle);
            GUILayout.EndArea();

        }

        private void DrawMainWinow()
        {
            mainPanel = new Rect(sideBarWidth, menuBarHeight, position.width - sideBarWidth, position.height - menuBarHeight);

            GUILayout.BeginArea(mainPanel);

            DrawGrid(20 * zoom, 0.2f * zoom, Color.grey);
            DrawGrid(100 * zoom, 0.4f * zoom, Color.grey);
            if(levelData != null)
            {
                ProcessEvents(Event.current);

                if (levelData.gridData.grid != null && levelData.gridData.grid.Count > 0)
                {
                    DrawTiles();
                }
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
                    if (levelData != null)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Bubble Mode"), false, SetMode, green);
                        menu.AddItem(new GUIContent("rectangle Mode"), false, SetMode, blue);
                        menu.AddItem(new GUIContent("Spikey Mode"), false, SetMode, yellow);
                        menu.AddSeparator("");
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Clear Area"), false, ClearGrid);

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

            foreach(GridSpot gridSpot in levelData.gridData.grid)
            {
                GUI.skin.button.fontStyle = FontStyle.Normal;

                bool anchor = false;
                bool start = false;
                bool end = false;
                bool star = false;

                bool pressed = false;

                Vector2 coordinates = gridSpot.coordinates * 100 + new Vector2(5, 5);

                Rect tileRect = new Rect(coordinates.x * zoom + offset.x,
                        (gridHeight) - coordinates.y * zoom + offset.y,
                        95 * zoom,
                        95 * zoom);
                Texture tileTexture = null;

                if (levelData.GetTileByCoordinates(gridSpot.coordinates, currentAnchor) != null)
                {
                    switch (levelData.GetTileByCoordinates(gridSpot.coordinates, currentAnchor).playerMode)
                    {
                        case Player.PlayerController.PlayerModes.Rectangle:
                            GUI.backgroundColor = blue;
                            break;
                        case Player.PlayerController.PlayerModes.Spikey:
                            GUI.backgroundColor = yellow;
                            break;
                        case Player.PlayerController.PlayerModes.Bubble:
                            GUI.backgroundColor = green;
                            break;
                    }
                    switch(levelData.GetTileByCoordinates(gridSpot.coordinates, currentAnchor).tileType)
                    {
                        case TileCreator.TileTypes.LeftAndRight:
                            tileTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_l+r.png", typeof(Texture));
                            break;
                        case TileCreator.TileTypes.LeftAndDown:
                            tileTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_l+d.png", typeof(Texture));
                            break;
                        case TileCreator.TileTypes.LeftAndUp:
                            tileTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_l+u.png", typeof(Texture));
                            break;
                        case TileCreator.TileTypes.UpAndDown:
                            tileTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_u+d.png", typeof(Texture));
                            break;
                        case TileCreator.TileTypes.DownAndRight:
                            tileTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_d+r.png", typeof(Texture));
                            break;
                        case TileCreator.TileTypes.UpAndRight:
                            tileTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/EditorTile_u+r.png", typeof(Texture));
                            break;
                    }
                    if(currentAnchor != null)
                    {
                        GUI.contentColor = GetAnchorColor(currentAnchor.anchorCoordinates);
                    }
                    else
                    {
                        GUI.contentColor = Color.black;
                    }
                }
                else if (gridSpot.levelSpot.blocked)
                {
                    GUI.backgroundColor = Color.black;
                }
                else if (gridSpot.levelSpot.anchor)
                {
                    anchor = true;
                    GUI.backgroundColor = GetAnchorColor(gridSpot.coordinates);
                }
                else
                {
                    GUI.backgroundColor = Color.grey;
                }

                if (gridSpot.coordinates == levelData.gridData.start.coordinates)
                {
                    start = true;
                    GUI.backgroundColor = Color.green;
                    tileTexture = GetDirectionTexture(levelData.gridData.start.direction);
                }
                else if (gridSpot.coordinates == levelData.gridData.end.coordinates)
                {
                    end = true;
                    GUI.backgroundColor = Color.blue;
                    tileTexture = GetDirectionTexture(levelData.gridData.end.direction);

                }

                if (gridSpot.levelSpot.star)
                {
                    star = true;
                    GUI.contentColor = Color.white;
                    if(levelData.GetTileByCoordinates(gridSpot.coordinates, currentAnchor) == null)
                    {
                        tileTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/GridEditor_Star.png", typeof(Texture));
                    }
                }

                Event e = Event.current;

                if(anchor)
                {
                    if (levelData.GetAnchorByCoordinates(gridSpot.coordinates) == currentAnchor)
                    {
                        GUI.skin.button.fontStyle = FontStyle.Bold;
                        GUI.skin.button.fontSize += 2;

                    }

                    if (GUI.Button(tileRect, "Anchor"))
                    {
                        pressed = true;
                    }

                    if (levelData.GetAnchorByCoordinates(gridSpot.coordinates) == currentAnchor)
                    {
                        GUI.skin.button.fontStyle = FontStyle.Normal;
                        GUI.skin.button.fontSize -= 2;

                    }


                }
                else if(start)
                {
                    if (GUI.Button(tileRect, tileTexture))
                    {
                        pressed = true;
                    }
                }
                else if (end)
                {
                    if (GUI.Button(tileRect, tileTexture))
                    {
                        pressed = true;
                    }
                }
                else if (star && levelData.GetTileByCoordinates(gridSpot.coordinates, currentAnchor) == null)
                {
                    if (GUI.Button(tileRect, tileTexture))
                    {
                        pressed = true;
                    }
                }
                else
                {
                    if (GUI.Button(tileRect, tileTexture))
                    {
                        pressed = true;
                    }

                    GUI.contentColor = Color.white;
                }


                if (pressed)
                {
                    if (e.button == 0)
                    {
                        if (!gridSpot.levelSpot.anchor && !gridSpot.levelSpot.blocked)
                        {
                            PlannedTile plannedTile = new PlannedTile()
                            {
                                coordinates = gridSpot.coordinates,
                                playerMode = currentMode,
                                tileType = currentTileType,
                            };

                            if (currentAnchor != null)
                            {
                                //Adds the planned tile to the collectable Tiles of the current anchor.
                                levelData.ChangeCollectableTile(currentAnchor, plannedTile);
                            }
                            else
                            {
                                //Adds the planned Tile to the planned-tile.list of gridData.
                                levelData.ChangePlannedTile(plannedTile);
                            }

                        }
                        else if(gridSpot.levelSpot.anchor)
                        {
                            currentAnchor = levelData.GetAnchorByCoordinates(gridSpot.coordinates);
                        }
                        else if(gridSpot.coordinates == levelData.gridData.start.coordinates)
                        {
                            levelData.gridData.start.direction = RotateDirection(levelData.gridData.start.direction);
                        }
                        else if (gridSpot.coordinates == levelData.gridData.end.coordinates)
                        {
                            levelData.gridData.end.direction = RotateDirection(levelData.gridData.end.direction);
                        }

                    }
                    else if (e.button == 1)
                    {
                        ContextMenu(gridSpot);
                    }

                }
            }
        }

        private void ContextMenu(GridSpot gridSpot)
        {
            GenericMenu tileMenu = new GenericMenu();
            if (gridSpot.coordinates.x < 0 || gridSpot.coordinates.y < 0 || gridSpot.coordinates.x == levelData.gridData.width || gridSpot.coordinates.y == levelData.gridData.height)
            {
                tileMenu.AddItem(new GUIContent("Set Start"), false, SetStart, gridSpot.coordinates);
                tileMenu.AddItem(new GUIContent("Set End"), false, SetEnd, gridSpot.coordinates);
            }
            else
            {
                if (gridSpot.levelSpot.anchor)
                {
                    tileMenu.AddItem(new GUIContent("Clear AnchorTile"), false, ClearAnchor, gridSpot.coordinates);
                }
                else
                {
                    tileMenu.AddItem(new GUIContent("Set AnchorTile"), false, SetAnchor, gridSpot.coordinates);
                }

                if (gridSpot.levelSpot.star)
                {
                    tileMenu.AddItem(new GUIContent("Clear Star"), false, ClearStar, gridSpot.coordinates);
                }
                else
                {
                    tileMenu.AddItem(new GUIContent("Set Star"), false, SetStar, gridSpot.coordinates);
                }

                if (gridSpot.levelSpot.blocked)
                {
                    tileMenu.AddItem(new GUIContent("Unblock Tile"), false, UnblockTile, gridSpot.coordinates);
                    tileMenu.AddSeparator("");
                    tileMenu.AddItem(new GUIContent("Set Start"), false, SetStart, gridSpot.coordinates);
                    tileMenu.AddItem(new GUIContent("Set End"), false, SetEnd, gridSpot.coordinates);
                }
                else
                {
                    tileMenu.AddItem(new GUIContent("Block Tile"), false, BlockTile, gridSpot.coordinates);

                }

                if (levelData.GetTileByCoordinates(gridSpot.coordinates, currentAnchor) != null)
                {
                    tileMenu.AddItem(new GUIContent("Remove Tile"), false, RemoveTile, gridSpot.coordinates);
                }
            }

            tileMenu.ShowAsContext();
        }

        private void InitLevelData()
        {
            if (levelData != null)
            {
                if (levelData.gridData == null)
                {
                    levelData.gridData = new LevelGrid();
                }

                if (levelData.plannedTiles == null)
                {
                    levelData.plannedTiles = new();
                }

                if (levelData.gridData.grid == null)
                {
                    levelData.gridData.grid = new();
                }

                if (levelData.gridData.anchorTiles == null)
                {
                    levelData.gridData.anchorTiles = new();
                }

                if(levelData.gridData.start == null)
                {
                    levelData.gridData.start = new TileDirection(new Vector2Int(-1, -1), new Vector2Int(1, 0));
                }

                if (levelData.gridData.end == null)
                {
                    levelData.gridData.end = new TileDirection(new Vector2Int(-1, -1), new Vector2Int(1, 0));
                }

            }
        }

        private void CreateLevelData()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (AssetDatabase.LoadAssetAtPath(path + "/" + newLevelName + ".asset", typeof(LevelData)) != null)
            {
                AssetDatabase.DeleteAsset(path + "/" + newLevelName + ".asset");
            }

            levelData = (LevelData)CreateInstance(typeof(LevelData));

            levelData.gridData = new();
            levelData.gridData.grid = new();
            levelData.plannedTiles = new();
            levelData.gridData.anchorTiles = new();

            AssetDatabase.CreateAsset(levelData, path + "/" + newLevelName + ".asset");

            Debug.Log("RectangleBuilder: Created '" + newLevelName + ".asset' at '" + path + "'");
        }

        private void SaveData()
        {
            EditorUtility.SetDirty(levelData);

            AssetDatabase.SaveAssetIfDirty(levelData);
        }
        private void CreateGrid()
        {
            List<GridSpot> newGrid = new();

            for (int x = -1; x < gridWidth + 1; x++)
            {
                for (int y = -1; y < gridHeight + 1; y++)
                {
                    if(x < 0 || x == gridWidth || y < 0 || y == gridHeight)
                    {
                        newGrid.Add(new GridSpot(new Vector2Int(x, y), new LevelSpot(new Vector2Int(x, y), true, false)));
                    }
                    else
                    {
                        newGrid.Add(new GridSpot(new Vector2Int(x, y), new LevelSpot(new Vector2Int(x, y), false, false)));
                    }
                }
            }

            levelData.gridData.grid = newGrid;

            levelData.gridData.width = gridWidth;
            levelData.gridData.height = gridHeight;
        }

        private void SetMode(object color)
        {
            currentColor = (Color)color;
            if ((Color)color == blue)
            {
                currentMode = Player.PlayerController.PlayerModes.Rectangle;
            }
            else if ((Color)color == green)
            {
                currentMode = Player.PlayerController.PlayerModes.Bubble;
            }
            else if ((Color)color == yellow)
            {
                currentMode = Player.PlayerController.PlayerModes.Spikey;
            }

        }

        private void ClearGrid()
        {
            levelData.gridData = new LevelGrid();
            levelData.gridData.grid = new();
            levelData.plannedTiles = new();
            levelData.gridData.anchorTiles = new();
        }

        private void BlockTile(object coordinates)
        {
            foreach(AnchorTile anchor in levelData.gridData.anchorTiles)
            {
                if (levelData.GetTileByCoordinates((Vector2Int)coordinates, anchor) != null)
                {
                    levelData.RemoveTile(levelData.GetTileByCoordinates((Vector2Int)coordinates, anchor), anchor);
                }
            }

            if(levelData.GetAnchorByCoordinates((Vector2Int)coordinates) != null)
            {
                levelData.gridData.anchorTiles.Remove(levelData.GetAnchorByCoordinates((Vector2Int)coordinates));
            }

            levelData.GetGridSpot((Vector2Int)coordinates).blocked = true;
            levelData.GetGridSpot((Vector2Int)coordinates).anchor = false;
            levelData.GetGridSpot((Vector2Int)coordinates).star = false;
        }

        private void UnblockTile(object coordinates)
        {
            if(levelData.gridData.start.coordinates == (Vector2Int)coordinates)
            {
                levelData.gridData.start.coordinates = new Vector2Int(-1, -1);
            }

            if (levelData.gridData.end.coordinates == (Vector2Int)coordinates)
            {
                levelData.gridData.end.coordinates = new Vector2Int(gridWidth, gridHeight);
            }


            levelData.GetGridSpot((Vector2Int)coordinates).blocked = false;
        }

        private void SetAnchor(object coordinates)
        {
            foreach (AnchorTile anchor in levelData.gridData.anchorTiles)
            {
                if (levelData.GetTileByCoordinates((Vector2Int)coordinates, anchor) != null)
                {
                    levelData.RemoveTile(levelData.GetTileByCoordinates((Vector2Int)coordinates, anchor), anchor);
                }
            }
            levelData.GetGridSpot((Vector2Int)coordinates).blocked = false;
            levelData.GetGridSpot((Vector2Int)coordinates).star = false;
            levelData.GetGridSpot((Vector2Int)coordinates).anchor = true;

            if(levelData.gridData.anchorTiles == null)
            {
                levelData.gridData.anchorTiles = new();
            }
            levelData.gridData.anchorTiles.Add(new AnchorTile() {anchorCoordinates = (Vector2Int)coordinates});
        }

        private void RemoveTile(object coordinates)
        {
            if(levelData.GetTileByCoordinates((Vector2Int)coordinates, currentAnchor) != null)
            {
                levelData.RemoveTile(levelData.GetTileByCoordinates((Vector2Int)coordinates, currentAnchor), currentAnchor);
            }
        }


        private void ClearAnchor(object coordinates)
        {
            levelData.GetGridSpot((Vector2Int)coordinates).anchor = false;
            levelData.gridData.anchorTiles.Remove(levelData.GetAnchorByCoordinates((Vector2Int)coordinates));
        }

        private void SetStar(object coordinates)
        {
            levelData.GetGridSpot((Vector2Int)coordinates).blocked = false;
            levelData.GetGridSpot((Vector2Int)coordinates).anchor = false;
            levelData.GetGridSpot((Vector2Int)coordinates).star = true;
        }
        private void ClearStar(object coordinate)
        {
            levelData.GetGridSpot((Vector2Int)coordinate).star = false;
        }

        private Color GetAnchorColor(Vector2Int coordinates)
        {
            Color anchorColor = Color.gray;
            for(int i = 0; i < levelData.gridData.anchorTiles.Count; i++)
            {
                if (levelData.gridData.anchorTiles[i].anchorCoordinates == coordinates)
                {
                    anchorColor = new Color(1 - (i / 3f), 0, i / 3f);

                    break;
                }
            }
            return anchorColor;
        }

        private void SetStart(object coordinates)
        {
            levelData.gridData.start.coordinates = (Vector2Int)coordinates;
            levelData.gridData.start.direction = FindFreeDirection((Vector2Int)coordinates);
        }

        private void SetEnd(object coordinates)
        {
            levelData.gridData.end.coordinates = (Vector2Int)coordinates;
            levelData.gridData.end.direction = FindFreeDirection((Vector2Int)coordinates);
        }

        private Vector2Int FindFreeDirection(Vector2Int coordinates)
        {
            if(levelData.GetGridSpot(coordinates + Vector2Int.right) != null && !levelData.GetGridSpot(coordinates + Vector2Int.right).blocked)
            {
                return Vector2Int.right;
            }
            else if (levelData.GetGridSpot(coordinates + Vector2Int.left) != null && !levelData.GetGridSpot(coordinates + Vector2Int.left).blocked)
            {
                return Vector2Int.left;
            }
            else if (levelData.GetGridSpot(coordinates + Vector2Int.up) != null && !levelData.GetGridSpot(coordinates + Vector2Int.up).blocked)
            {
                return Vector2Int.up;
            }
            else if (levelData.GetGridSpot(coordinates + Vector2Int.down) != null && !levelData.GetGridSpot(coordinates + Vector2Int.down).blocked)
            {
                return Vector2Int.down;
            }

            return Vector2Int.right;
        }

        private Texture GetDirectionTexture(Vector2Int direction)
        {
            if(direction == Vector2Int.right)
            {
                return (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/GridEditor_Arrow_right.png", typeof(Texture));
            }
            else if (direction == Vector2Int.down)
            {
                return (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/GridEditor_Arrow_down.png", typeof(Texture));
            }
            else if (direction == Vector2Int.left)
            {
                return (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/GridEditor_Arrow_left.png", typeof(Texture));
            }
            else if (direction == Vector2Int.up)
            {
                return (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/GridEditor_Arrow_up.png", typeof(Texture));
            }


            return (Texture)AssetDatabase.LoadAssetAtPath("Assets/LevelBuilder/EditorImages/GridEditor_Arrow_right.png", typeof(Texture));

        }

        private Vector2Int RotateDirection(Vector2Int direction)
        {
            if (direction == Vector2Int.right)
            {
                return Vector2Int.down;
            }
            else if (direction == Vector2Int.down)
            {
                return Vector2Int.left;
            }
            else if (direction == Vector2Int.left)
            {
                return Vector2Int.up;
            }
            else if (direction == Vector2Int.up)
            {
                return Vector2Int.right;
            }

            return Vector2Int.right;

        }

    }
}

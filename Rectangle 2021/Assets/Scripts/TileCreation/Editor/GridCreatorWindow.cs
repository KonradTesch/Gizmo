using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Rectangle.Level;

namespace Rectangle.TileCreation
{
    public class GridCreatorWindow : EditorWindow
    {
        private Rect menuBar;
        private Rect sideBar;
        private Rect mainPanel;

        private readonly float menuBarHeight = 55f;
        private readonly float sideBarWidth = 50f;
        private readonly string path = "Assets/Resources/LevelData";

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
                CreateGrid();
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
                }
            }

            for (int i = 0; i < levelData.gridData.anchorTiles.Count; i++)
            {
                GUI.backgroundColor = GetAnchorColor(levelData.gridData.anchorTiles[i].anchorCoordinates);

                if(currentAnchor == levelData.gridData.anchorTiles[i])
                {
                    if (GUI.Button(new Rect(440 + (i * 25), 30, 20, 20), "O")) { };
                }
                else
                {
                    if (GUI.Button(new Rect(440 + (i * 25), 30, 20, 20), ""))
                    {
                        currentAnchor = levelData.gridData.anchorTiles[i];
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
            foreach(LevelSpot spot in levelData.gridData.grid.Values)
            {

                Vector2 coordinates = spot.coordinates * 100 + new Vector2(5, 5);

                Rect tileRect = new Rect(coordinates.x * zoom + offset.x,
                        coordinates.y * zoom + offset.y,
                        95 * zoom,
                        95 * zoom);
                Texture tileTexture = null;

                if (levelData.GetTileByCoordinates(spot.coordinates, currentAnchor) != null)
                {
                    switch (levelData.GetTileByCoordinates(spot.coordinates, currentAnchor).playerMode)
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
                    switch(levelData.GetTileByCoordinates(spot.coordinates, currentAnchor).tileType)
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
                else if (spot.blocked)
                {
                    GUI.backgroundColor = Color.black;
                }
                else if (spot.anchor)
                {
                    GUI.backgroundColor = GetAnchorColor(spot.coordinates);
                }
                else
                {
                    GUI.backgroundColor = Color.grey;
                }

                if (spot.coordinates == levelData.gridData.start)
                {
                    GUI.backgroundColor = Color.green;
                }
                else if (spot.coordinates == levelData.gridData.end)
                {
                    GUI.backgroundColor = Color.blue;
                }

                if(spot.star)
                {
                    GUI.contentColor = Color.white;
                }

                Event e = Event.current;

                if (GUI.Button(tileRect, tileTexture))
                {
                    if (e.button == 0)
                    {
                        if (!spot.anchor && !spot.blocked)
                        {
                            PlannedTile plannedTile = new PlannedTile()
                            {
                                coordinates = spot.coordinates,
                                playerMode = currentMode,
                                tileType = currentTileType,
                                anchor = currentAnchor
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
                    }
                    else if (e.button == 1)
                    {
                        GenericMenu tileMenu = new GenericMenu();
                        if(spot.coordinates.x < 0 || spot.coordinates.y < 0 || spot.coordinates.x == levelData.gridData.width || spot.coordinates.y == levelData.gridData.height)
                        {
                            tileMenu.AddItem(new GUIContent("Set Start"), false, SetStart, spot.coordinates);
                            tileMenu.AddItem(new GUIContent("SetEnd"), false, SetEnd, spot.coordinates);
                        }
                        else
                        {
                            if (spot.anchor)
                            {
                                tileMenu.AddItem(new GUIContent("Clear AnchorTile"), false, ClearAnchor, spot.coordinates);
                            }
                            else
                            {
                                tileMenu.AddItem(new GUIContent("Set AnchorTile"), false, SetAnchor, spot.coordinates);
                            }

                            if (spot.star)
                            {
                                tileMenu.AddItem(new GUIContent("Clear Star"), false, ClearStar, spot.coordinates);
                            }
                            else
                            {
                                tileMenu.AddItem(new GUIContent("Set Star"), false, SetStar, spot.coordinates);
                            }

                            if (spot.blocked)
                            {
                                tileMenu.AddItem(new GUIContent("Unblock Tile"), false, UnblockTile, spot.coordinates);
                            }
                            else
                            {
                                tileMenu.AddItem(new GUIContent("Block Tile"), false, BlockTile, spot.coordinates);

                            }

                            if (levelData.GetTileByCoordinates(spot.coordinates, currentAnchor) != null)
                            {
                                tileMenu.AddItem(new GUIContent("Remove Tile"), false, RemoveTile, spot.coordinates);
                            }

                        }

                        tileMenu.ShowAsContext();

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

            newLevelData.gridData = new LevelGrid();
            newLevelData.gridData.grid = new();
            newLevelData.plannedTiles = new();

            AssetDatabase.CreateAsset(newLevelData, path + "/" + newLevelName + ".asset");

            levelData = newLevelData;
            Debug.Log("RectangleBuilder: Created '" + newLevelName + ".asset' at '" + path + "'");
        }

        private void CreateGrid()
        {
            Dictionary<Vector2Int, LevelSpot> newGrid = new();

            for (int x = -1; x < gridWidth + 1; x++)
            {
                for (int y = -1; y < gridHeight + 1; y++)
                {
                    if(x < 0 || x == gridWidth || y < 0 || y == gridHeight)
                    {
                        newGrid.Add(new Vector2Int(x, y), new LevelSpot(new Vector2Int(x, y), true, false));
                    }
                    else
                    {
                        newGrid.Add(new Vector2Int(x, y), new LevelSpot(new Vector2Int(x, y), false, false));
                    }
                }
            }

            levelData.gridData.grid = newGrid;

            levelData.gridData.width = gridWidth;
            levelData.gridData.height = gridHeight;

            //UpdateData();
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
            levelData.gridData.grid[(Vector2Int)coordinates].blocked = true;
            levelData.gridData.grid[(Vector2Int)coordinates].anchor = false;
            levelData.gridData.grid[(Vector2Int)coordinates].star = false;
        }

        private void UnblockTile(object coordinates)
        {
            levelData.gridData.grid[(Vector2Int)coordinates].blocked = false;
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
            levelData.gridData.grid[(Vector2Int)coordinates].blocked = false;
            levelData.gridData.grid[(Vector2Int)coordinates].star = false;
            levelData.gridData.grid[(Vector2Int)coordinates].anchor = true;

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
            levelData.gridData.grid[(Vector2Int)coordinates].anchor = false;
            levelData.gridData.anchorTiles.Remove(levelData.GetAnchorByCoordinates((Vector2Int)coordinates));
        }

        private void SetStar(object coordinates)
        {
            levelData.gridData.grid[(Vector2Int)coordinates].blocked = false;
            levelData.gridData.grid[(Vector2Int)coordinates].anchor = false;
            levelData.gridData.grid[(Vector2Int)coordinates].star = true;
        }
        private void ClearStar(object coordinate)
        {
            levelData.gridData.grid[(Vector2Int)coordinate].star = false;
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
            levelData.gridData.start = (Vector2Int)coordinates;
        }

        private void SetEnd(object coordinates)
        {
            levelData.gridData.end = (Vector2Int)coordinates;
        }

    }
}

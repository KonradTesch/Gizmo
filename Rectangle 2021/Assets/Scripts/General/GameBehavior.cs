using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Rectangle.Player;
using Rectangle.UI;
using Rectangle.Level;
using Rectangle.LevelCreation;

namespace Rectangle.General
{
    [RequireComponent(typeof(LevelBuilder))]
    [RequireComponent(typeof(TileBuilder))]
    /// <summary>
    /// Controls the general game mechanics.
    /// </summary>
    public class GameBehavior : MonoBehaviour
    {
        public static GameBehavior instance;

        [Header("Player")]

        /// <summary>
        /// The player mode controller.
        /// </summary>
        [Tooltip("The player mode controller.")]
        public PlayerController player;

        [Header("UI")]

        /// <summary>
        /// The UI panel with the tiles.
        /// </summary>
        [Tooltip("The UI panel with the tiles.")]
        [SerializeField] private TilePanel tilePanel;

        /// <summary>
        /// The UI canvas with the button at the begin of a level.
        /// </summary>
        [Tooltip("The UI canvas with the button at the begin of a level.")]
        [SerializeField] private GameObject buttonUI;

        /// <summary>
        /// The button to start the level.
        /// </summary>
        [Tooltip("The button to start the level.")]
        [SerializeField] private Button buildLevelButton;

        /// <summary>
        /// The button to cancel the building mode.
        /// </summary>
        [Tooltip("The button to cancel the building mode.")]
        [SerializeField] private Button cancelBuildingButton;

        /// <summary>
        /// The success panel object.
        /// </summary>
        [Tooltip("The success panel object.")]
        public GameObject sucessPanel;

        /// <summary>
        /// The game over screen object.
        /// </summary>
        [Tooltip("The game over screen object.")]
        public GameObject gameOverPanel;

        [Header("Background")]
        public BackgroundController background;

        [Header("Builder Settings")]

        /// <summary>
        /// The settings file for level building.
        /// </summary>
        [Tooltip("The settings file for level building.")]
        public LevelBuilderSettings builderSettings;

        public List<TileGroupData> tileInventory;

        [HideInInspector] public string levelName;
        private TileBuilder tileBuilder;
        [HideInInspector] public LevelBuilder levelBuilder;

        private CameraController camController;
        private bool canStart;

        private bool buildingMode = true;

        [HideInInspector] public GameObject gridBackground;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            levelName = SceneManager.GetActiveScene().name;

            player.playerActive = false;

            tileBuilder = GetComponent<TileBuilder>();
            levelBuilder = GetComponent<LevelBuilder>();
            camController = Camera.main.GetComponent<CameraController>();

            levelBuilder.BuildLevel();
            tilePanel.InitTileButtons(tileInventory);

            cancelBuildingButton.interactable = false;
            buildLevelButton.interactable = false;

            camController.SetBuildingCamera(levelBuilder.gridData);

            canStart = false;
        }

        /// <summary>
        /// Begins the actual level after the placement of the mode shapes.
        /// </summary>
        public void StartPlayMode()
        {
            Debug.Log("GameBehavior: -> StartPlayMode()");
            if (canStart)
            {
                background.gameObject.SetActive(true);
                gridBackground.SetActive(false);

                foreach (LevelTile tile in levelBuilder.placedTiles)
                {
                    TileInventoryChange(new InventoryTile(tile.playerMode, tile.tileType), -1);
                }

                tileBuilder.BuildLevel(levelBuilder.placedTiles, levelBuilder.anchorTiles, levelBuilder.levelData);

                //TimerUI.timer = true;
                player.gameObject.SetActive(true);
                buttonUI.SetActive(false);
                tilePanel.gameObject.SetActive(false);
                camController.CameraTransition(Physics2D.OverlapPoint(player.activePlayer.transform.position, builderSettings.gridLayer).GetComponent<BackgroundMode>().transform.position);
                camController.SetLevelCamera();
                buildingMode = false;
                player.playerActive = true;

                tilePanel.ResetUsedGrids(levelBuilder.placedTiles);
            }
            Debug.Log("GameBehavior: <- StartPlayMode()");
        }

        /// <summary>
        /// Changes between the building and jump 'n' run mode.
        /// </summary>
        public void BuildingMode()
        {
            Debug.Log("GameBehavior: -> ChangeGameMode()");
            if (!buildingMode)
            {
                background.gameObject.SetActive(false);
                gridBackground.SetActive(true);

                buttonUI.SetActive(true);
                cancelBuildingButton.interactable = true;
                buildLevelButton.interactable = false;
                camController.SetBuildingCamera(levelBuilder.gridData);
                tilePanel.InitTileButtons(tileInventory);
                tilePanel.gameObject.SetActive(true);
                buildingMode = true;

            }
            Debug.Log("GameBehavior: <- ChangeGameMode()");

        }

        public void CanncelBuildingMode()
        {
            if(buildingMode)
            {
                buttonUI.SetActive(false);
                camController.CameraTransition(Physics2D.OverlapPoint(player.activePlayer.transform.position, builderSettings.gridLayer).GetComponent<BackgroundMode>().transform.position);
                camController.SetLevelCamera();
                tilePanel.gameObject.SetActive(false);
                buildingMode = false;
                player.playerActive = true;

                tilePanel.ResetUsedGrids(null);
            }
        }

        /// <summary>
        /// Checks if the tiles are placed correctly.
        /// </summary>
        public void CheckGridCollider()
        {
            if (levelBuilder.CheckLevelPath(levelBuilder.WorldPositionToCoordinate(player.activePlayer.transform.position)))
            {
                buildLevelButton.interactable = true;
                canStart = true;
            }
            else
            {
                canStart = false;
                buildLevelButton.interactable = false;
            }
        }

        public void InitLevelTiles(List<PlannedTile> inventoryTiles)
        {
            foreach (PlannedTile newTile in inventoryTiles)
            {
                bool alreadyAdded = false;
                for(int i = 0; i < tileInventory.Count; i++)
                {
                    if (tileInventory[i].tileType == newTile.tileType && tileInventory[i].playerMode == newTile.playerMode)
                    {
                        tileInventory[i].tileCount++;
                        alreadyAdded = true;
                        break;
                    }
                }

                if(alreadyAdded)
                {
                    continue;
                }

                TileGroupData tileGroup = new()
                {
                    tileType = newTile.tileType,
                    playerMode = newTile.playerMode,
                    tileCount = 1,
                    tileSprite = builderSettings.GetTileTypeSprite(newTile.tileType),
                };

                tileGroup.tileColor = builderSettings.GetModeColor(tileGroup.playerMode);

                tileInventory.Add(tileGroup);
            }
        }

        public void TileInventoryChange(InventoryTile tile, int count)
        {
            foreach(TileGroupData tileGroup in tileInventory)
            {
                if(tileGroup.tileType == tile.tileType && tileGroup.playerMode == tile.playerMode)
                {
                    tileGroup.tileCount += count;
                    return;
                }
            }

            if(count > 0)
            {
                TileGroupData newTileGroup = new TileGroupData()
                {
                    playerMode = tile.playerMode,
                    tileType = tile.tileType,
                    tileCount = count,
                    tileColor = builderSettings.GetModeColor(tile.playerMode),
                    tileSprite = builderSettings.GetTileTypeSprite(tile.tileType)
                };
                tileInventory.Add(newTileGroup);
            }
        }

        public PlayerController.PlayerModes CheckTileInventoryModes(TileCreator.TileTypes tileType)
        {
            foreach (TileGroupData tileGroup in tileInventory)
            {
                if (tileGroup.tileType == tileType)
                {
                    return tileGroup.playerMode;
                }
            }

            TileGroupData newTileGroup = new()
            {
                tileType = tileType,
                playerMode = tileBuilder.GetRandomPlayerMode(tileType),
                tileCount = 0,
                tileSprite = builderSettings.GetTileTypeSprite(tileType),
            };
            newTileGroup.tileColor = builderSettings.GetModeColor(newTileGroup.playerMode);
            tileInventory.Add(newTileGroup);

            return newTileGroup.playerMode;



        }
    }

    public class InventoryTile
    {
        public int count;
        public PlayerController.PlayerModes playerMode;
        public TileCreator.TileTypes tileType;

        public InventoryTile(PlayerController.PlayerModes playerMode, TileCreator.TileTypes tileType)
        {
            this.playerMode = playerMode;
            this.tileType = tileType;
        }
    }
}
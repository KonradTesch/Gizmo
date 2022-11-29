using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

using Rectangle.Player;
using Rectangle.UI;
using Rectangle.Level;
using Rectangle.TileCreation;

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

        [Header("Builder Settings")]

        /// <summary>
        /// The settings file for level building.
        /// </summary>
        [Tooltip("The settings file for level building.")]
        public LevelBuilderSettings builderSettings;

        public List<TileGroupData> tileInventory;

        private TileBuilder tileBuilder;
        [HideInInspector] public LevelBuilder levelBuilder;

        private CameraController camController;
        private bool canStart;

        private bool buildingMode = true;

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

            player.playerActive = false;

            tileBuilder = GetComponent<TileBuilder>();
            levelBuilder = GetComponent<LevelBuilder>();
            camController = Camera.main.GetComponent<CameraController>();

            levelBuilder.BuildLevel();
            tilePanel.InitTileButtons(tileInventory, levelBuilder.gridData.width * builderSettings.tileSize.x);

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
                foreach (LevelTile tile in levelBuilder.placedTiles)
                {
                    TileInventoryChange(tile.tileType, -1);
                }

                tileBuilder.BuildLevel(levelBuilder.placedTiles, levelBuilder.anchorTiles);

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
                buttonUI.SetActive(true);
                cancelBuildingButton.interactable = true;
                buildLevelButton.interactable = false;
                camController.SetBuildingCamera(levelBuilder.gridData);
                tilePanel.InitTileButtons(tileInventory, levelBuilder.gridData.width * builderSettings.tileSize.x);
                tilePanel.gameObject.SetActive(true);
                buildingMode = true;

            }
            Debug.Log("GameBehavior: <- ChangeGameMode()");

        }

        public void CanncelGameMode()
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

        public void InitLevelTiles(List<TileCreator.TileTypes> tileTypes)
        {
            Dictionary<TileCreator.TileTypes, int> levelTiles = new();

            foreach (TileCreator.TileTypes type in tileTypes)
            {
                if (levelTiles.ContainsKey(type))
                {
                    levelTiles[type]++;
                }
                else
                {
                    levelTiles.Add(type, 1);
                }
            }

            foreach (KeyValuePair<TileCreator.TileTypes, int> tileType in levelTiles)
            {
                bool alreadyAdded = false;
                for(int i = 0; i < tileInventory.Count; i++)
                {
                    if (tileInventory[i].tileType == tileType.Key)
                    {
                        tileInventory[i].tileCount = tileType.Value;
                        alreadyAdded = true;
                    }
                }

                if(alreadyAdded)
                {
                    continue;
                }

                TileGroupData tileGroup = new()
                {
                    tileType = tileType.Key,
                    playerMode = tileBuilder.GetRandomPlayerMode(tileType.Key),
                    tileCount = tileType.Value,
                    tileSprite = builderSettings.GetTileTypeSprite(tileType.Key),
                };

                tileGroup.tileColor = builderSettings.GetModeColor(tileGroup.playerMode);

                tileInventory.Add(tileGroup);
            }
        }

        public void TileInventoryChange(TileCreator.TileTypes tileType, int count)
        {
            foreach(TileGroupData tileGroup in tileInventory)
            {
                if(tileGroup.tileType == tileType)
                {
                    tileGroup.tileCount += count;
                    return;
                }
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
}
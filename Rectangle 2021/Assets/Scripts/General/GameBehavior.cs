using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

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

        public AnchorTilePanel anchorTilePanel;

        /// <summary>
        /// The UI canvas with the button at the begin of a level.
        /// </summary>
        [Tooltip("The UI canvas with the button at the begin of a level.")]
        public GameObject infoPanel;

        /// <summary>
        /// The button to start the level.
        /// </summary>
        [Tooltip("The button to start the level.")]
        [SerializeField] private TextMeshProUGUI buildLevelText;

        [SerializeField] private GameObject buildingUI;

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

        [Header("Sounds")]
        public AudioSource uiAudioSource;
        [SerializeField] private AudioClip buildLevelSound;
        [SerializeField] private AudioClip resetTileSound;
        public AudioClip deathSound;
        public AudioClip winSound;
        public AudioClip nutCatchSound;

        [HideInInspector] public List<TileGroupData> tileInventory;
        [HideInInspector] public List<LevelTile> placedTiles = new();
        [HideInInspector] public int usedTilesNumber;
        [HideInInspector] public List<Level.AnchorTile> anchorTiles = new();

        private TileBuilder tileBuilder;
        [HideInInspector] public LevelBuilder levelBuilder;

        private CameraController camController;
        private bool canStart;

        private bool buildingMode = true;

        [HideInInspector] public GameObject gridBackground;

        public delegate void MyDelegate();
        public static MyDelegate death;
        public static MyDelegate win;
        public static MyDelegate star;
        public static MyDelegate badTime;


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

            if (SaveGameManager.instance != null && SaveGameManager.instance.activeLevel != null)
            {
                levelBuilder.levelData = SaveGameManager.instance.activeLevel.levelData;
            }

            levelBuilder.BuildLevel();
            tilePanel.InitTileButtons(tileInventory);

            buildLevelText.gameObject.SetActive(false);

            camController.SetBuildingCamera(levelBuilder.gridData);

            canStart = false;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && buildingMode)
            {
                uiAudioSource.PlayOneShot(buildLevelSound);
                StartPlayMode();
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                uiAudioSource.PlayOneShot(resetTileSound);

                foreach(LevelTile tile in placedTiles)
                {
                    tile.Return();
                }

                placedTiles.Clear();
                CheckGridCollider();
            }
        }

        /// <summary>
        /// Begins the actual level after the placement of the mode shapes.
        /// </summary>
        public void StartPlayMode()
        {
            Debug.Log("GameBehavior: -> StartPlayMode()");
            if (canStart)
            {
                buildingUI.SetActive(false);
                background.gameObject.SetActive(true);
                gridBackground.SetActive(false);

                usedTilesNumber += levelBuilder.pathTiles.Count;

                foreach (LevelTile tile in levelBuilder.pathTiles)
                {
                    TileInventoryChange(new InventoryTile(tile.playerMode, tile.tileType), -1);
                }

                foreach(Level.AnchorTile anchor in anchorTiles)
                {
                    anchor.gameObject.SetActive(false);
                }

                tileBuilder.BuildLevel(levelBuilder.pathTiles, levelBuilder.anchorTiles, levelBuilder.levelData);

                //TimerUI.timer = true;
                player.gameObject.SetActive(true);
                infoPanel.SetActive(false);
                tilePanel.gameObject.SetActive(false);
                camController.CameraTransition(Physics2D.OverlapPoint(player.activePlayer.transform.position, builderSettings.gridLayer).GetComponent<BackgroundMode>().transform.position);
                camController.SetLevelCamera();
                buildingMode = false;
                player.playerActive = true;

                tilePanel.ResetUsedGrids(levelBuilder.pathTiles);
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
                buildingUI.SetActive(true);
                background.gameObject.SetActive(false);
                anchorTilePanel.gameObject.SetActive(false);
                gridBackground.SetActive(true);

                infoPanel.SetActive(true);
                camController.SetBuildingCamera(levelBuilder.gridData);
                tilePanel.InitTileButtons(tileInventory);
                tilePanel.gameObject.SetActive(true);
                buildingMode = true;

                float shortestDistance = Vector2.Distance(player.activePlayer.transform.position, anchorTiles[0].transform.position);
                Level.AnchorTile activeAnchor = null;
                foreach(Level.AnchorTile anchor in anchorTiles)
                {
                    if(Vector3.Distance(player.activePlayer.transform.position, anchor.transform.position) <= shortestDistance)
                    {
                        shortestDistance = Vector3.Distance(player.activePlayer.transform.position, anchor.transform.position);
                        activeAnchor = anchor;
                    }

                    if(!anchor.used)
                    {
                        anchor.gameObject.SetActive(true);
                        anchor.SetDefaultSprite();
                    }
                }
                activeAnchor.SetHighlightSprite();

            }
            Debug.Log("GameBehavior: <- ChangeGameMode()");

        }


        /// <summary>
        /// Checks if the tiles are placed correctly.
        /// </summary>
        public void CheckGridCollider()
        {
            if (levelBuilder.CheckLevelPath(levelBuilder.WorldPositionToCoordinate(player.activePlayer.transform.position)))
            {
                buildLevelText.gameObject.SetActive(true);
                canStart = true;
            }
            else
            {
                buildLevelText.gameObject.SetActive(false);
                canStart = false;
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
                    tileSprite = builderSettings.GetTileTypeSprite(newTile.tileType, newTile.playerMode),
                };


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
                    tileSprite = builderSettings.GetTileTypeSprite(tile.tileType, tile.playerMode)
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
                tileSprite = builderSettings.GetTileTypeSprite(tileType, tileBuilder.GetRandomPlayerMode(tileType)),
            };
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.LevelCreation;
using Rectangle.General;
using Rectangle.Player;
using Rectangle.UI;

namespace Rectangle.Level
{
    /// <summary>
    /// Th UI level tile scripts that controlls the behavior during building mode.
    /// </summary>
    public class LevelTile : MonoBehaviour
    {

        /// <summary>
        /// The type of the level tile. 
        /// </summary>
        [Tooltip("The type of the level tile. ")]
        public TileCreator.TileTypes tileType;

        /// <summary>
        /// The player mode of the level tile.
        /// </summary>
        [Tooltip("")]
        public PlayerController.PlayerModes playerMode;

        /// <summary>
        /// The builder settings.
        /// </summary>
        [Tooltip("The builder settings.")]
        [SerializeField] private LevelBuilderSettings builderSettings;

        /// <summary>
        /// The image when a collectable item is placed in the level tile.
        /// </summary>
        [Tooltip("The image when a collectable item is placed in the level tile.")]
        [SerializeField] private Sprite nutInCornerSprite;

        [Header("Sounds")]

        /// <summary>
        /// The audio clip when the tile is pressed.
        /// </summary>
        [Tooltip("The audio clip when the tile is pressed.")]
        [SerializeField] private AudioClip pressSound;

        /// <summary>
        /// The audio clip when the tile is placed.
        /// </summary>
        [Tooltip("The audio clip when the tile is placed.")]
        [SerializeField] private AudioClip placeSound;

        /// <summary>
        /// The audio clip when the tile returns.
        /// </summary>
        [Tooltip("The audio clip when the tile returns.")]
        [SerializeField] private AudioClip returnSound;

        /// <summary>
        /// The audio clip when the mouse hovers over the tile.
        /// </summary>
        [Tooltip("The audio clip when the mouse hovers over the tile.")]
        [SerializeField] private AudioClip hoverSound;


        [Header("Tile Sprites")]

        /// <summary>
        /// A list of sprites for the level tile.
        /// </summary>
        [Tooltip("The audio clip when the mouse hovers over the tile.")]
        public List<TileSprites> tileSprites;

        private AudioSource audioSource;
        private SpriteRenderer rend;
        private LevelBuilder levelBuilder;

        [HideInInspector] public TileButton button;
        [HideInInspector] public List<TileCreator.TileTypes> collectableTiles;

        private LayerMask gridLayer;

        [HideInInspector]public GridField gridCollider;
        private GridField lastGrid;

        private GameObject collectableImage;

        private Sprite normal;
        private Sprite pressed;
        private Sprite hover;

        private void Start()
        {
            gridLayer = LayerMask.GetMask("Grid");
            rend = GetComponent<SpriteRenderer>();

            audioSource = GameBehavior.instance.uiAudioSource;

            levelBuilder = GameBehavior.instance.levelBuilder;

            InitSprites();
            if(tileType != TileCreator.TileTypes.Anchor)
            {
                transform.localScale = new Vector3(1 / transform.parent.lossyScale.x, 1 / transform.parent.lossyScale.y, 1);
            }
        }

        /// <summary>
        /// Sets the right sprites for thios tile from the list.
        /// </summary>
        private void InitSprites()
        {
            for(int i = 0; i < tileSprites.Count; i++)
            {
                if (tileSprites[i].playerMode == playerMode)
                {
                    for(int n = 0; n < tileSprites[i].typeSprites.Length; n++)
                    {
                        if (tileSprites[i].typeSprites[n].tileType == tileType)
                        {
                            normal = tileSprites[i].typeSprites[n].normal;
                            pressed = tileSprites[i].typeSprites[n].pressed;
                            hover = tileSprites[i].typeSprites[n].hover;
                        }
                    }

                }
            }
            rend.sprite = normal;
        }

        private void OnMouseDrag()
        {
            if(tileType != TileCreator.TileTypes.Anchor)
            {
                rend.sprite = pressed;
                transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
            }
        }

        private void OnMouseUp()
        {
            if (tileType != TileCreator.TileTypes.Anchor)
            {
                rend.sprite = normal;
                rend.sortingOrder = 1;

                Collider2D positionCollider;

                positionCollider = Physics2D.OverlapPoint(transform.position, gridLayer);

                if (positionCollider != null ? !positionCollider.GetComponent<GridField>().isUsed : false)
                {
                    //Place Tile on Grid
                    audioSource.PlayOneShot(placeSound);

                    gridCollider = positionCollider.GetComponent<GridField>();
                    gridCollider.GetComponent<BackgroundMode>().playerMode = playerMode;

                    GameBehavior.instance.placedTiles.Add(this);

                    transform.position = gridCollider.transform.position;

                    for (int i = 0; i < levelBuilder.levelData.gridData.grid.Count; i++)
                    {
                        if (levelBuilder.levelData.gridData.grid[i].coordinates == levelBuilder.WorldPositionToCoordinate(transform.position))
                        {
                            levelBuilder.levelData.gridData.grid[i].levelSpot.placedTile = this;
                        }
                    }

                    gridCollider.isUsed = true;

                    GameBehavior.instance.CheckGridCollider();

                    button.PlaceTile(gridCollider);

                    if (gridCollider.collectableItem)
                    {
                        //Places the nut image as a child on the tile.
                        collectableImage = new GameObject("Nut");

                        collectableImage.transform.SetParent(transform);
                        collectableImage.transform.localPosition = Vector3.zero;
                        collectableImage.transform.localScale = Vector3.one;
                        collectableImage.AddComponent<SpriteRenderer>().sprite = nutInCornerSprite;
                    }

                }
                else
                {
                    if (lastGrid != null)
                    {
                        //returns tile to his last position on the grid
                        gridCollider = lastGrid;
                    }
                    else
                    {
                        //Retturns the tile to the inventory
                        GameBehavior.instance.placedTiles.Remove(this);
                        Return();
                        return;
                    }
                }

            }

        }

        void OnMouseDown()
        {
            if(tileType != TileCreator.TileTypes.Anchor)
            {
                rend.sprite = pressed;
                rend.sortingOrder = 2;

                audioSource.PlayOneShot(pressSound);

                if(collectableImage != null)
                {
                    Destroy(collectableImage);
                    collectableImage = null;
                }

                if (transform.localPosition == Vector3.zero)
                {
                    //Take Tile from Button
                    button.GetTile();
                }

                if (gridCollider != null)
                {
                    //Take Tile from Grid
                    button.ResetTile(gridCollider);

                    gridCollider.isUsed = false;
                    gridCollider.GetComponent<BackgroundMode>().playerMode = PlayerController.PlayerModes.None;

                    GameBehavior.instance.placedTiles.Remove(this);

                    for (int i = 0; i < levelBuilder.levelData.gridData.grid.Count; i++)
                    {
                        if (levelBuilder.levelData.gridData.grid[i].coordinates == levelBuilder.WorldPositionToCoordinate(gridCollider.transform.position))
                        {
                            levelBuilder.levelData.gridData.grid[i].levelSpot.placedTile = null;
                        }
                    }

                    lastGrid = gridCollider;

                    gridCollider = null;
                }

                transform.localScale = new Vector3(2 / transform.parent.lossyScale.x, 2 / transform.parent.lossyScale.y, 1);
            }
        }

        private void OnMouseOver()
        {
            if(tileType != TileCreator.TileTypes.Anchor)
            {
                if (!Input.GetMouseButton(0))
                {
                    rend.sprite = hover;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (collectableImage != null)
                    {
                        Destroy(collectableImage);
                        collectableImage = null;
                    }

                    if (gridCollider != null)
                    {
                        //Remove tile from Grid and return it to the inventory
                         GameBehavior.instance.placedTiles.Remove(this);
                        Return();

                        lastGrid = null;

                        button.ResetTile(gridCollider);


                        for (int i = 0; i < levelBuilder.levelData.gridData.grid.Count; i++)
                        {
                            if (levelBuilder.levelData.gridData.grid[i].coordinates == levelBuilder.WorldPositionToCoordinate(gridCollider.transform.position))
                            {
                                levelBuilder.levelData.gridData.grid[i].levelSpot.placedTile = null;
                            }
                        }

                        gridCollider.GetComponent<BackgroundMode>().playerMode = PlayerController.PlayerModes.None;
                        gridCollider.isUsed = false;
                        gridCollider = null;

                    }

                    GameBehavior.instance.CheckGridCollider();
                }
            }
        }

        private void OnMouseExit()
        {
            if(tileType != TileCreator.TileTypes.Anchor)
            {
                rend.sprite = normal;
            }
        }

        /// <summary>
        /// Returns the tile to the inventory
        /// </summary>
        public void Return()
        {
            audioSource.PlayOneShot(returnSound);

            rend.sortingOrder = 1;

            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(1 / transform.parent.lossyScale.x, 1 / transform.parent.lossyScale.y, 1);
            button.ReturnTile();

        }
    }

    [System.Serializable]
    public class TileSprites
    {
        /// <summary>
        /// The player mode for which the sprites are meant.
        /// </summary>
        public PlayerController.PlayerModes playerMode;
        /// <summary>
        /// The collection of sprites
        /// </summary>
        public TileTypeSprites[] typeSprites;
    }

    [System.Serializable]
    public class TileTypeSprites
    {
        /// <summary>
        /// The tile type for which the sprites are meant.
        /// </summary>
        public TileCreator.TileTypes tileType;
        [Header("Sprites")]
        /// <summary>
        /// The sprite for the normal state.
        /// </summary>
        public Sprite normal;
        /// <summary>
        /// The sprite for the pressed state.
        /// </summary>
        public Sprite pressed;
        /// <summary>
        /// The sprite for the highlited state.
        /// </summary>
        public Sprite highlighted;
        /// <summary>
        /// The sprite for the hovered state.
        /// </summary>
        public Sprite hover;

    }
}

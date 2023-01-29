using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.LevelCreation;
using Rectangle.Player;
using Rectangle.UI;

namespace Rectangle.Level
{
    public class LevelTile : MonoBehaviour
    {
        public TileCreator.TileTypes tileType;
        public PlayerController.PlayerModes playerMode;
        [SerializeField] private LevelBuilderSettings builderSettings;

        [Header("Tile Sprites")]

        public List<TileSprites> tileSprites;

        [HideInInspector] public TileButton button;

        [HideInInspector] public List<TileCreator.TileTypes> collectableTiles;

        private SpriteRenderer rend;

        private LayerMask gridLayer;

        [HideInInspector]public GridField gridCollider;
        private GridField lastGrid;

        private LevelBuilder levelBuilder;

        private Sprite normal;
        private Sprite pressed;
        private Sprite highlighted;
        private Sprite hover;

        private void Start()
        {
            gridLayer = LayerMask.GetMask("Grid");
            rend = GetComponent<SpriteRenderer>();

            levelBuilder = General.GameBehavior.instance.levelBuilder;

            InitSprites();
        }

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
                            highlighted = tileSprites[i].typeSprites[n].highlighted;
                            hover = tileSprites[i].typeSprites[n].hover;
                        }
                    }

                }
            }

            rend.sprite = normal;
        }

        private void OnMouseDrag()
        {
            rend.sprite = pressed;
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y , 0);
        }

        private void OnMouseUp()
        {
            rend.sprite = normal;
            rend.sortingOrder = 1;

            Collider2D positionCollider;

            positionCollider = Physics2D.OverlapPoint(transform.position, gridLayer);

            if (positionCollider != null ? !positionCollider.GetComponent<GridField>().isUsed : false)
            {
                gridCollider = positionCollider.GetComponent<GridField>();
                gridCollider.backgroundRend.color = rend.color;
                gridCollider.GetComponent<BackgroundMode>().playerMode = playerMode;

                button.PlaceTile(gridCollider);
            }
            else
            {
                if (lastGrid != null)
                {
                    gridCollider = lastGrid;
                }
                else
                {
                    Return();
                    return;
                }
            }

            transform.position = gridCollider.transform.position;

            for (int i = 0; i < levelBuilder.levelData.gridData.grid.Count; i++)
            {
                if (levelBuilder.levelData.gridData.grid[i].coordinates == levelBuilder.WorldPositionToCoordinate(transform.position))
                {
                    levelBuilder.levelData.gridData.grid[i].levelSpot.placedTile = this;
                }
            }

            gridCollider.isUsed = true;

            General.GameBehavior.instance.CheckGridCollider();
        }

        void OnMouseDown()
        {
            rend.sprite = pressed;
            rend.sortingOrder = 2;

            if (transform.localPosition == Vector3.zero)
            {
                button.GetTile();
            }

            if(gridCollider != null)
            {
                button.ResetTile(gridCollider);

                gridCollider.isUsed = false;
                gridCollider.backgroundRend.color = Color.gray;
                gridCollider.GetComponent<BackgroundMode>().playerMode = PlayerController.PlayerModes.None;

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

            transform.localScale = Vector3.one * 2;
        }

        private void OnMouseOver()
        {
            if (!Input.GetMouseButton(0))
            {
                rend.sprite = hover;
            }

            if (Input.GetMouseButtonDown(1))
            {
                if(gridCollider != null)
                {
                    Return();

                    lastGrid = null;

                    button.ResetTile(gridCollider);


                    for(int i = 0; i < levelBuilder.levelData.gridData.grid.Count; i++)
                    {
                        if (levelBuilder.levelData.gridData.grid[i].coordinates == levelBuilder.WorldPositionToCoordinate(gridCollider.transform.position))
                        {
                            levelBuilder.levelData.gridData.grid[i].levelSpot.placedTile = this;
                        }
                    }

                    gridCollider.backgroundRend.color = Color.gray;
                    gridCollider.GetComponent<BackgroundMode>().playerMode = PlayerController.PlayerModes.None;
                    gridCollider.isUsed = false;
                    gridCollider = null;

                }

                General.GameBehavior.instance.CheckGridCollider();
            }
        }

        private void OnMouseExit()
        {
            rend.sprite = normal;
        }

        private void Return()
        {
            rend.sortingOrder = 1;

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector2.one;
            button.ReturnTile();

        }
    }

    [System.Serializable]
    public class TileSprites
    {
        public PlayerController.PlayerModes playerMode;
        public TileTypeSprites[] typeSprites;

    }

    [System.Serializable]
    public class TileTypeSprites
    {
        public TileCreator.TileTypes tileType;
        [Header("Sprites")]
        public Sprite normal;
        public Sprite pressed;
        public Sprite highlighted;
        public Sprite hover;

    }
}

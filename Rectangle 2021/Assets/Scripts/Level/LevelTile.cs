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
        public Vector2Int tileSize = new Vector2Int(32, 18);
        [SerializeField] private LevelBuilderSettings builderSettings;

        [HideInInspector] public TileButton button;

        public List<TileCreator.TileTypes> collectableTiles;

        private SpriteRenderer rend;

        private LayerMask gridLayer;

        [HideInInspector]public GridField gridCollider;
        private GridField lastGrid;

        private LevelBuilder levelBuilder;

        private void Start()
        {
            gridLayer = LayerMask.GetMask("Grid");
            rend = GetComponent<SpriteRenderer>();

            levelBuilder = General.GameBehavior.instance.levelBuilder;
        }

        private void OnMouseDrag()
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y , 0);
        }

        private void OnMouseUp()
        {
            rend.sortingOrder = 1;

            Collider2D positionCollider;

            positionCollider = Physics2D.OverlapPoint(transform.position, gridLayer);

            if (positionCollider != null ? !positionCollider.GetComponent<GridField>().isUsed : false)
            {
                gridCollider = positionCollider.GetComponent<GridField>();
                gridCollider.GetComponent<SpriteRenderer>().color = rend.color;
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
            levelBuilder.gridData.grid[levelBuilder.WorldPositionToCoordinate(transform.position)].placedTile = this;
            gridCollider.isUsed = true;

            General.GameBehavior.instance.CheckGridCollider();
        }

        void OnMouseDown()
        {
            rend.sortingOrder = 2;

            if (transform.localPosition == Vector3.zero)
            {
                button.GetTile();
            }

            if(gridCollider != null)
            {
                button.ResetTile(gridCollider);

                gridCollider.isUsed = false;
                gridCollider.GetComponent<SpriteRenderer>().color = Color.gray;
                gridCollider.GetComponent<BackgroundMode>().playerMode = PlayerController.PlayerModes.None;

                levelBuilder.gridData.grid[levelBuilder.WorldPositionToCoordinate(gridCollider.transform.position)].placedTile = null;

                lastGrid = gridCollider;

                gridCollider = null;
            }

            transform.localScale = Vector3.one * 2;
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if(gridCollider != null)
                {
                    Return();

                    lastGrid = null;

                    button.ResetTile(gridCollider);

                    levelBuilder.gridData.grid[levelBuilder.WorldPositionToCoordinate(gridCollider.transform.position)].placedTile = null;

                    gridCollider.GetComponent<SpriteRenderer>().color = Color.gray;
                    gridCollider.GetComponent<BackgroundMode>().playerMode = PlayerController.PlayerModes.None;
                    gridCollider.isUsed = false;
                    gridCollider = null;

                }

                General.GameBehavior.instance.CheckGridCollider();
            }
        }

        private void Return()
        {
            rend.sortingOrder = 1;

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector2.one;
            button.ReturnTile();

        }
    }
}

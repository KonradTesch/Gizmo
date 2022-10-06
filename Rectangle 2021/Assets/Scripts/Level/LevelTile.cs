using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.TileCreation;
using Rectangle.UI;

namespace Rectangle.Level
{
    public class LevelTile : MonoBehaviour
    {
        public TileCreator.TileTypes tileType;
        public Vector2Int tileSize = new Vector2Int(32, 32);
        [SerializeField] private LevelBuilderSettings builderSettings;

        [HideInInspector] public TileButton button;

        private LayerMask gridLayer;

        private SpriteRenderer rend;

        private IsGridUsed gridCollider;
        private IsGridUsed lastGrid;

        private void Start()
        {
            gridLayer = LayerMask.GetMask("Grid");
            rend = GetComponent<SpriteRenderer>();
        }

        private void OnMouseDrag()
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y , 0);
        }

        private void OnMouseUp()
        {
            Collider2D positionCollider;

            positionCollider = Physics2D.OverlapPoint(transform.position, gridLayer);

            if (positionCollider != null ? !positionCollider.GetComponent<IsGridUsed>().isUsed : false)
            {
                gridCollider = positionCollider.GetComponent<IsGridUsed>();

                rend.color = builderSettings.GetModeColor(positionCollider.GetComponent<BackgroundMode>().playerMode);
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
            gridCollider.isUsed = true;

            General.GameBehavior.instance.CheckGridCollider();
        }

        void OnMouseDown()
        {
            if(transform.localPosition == Vector3.zero)
            {
                button.GetTile();
            }

            if(gridCollider != null)
            {
                gridCollider.isUsed = false;

                lastGrid = gridCollider;

                gridCollider = null;
            }

            transform.localScale = Vector3.one * 2;
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Return();

                if(gridCollider != null)
                    gridCollider.isUsed = false;

                General.GameBehavior.instance.CheckGridCollider();
            }
        }

        private void Return()
        {
            rend.color = Color.white;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector2.one;
            button.ReturnTile();
        }
    }
}

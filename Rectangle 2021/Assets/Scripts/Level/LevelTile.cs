using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.TileCreater;
using Rectangle.Player;

namespace Rectangle.Level
{
    public class LevelTile : MonoBehaviour
    {
        public TileCreater.TileCreater.TileTypes tileType;
        public ModeController.PlayerModes playerMode;
        public Vector2Int tileSize = new Vector2Int(32, 32);

        private LayerMask gridLayer;

        private bool isPicked;

        private Vector2 mouseOffset = Vector2.zero;
        private Vector3 latestPosition;

        private TileGroup tileGroup;

        private void Start()
        {
            tileGroup = GetComponentInParent<TileGroup>();

            gridLayer = LayerMask.GetMask("Grid");
        }
        void Update()
        {
            if (Input.GetMouseButtonUp(0) && isPicked)
            {
                isPicked = false;

                Vector3 newPos = NewShapePosition();

                transform.parent.position = newPos;

                if (newPos != latestPosition)
                {
                    foreach (IsGridUsed grid in tileGroup.gridColliders)
                    {
                        grid.isUsed = true;
                    }
                    General.GameBehavior.instance.CheckGridCollider();
                }
                else
                {
                    foreach (IsGridUsed grid in tileGroup.latestGrid)
                    {
                        grid.isUsed = true;
                    }
                    tileGroup.gridColliders = new List<IsGridUsed>(tileGroup.latestGrid);

                    if (tileGroup.latestGrid.Count == 0)
                        transform.parent.transform.localScale = new Vector3(0.4f, 0.4f, 1);

                }
            }

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isPicked == true)
            {
                transform.parent.position = mousePos + mouseOffset - (Vector2)transform.localPosition;
            }
            mouseOffset = (Vector2)transform.position - mousePos;

        }

        void OnMouseDown()
        {
            if (transform.parent.transform.localScale != Vector3.one)
            {
                transform.parent.transform.localScale = Vector3.one;
            }
            latestPosition = transform.position - transform.localPosition;
            isPicked = true;

            foreach (IsGridUsed grid in tileGroup.gridColliders)
            {
                grid.isUsed = false;
            }
            tileGroup.latestGrid = new List<IsGridUsed>(tileGroup.gridColliders);
            tileGroup.gridColliders.Clear();
        }

        /// <summary>
        /// Returnst the new position whan this mode shape is dropped.
        /// </summary>
        /// <returns></returns>
        public Vector3 NewShapePosition()
        {
            Collider2D positionCollider;

            foreach (Transform child in transform.parent.transform)
            {
                positionCollider = Physics2D.OverlapPoint(child.transform.position, gridLayer);

                if (positionCollider == null)
                    return latestPosition;

                IsGridUsed grid = positionCollider.gameObject.GetComponent<IsGridUsed>();
                tileGroup.gridColliders.Add(grid);
                if (grid.isUsed)
                {
                    return latestPosition;
                }

            }

            positionCollider = Physics2D.OverlapPoint(transform.position, gridLayer);

            return positionCollider.gameObject.transform.position - transform.localPosition;
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            {
                tileGroup.gameObject.transform.position = tileGroup.originPosition;
                tileGroup.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                foreach (IsGridUsed grid in tileGroup.gridColliders)
                {
                    grid.isUsed = false;
                }
                tileGroup.gridColliders.Clear();

                General.GameBehavior.instance.CheckGridCollider();
            }
        }
    }
}

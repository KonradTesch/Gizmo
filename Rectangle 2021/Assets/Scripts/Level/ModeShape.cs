using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    public class ModeShape : MonoBehaviour
    {

        private LayerMask gridLayer;

        private bool isPicked;

        private Vector2 mouseOffset;
        private Vector3 latestPosition;

        private ModeGroup modeGroup;

        private void Start()
        {
            modeGroup = GetComponentInParent<ModeGroup>();

            gridLayer = LayerMask.GetMask("GridCollider");
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
                    foreach (IsGridUsed grid in modeGroup.gridColliders)
                    {
                        grid.isUsed = true;
                    }
                }
                else
                {
                    foreach (IsGridUsed grid in modeGroup.latestGrid)
                    {
                        grid.isUsed = true;
                    }
                    modeGroup.gridColliders = new List<IsGridUsed>(modeGroup.latestGrid);

                    if (modeGroup.latestGrid.Count == 0)
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

            foreach (IsGridUsed grid in modeGroup.gridColliders)
            {
                grid.isUsed = false;
            }
            modeGroup.latestGrid = new List<IsGridUsed>(modeGroup.gridColliders);
            modeGroup.gridColliders.Clear();
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
                modeGroup.gridColliders.Add(grid);
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
                modeGroup.gameObject.transform.position = modeGroup.originPosition;
                modeGroup.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                foreach (IsGridUsed grid in modeGroup.gridColliders)
                {
                    grid.isUsed = false;
                }
                modeGroup.gridColliders.Clear();
            }
        }
    }
}

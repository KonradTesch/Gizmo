using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.General
{
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// The offset position of the camera.
        /// </summary>
        [Tooltip("The offset position of the camera.")]
        [SerializeField] private Vector3 camOffset;

        [SerializeField] private UI.BuildingScreens buildingScreens;

        public Vector3 buildingScreenOffset;
        public Vector2 resolutionOffset;

        [Header("Camera Transition")]

        /// <summary>
        /// The time in seconds of the transition betwenn two camera positions.
        /// </summary>
        [Tooltip("The time in seconds of the transition betwenn two camera positions.")]
        [SerializeField] private float transitionTime = 1;

        private float timer = 0;
        private bool transition;
        private Vector3 startPosition;
        private Vector3 newPosition;

        void Update()
        {
        if(transition)
            {
                transform.position = Vector3.Lerp(startPosition, newPosition, timer / transitionTime);

                timer += Time.deltaTime;

                if(timer > transitionTime)
                {
                    transform.position = newPosition;
                    transition = false;
                }
            }
        }

        /// <summary>
        /// Moves the camera to a new Position;
        /// </summary>
        public void CameraTransition(Vector3 newPosition)
        {
            if(transitionTime > 0)
            {
                transition = true;
                startPosition = transform.position;
                this.newPosition = newPosition + camOffset;
                timer = 0;
            }
            else
            {
                transform.position = newPosition + camOffset;
            }
        }

        /// <summary>
        /// Let the camera zoom in the level.
        /// </summary>
        public void SetLevelCamera()
        {
            float minWidth = GameBehavior.instance.builderSettings.tileSize.x / 2f / ((float)Screen.width / (float)Screen.height);
            float minHeight = GameBehavior.instance.builderSettings.tileSize.y / 2f;

            if(minWidth > minHeight)
            {
                GetComponent<Camera>().orthographicSize = minWidth;
            }
            else
            {
                GetComponent<Camera>().orthographicSize = minHeight;
            }
        }

        /// <summary>
        /// Let the camera zoom out to the building view.
        /// </summary>
        public void SetBuildingCamera(LevelCreation.LevelGrid gridData)
        {
            float minWidth = ((gridData.width / 2f) * GameBehavior.instance.builderSettings.tileSize.x) / ((float)Screen.width / (float)Screen.height);
            float minHeight = (gridData.height / 2f) * GameBehavior.instance.builderSettings.tileSize.y;

            Camera cam = GetComponent<Camera>();

            if(minWidth + resolutionOffset.x > minHeight + resolutionOffset.y)
            {
                cam.orthographicSize = minWidth + resolutionOffset.x;
            }
            else
            {
                cam.orthographicSize = minHeight + resolutionOffset.y;
            }

            transform.position = new Vector3(minWidth * ((float)Screen.width / (float)Screen.height), minHeight - 8f, camOffset.z) + buildingScreenOffset;

            buildingScreens.InitBuildingScreens(transform.position ,cam.orthographicSize, new Vector2(gridData.width, gridData.height));
        }
    }

}

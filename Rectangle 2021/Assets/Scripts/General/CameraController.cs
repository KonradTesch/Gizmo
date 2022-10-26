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

        /// <summary>
        /// The Camera Size (Zoom) during the building view.
        /// </summary>
        [Tooltip("The Camera Size (Zoom) during the building view.")]
        [SerializeField] private float buildingCamSize;

        /// <summary>
        /// The Camera Size (Zoom) during the jump 'n' run level.
        /// </summary>
        [Tooltip("The Camera Size (Zoom) during the jump 'n' run level.")]
        [SerializeField] private float levelCamSize;

        [Header("Camera Transition")]

        /// <summary>
        /// The time in seconds of the transition betwenn two camera positions.
        /// </summary>
        [Tooltip("The time in seconds of the transition betwenn two camera positions.")]
        [SerializeField] private float transitionTime = 1;

        private Vector3 LevelBuildingPosition;

        private float timer = 0;
        private bool transition;
        private Vector3 startPosition;
        private Vector3 newPosition;

        private void Start()
        {
            LevelBuildingPosition = transform.position;
        }

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
            GetComponent<Camera>().orthographicSize = levelCamSize;
        }

        /// <summary>
        /// Let the camera zoom out to the building view.
        /// </summary>
        public void SetBuildingCamera()
        {
            GetComponent<Camera>().orthographicSize = buildingCamSize;
            transform.position = LevelBuildingPosition;
        }
    }

}

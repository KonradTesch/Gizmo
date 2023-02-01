using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.UI
{
    public class BuildingScreens : MonoBehaviour
    {
        [SerializeField] private GameObject gridBackground;

        public void InitBuildingScreens(Vector3 camPosition, float orthographicSize, Vector2 levelSize)
        {
            transform.position = camPosition + new Vector3(0, 0, 10);
            transform.localScale = Vector3.one * orthographicSize / 53f;

            InitGridBackground(levelSize);
        }

        private void InitGridBackground(Vector2 levelSize)
        {
            gridBackground.transform.localScale = new Vector3(0.2f * levelSize.x, 0.2f * levelSize.y, 1);
        }

    }
}

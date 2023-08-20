using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.Level;

namespace Rectangle.UI
{
    /// <summary>
    /// The UI element that displays the level tile inside a anchor tile during building mode.
    /// </summary>
    public class AnchorTilePanel : MonoBehaviour
    {
        /// <summary>
        /// The example UI elementb for the displayed level tile.
        /// </summary>
        [Tooltip("The example UI elementb for the displayed level tile.")]
        [SerializeField] private TileButton tileReference;

        /// <summary>
        /// The UI panel.
        /// </summary>
        [Tooltip("The UI panel.")]
        [SerializeField] private GameObject panelUI;

        /// <summary>
        /// Displayes the levvel tiles that are stored inside an anchor tile.
        /// </summary>
        public void ShowAnchorTiles(List<TileGroupData> tileGroups)
        {
            gameObject.SetActive(true);

            SpriteRenderer rend = GetComponent<SpriteRenderer>();

            foreach (Transform child in transform)
            {
                if (child.gameObject != tileReference.gameObject && child.gameObject != panelUI)
                    Destroy(child.gameObject);
            }

            int i = 1;

            foreach (TileGroupData tileGroup in tileGroups)
            {
                if (tileGroup.tileCount > 0)
                {
                    TileButton newButton = Instantiate(tileReference, transform).GetComponent<TileButton>();

                    newButton.transform.localPosition = new Vector3(newButton.transform.localPosition.x, (rend.bounds.size.y / (tileGroups.Count + 1)) * i - (rend.bounds.size.y / 2),  0);

                    newButton.tileCount = tileGroup.tileCount;
                    newButton.tileType = tileGroup.tileType;
                    newButton.tileSprite = tileGroup.tileSprite;
                    newButton.playerMode = tileGroup.playerMode;

                    SpriteRenderer tileRend = newButton.GetComponent<SpriteRenderer>();

                    tileRend.sprite = tileGroup.tileSprite;
                    tileRend.color = Color.grey;

                    newButton.gameObject.SetActive(true);

                    i++;
                }
            }
        }
    }
}

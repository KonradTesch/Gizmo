using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.LevelCreation;
using Rectangle.General;

using Rectangle.Player;

namespace Rectangle.UI
{
    /// <summary>
    /// The dilsplay inside an anchor that shows the level tiles, that are stored inside the anchor.
    /// </summary>
    public class AnchorBox : MonoBehaviour
    {

        /// <summary>
        /// The list of UI elements for the tiles.
        /// </summary>
        [Tooltip("The list of UI elements for the tiles.")]
        public List<SpriteRenderer> anchorTileSprites;

        /// <summary>
        /// The animator for the open-box-animation.
        /// </summary>
        [Tooltip("The animator for the open-box-animation.")]
        public Animator animator;


        private List<PlannedTile> anchorTiles;
        private bool isUsed = false;
        private bool inAnchor = false;

        /// <summary>
        /// Cretaes the displyed anchors inside the anchor box.
        /// </summary>
        public void SetAnchorTiles(List<PlannedTile> anchorTiles)
        {
            this.anchorTiles = anchorTiles;

            for (int i = 0; i < anchorTileSprites.Count; i++)
            {
                if(i < anchorTiles.Count)
                {
                    Sprite tileSprite = GameBehavior.instance.builderSettings.GetTileTypeSprite(anchorTiles[i].tileType, anchorTiles[i].playerMode);

                    anchorTileSprites[i].sprite = tileSprite;
                }
                else
                {
                    anchorTileSprites[i].enabled = false;
                }
            }
        }

        private void Update()
        {
            if(inAnchor && !isUsed)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isUsed = true;
                    animator.SetTrigger("close");

                    GameBehavior.instance.player.playerActive = false;
                    GameBehavior.instance.player.currentCheckpoint = GameBehavior.instance.player.activePlayer.transform.position;

                    foreach (PlannedTile tile in anchorTiles)
                    {
                        GameBehavior.instance.TileInventoryChange(new InventoryTile(tile.playerMode, tile.tileType), 1);
                    }

                    foreach (SpriteRenderer sprite in anchorTileSprites)
                    {
                        sprite.gameObject.SetActive(false);
                    }

                    GameBehavior.instance.BuildingMode();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.GetComponent<PlayerBase>() != null)
            {
                if(!isUsed)
                {
                    animator.SetTrigger("open");
                }
                inAnchor = true;

                collision.transform.parent.GetComponent<PlayerController>().anchor = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerBase>() != null)
            {
                animator.SetTrigger("close");

                inAnchor = false;
                collision.transform.parent.GetComponent<PlayerController>().anchor = false;
            }
        }

    }
}

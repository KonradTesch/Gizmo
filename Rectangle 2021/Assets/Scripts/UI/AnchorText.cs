using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using Rectangle.Player;
using Rectangle.LevelCreation;
using Rectangle.Level;

namespace Rectangle.UI
{
    public class AnchorText : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshPro rectangleNumber;
        [SerializeField] private TextMeshPro bubbleNuimber;
        [SerializeField] private TextMeshPro spikeyNumber;

        [HideInInspector] public List<TileGroupData> anchorTiles;

        private bool showTiles;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("click");
            if(showTiles)
            {
                General.GameBehavior.instance.anchorTilePanel.ShowAnchorTiles(anchorTiles);
            }
            else
            {
                General.GameBehavior.instance.anchorTilePanel.gameObject.SetActive(false);
            }
        }

        public void SetTileNumbers(List<PlannedTile> collectableTiles)
        {
            int rectCount = 0;
            int bubbleCount = 0;
            int spikeyCount = 0;

            foreach(PlannedTile tile in collectableTiles)
            {
                switch(tile.playerMode)
                {
                    case PlayerController.PlayerModes.Rectangle:
                        rectCount++;
                        break;
                    case PlayerController.PlayerModes.Bubble:
                        bubbleCount++;
                        break;
                    case PlayerController.PlayerModes.Spikey:
                        spikeyCount++;
                        break;
                }
            }

            rectangleNumber.text = rectCount.ToString();
            bubbleNuimber.text = bubbleCount.ToString();
            spikeyNumber.text = spikeyCount.ToString();

        }

        private void AddToAnchorTiles(PlayerController.PlayerModes playerMode, TileCreator.TileTypes tileType)
        {
            for( int i = 0; i < anchorTiles.Count; i++)
            {
                if (anchorTiles[i].playerMode == playerMode && anchorTiles[i].playerMode == playerMode)
                {
                    anchorTiles[i].tileCount++;
                    return;
                }
            }

            TileGroupData newTileGroup = new TileGroupData()
            {
                playerMode = playerMode,
                tileType = tileType,
                tileCount = 1,
                tileSprite = General.GameBehavior.instance.builderSettings.GetTileTypeSprite(tileType, playerMode)
            };

            anchorTiles.Add(newTileGroup);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.GetComponent<PlayerBase>() != null)
            {
                collision.transform.parent.GetComponent<PlayerController>().anchor = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerBase>() != null)
            {
                collision.transform.parent.GetComponent<PlayerController>().anchor = false;
            }
        }
    }
}

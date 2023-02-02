using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rectangle.LevelCreation;

namespace Rectangle.Level
{


    public class AnchorTile : MonoBehaviour
    {
        [Header("Sprites")]

        [SerializeField] private Sprite defaultAnchor;
        [SerializeField] private Sprite hoverAnchor;
        [SerializeField] private Sprite highlightAnchor;

        private GameObject infoPanel;

        private List<TileGroupData> anchorTiles;

        private bool showTiles = true;

        private SpriteRenderer rend;

        private void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            rend.sprite = defaultAnchor;
            infoPanel = General.GameBehavior.instance.infoPanel;
        }

        private void OnMouseDown()
        {
            if (showTiles)
            {
                rend.sprite = highlightAnchor;
                General.GameBehavior.instance.anchorTilePanel.ShowAnchorTiles(anchorTiles);
                infoPanel.SetActive(false);
                showTiles = false;
            }
            else
            {
                rend.sprite = defaultAnchor;
                showTiles = true;
                General.GameBehavior.instance.anchorTilePanel.gameObject.SetActive(false);
                infoPanel.SetActive(true);
            }
        }

        private void OnMouseOver()
        {if(showTiles)
            {
                rend.sprite = hoverAnchor;
            }
        }

        private void OnMouseExit()
        {
            if(showTiles)
            {
                rend.sprite = defaultAnchor;
            }
        }
        public void InitAnchorTiles(List<PlannedTile> tiles)
        {
            anchorTiles = new();
            for(int i = 0; i < tiles.Count; i++)
            {
                AddToAnchorTiles(tiles[i].playerMode, tiles[i].tileType);
            }
        }

        private void AddToAnchorTiles(Player.PlayerController.PlayerModes playerMode, TileCreator.TileTypes tileType)
        {
            for (int i = 0; i < anchorTiles.Count; i++)
            {
                if (anchorTiles[i].playerMode == playerMode && anchorTiles[i].tileType == tileType)
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


    }
}

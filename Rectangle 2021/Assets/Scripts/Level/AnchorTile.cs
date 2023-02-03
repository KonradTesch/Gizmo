using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rectangle.LevelCreation;
using Rectangle.General;

namespace Rectangle.Level
{


    public class AnchorTile : MonoBehaviour
    {
        [Header("Sprites")]

        [SerializeField] private Sprite defaultAnchor;
        [SerializeField] private Sprite hoverAnchor;
        [SerializeField] private Sprite pressedAnchor;
        [SerializeField] private Sprite highlightAnchor;

        private GameObject infoPanel;

        private List<TileGroupData> anchorTiles;

        private bool showTiles = true;
        [HideInInspector] public bool used = false;

        private SpriteRenderer rend;

        private void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            rend.sprite = defaultAnchor;
            infoPanel = GameBehavior.instance.infoPanel;
            GameBehavior.instance.anchorTiles.Add(this);
        }

        private void OnMouseDown()
        {
            if(!used)
            {
                if (showTiles)
                {
                    rend.sprite = pressedAnchor;
                    GameBehavior.instance.anchorTilePanel.ShowAnchorTiles(anchorTiles);

                    for (int i = 0; i < GameBehavior.instance.anchorTiles.Count; i++)
                    {
                        if (GameBehavior.instance.anchorTiles[i] != this)
                        {
                            GameBehavior.instance.anchorTiles[i].SetDefaultSprite(); ;
                        }
                    }

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
        }

        public void SetDefaultSprite()
        {
            if(!used)
            {
                rend.sprite = defaultAnchor;
                showTiles = true;
            }
        }

        public void SetHighlightSprite()
        {
            rend.sprite = highlightAnchor;
            used = true;
        }

        private void OnMouseOver()
        {if(showTiles && !used)
            {
                rend.sprite = hoverAnchor;
            }
        }

        private void OnMouseExit()
        {
            if(showTiles && !used)
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

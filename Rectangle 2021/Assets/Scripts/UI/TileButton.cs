using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rectangle.LevelCreation;
using Rectangle.Level;
using Rectangle.Player;

namespace Rectangle.UI
{
    public class TileButton : MonoBehaviour
    {

        /// <summary>
        /// The text object that shows the amout of level tiles.
        /// </summary>
        [Tooltip("The text object that shows the amout of level tiles.")]
        [SerializeField] private TextMeshPro tileCountText;

        /// <summary>
        /// A prefab of an level tile.
        /// </summary>
        [Tooltip("A prefab of an level tile.")]
        [SerializeField] private GameObject tilePrefab;

        [HideInInspector] public Sprite tileSprite;
        [HideInInspector] public int tileCount;
        [HideInInspector] public TileCreator.TileTypes tileType;
        [HideInInspector] public PlayerController.PlayerModes playerMode;

        [HideInInspector]public List<GridField> usedGridFields = new();

        private void OnEnable()
        {
            tileCountText.text = tileCount.ToString() +" x";

            for(int i = 0; i < tileCount; i++)
            {
                LevelTile newTile = Instantiate(tilePrefab, transform).GetComponent<LevelTile>();
                newTile.transform.localPosition = Vector3.zero;
                newTile.tileType = tileType;
                newTile.playerMode = playerMode;
                newTile.button = this;

                SpriteRenderer tileRend = newTile.GetComponent<SpriteRenderer>();

                tileRend.sprite = tileSprite;
            }
        }

        /// <summary>
        /// Decreases the amount of left tiles, when the player takes a tile.
        /// </summary>
        public void GetTile()
        {
            tileCount--;

            tileCountText.text = tileCount.ToString() + " x";
        }

        /// <summary>
        /// Increases the amount of left tiles, when the player returns a tile.
        /// </summary>
        public void ReturnTile()
        {

            tileCount++;

            tileCountText.text = tileCount.ToString() + " x";
            General.GameBehavior.instance.CheckGridCollider();
        }

        /// <summary>
        /// Adds the grid field data to the list of used Fileds when a tile from this button is placed.
        /// </summary>
        /// <param name="gridField"></param>
        public void PlaceTile(GridField gridField)
        {
            transform.parent.GetComponent<TilePanel>().usedGridFields.Add(gridField);
        }

        /// <summary>
        /// Removes the grid filed data from the used filds when a tile returns
        /// </summary>
        public void ResetTile(GridField gridField)
        {
            transform.parent.GetComponent<TilePanel>().usedGridFields.Remove(gridField);
        }


    }
}

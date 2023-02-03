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
        [SerializeField] private TextMeshPro tileCountText;
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

        public void GetTile()
        {
            tileCount--;

            tileCountText.text = tileCount.ToString() + " x";
        }

        public void ReturnTile(bool deleteInManger = true)
        {

            tileCount++;

            tileCountText.text = tileCount.ToString() + " x";
            if(deleteInManger)
            {
                General.GameBehavior.instance.CheckGridCollider();
            }
        }

        public void PlaceTile(GridField gridField)
        {
            transform.parent.GetComponent<TilePanel>().usedGridFields.Add(gridField);
        }

        public void ResetTile(GridField gridField)
        {
            transform.parent.GetComponent<TilePanel>().usedGridFields.Remove(gridField);
        }


    }
}

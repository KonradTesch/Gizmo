using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rectangle.TileCreation;
using Rectangle.Level;

namespace Rectangle.UI
{
    public class TileButton : MonoBehaviour
    {
        [SerializeField] private TextMeshPro tileCountText;
        [SerializeField] private GameObject tilePrefab;

        [HideInInspector] public Sprite tileSprite;
        [HideInInspector]public int tileCount;
        [HideInInspector]public TileCreator.TileTypes tileType;



        private void OnEnable()
        {
            tileCountText.text = tileCount.ToString();

            for(int i = 0; i < tileCount; i++)
            {
                LevelTile newTile = Instantiate(tilePrefab, transform).GetComponent<LevelTile>();
                newTile.transform.localPosition = Vector3.zero;
                newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
                newTile.tileType = tileType;
                newTile.button = this;
            }
        }

        public void GetTile()
        {

            tileCount--;

            tileCountText.text = tileCount.ToString();
        }

        public void ReturnTile()
        {
            tileCount++;

            tileCountText.text = tileCount.ToString();

            General.GameBehavior.instance.CheckGridCollider();
        }
    }
}

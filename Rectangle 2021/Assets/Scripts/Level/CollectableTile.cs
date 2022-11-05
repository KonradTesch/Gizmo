using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.TileCreation;

namespace Rectangle.Level
{
    public class CollectableTile : MonoBehaviour
    {
        public TileCreator.TileTypes tileType;
        public Player.PlayerController.PlayerModes playerMode;
        public int count;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            General.GameBehavior.instance.TileInventoryChange(tileType, count);
            Destroy(gameObject);
        }

    }
}

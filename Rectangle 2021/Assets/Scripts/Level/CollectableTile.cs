using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.LevelCreation;
using Rectangle.General;

namespace Rectangle.Level
{
    public class CollectableTile : MonoBehaviour
    {
        public TileCreator.TileTypes tileType;
        public Player.PlayerController.PlayerModes playerMode;
        public int count;
        public TileBuilder tileBuilder;
        public Vector2 tilePosition;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameBehavior.instance.TileInventoryChange(new InventoryTile(playerMode, tileType), count);

            if(tileBuilder.collectedTiles.ContainsKey(tilePosition))
            {
                tileBuilder.collectedTiles[tilePosition].Add(new InventoryTile(playerMode, tileType));
            }
            else
            {
                tileBuilder.collectedTiles.Add(tilePosition, new List<InventoryTile>() { new InventoryTile(playerMode, tileType) });
            }

            Destroy(gameObject);
        }

    }
}

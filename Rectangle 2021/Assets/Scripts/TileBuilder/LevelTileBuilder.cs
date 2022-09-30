using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.TileBuilder
{
    public class LevelTileBuilder : MonoBehaviour
    {
        [Header("TileMaps")]
        public Tilemap groundTilemap;
        public Tilemap rampTilemap;
        public Tilemap nooksTilemap;
        public Vector2Int tileSize = new Vector2Int(32, 32);

        [Header("Tile Settings")]
        public string tileName;
        public TileTypes tileType;
        public Player.ModeController.PlayerModes playerMode;

        public enum TileTypes { undefined, AllSides, LeftAndRight, LeftAndDown, LeftAndUp, UpAndRight, DownAndRight, UpAndDown};

    }
}

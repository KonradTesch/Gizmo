using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.Level;

namespace Rectangle.TileCreation
{
    public class TileCreator : MonoBehaviour
    {
        [Header("TileMaps")]
        public Tilemap groundTilemap;
        public Tilemap rampTilemap;
        public Vector2Int tileSize = new Vector2Int(32, 32);

        [Header("Asset Folder")]
        public string saveFolderPath;

        [Header("Tile Settings")]
        public LevelBuilderSettings builderSettings;
        [Space()]
        public string tileName;
        public TileTypes tileType;
        public Player.PlayerController.PlayerModes playerMode;


        public enum TileTypes { undefined, AllSides, LeftAndRight, LeftAndDown, LeftAndUp, UpAndRight, DownAndRight, UpAndDown};



    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.TileBuilder;

namespace Rectangle.LevelEditor
{
    [CreateAssetMenu(fileName = "LevelEditorSettings", menuName = "Rectangle/Level Editor Settings")]
    public class LevelEditorSettings : ScriptableObject
    {
        [Header("Tiles")]
        public int tileSize = 8;
        public LevelTileData gridTiles;
        public TileBase borderTile;

        [Header("Level Backgrounds")]

        public Sprite backgroundImage;
        public Color rectangleColor;
        public Color spikeyColor;
        public Color bubbleColor;
        public Color littleColor;

    }
}

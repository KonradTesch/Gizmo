using Rectangle.TileCreater;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.Level
{
    [CreateAssetMenu(fileName = "LevelBuilderSettings", menuName = "Rectangle/Level Builder Settings")]
    public class LevelBuilderSettings : ScriptableObject
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

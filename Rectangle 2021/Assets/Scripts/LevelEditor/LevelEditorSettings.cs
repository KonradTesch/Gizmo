using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.LevelEditor
{
    [CreateAssetMenu(fileName = "LevelBuilderSettings", menuName = "Rectangle/Level Builder Settings")]
    public class LevelEditorSettings : ScriptableObject
    {
        public int tileSize = 8;

        public TileBase rightTopCorner;
        public TileBase leftTopCorner;
        public TileBase rightBottomCorner;
        public TileBase leftBottomCorner;

        public TileBase topBorder;
        public TileBase leftBorder;
        public TileBase bottomBorder;
        public TileBase rightBorder;

    }
}

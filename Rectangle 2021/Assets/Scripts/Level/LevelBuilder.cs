using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.Level
{
    public class LevelBuilder : MonoBehaviour
    {
        public Texture2D levelLayout;
        public LevelBuilderSettings builderSettings;
        public Tilemap gridTilemap;
        public Tilemap borderTilemap;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.Level;

namespace Rectangle.LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        public Texture2D levelLayout;
        public LevelEditorSettings builderSettings;
        public Tilemap gridTilemap;
        public Tilemap borderTilemap;
    }

}
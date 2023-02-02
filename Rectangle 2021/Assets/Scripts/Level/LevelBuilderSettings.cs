using System.Collections;
using System.Collections.Generic;
using Rectangle.LevelCreation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.Level
{
    [CreateAssetMenu(fileName = "LevelBuilderSettings", menuName = "Rectangle/Level Builder Settings")]
    public class LevelBuilderSettings : ScriptableObject
    {
        [Header("Tiles")]
        public Vector2Int tileSize;
        public LevelTileData gridTiles;
        public LevelTileData closedBorderTile;
        public LevelTileData openBorderTile;
        public TileBase borderTile;

        [Header("Level Backgrounds")]

        public LayerMask gridLayer;
        public LayerMask backgroundLayer;
        public Sprite backgroundImage;
        public Color rectangleColor;
        public Color spikeyColor;
        public Color bubbleColor;
        public Color littleColor;

        [Header("LevelTiles")]

        public GameObject levelTilePrefab;
        public GameObject anchorTilePrefab;
        public Sprite anchorTileSprite;
        public TileType[] tileTypes;

        [Header("Star")]

        public GameObject starPrefab;
        public Sprite starSprite;

        public Sprite GetTileTypeSprite(TileCreator.TileTypes tileType, Player.PlayerController.PlayerModes playerMode)
        {
            foreach(TileType type in tileTypes)
            {
                if(type.tileType == tileType)
                {
                    for(int i = 0; i < type.modeSprites.Count; i++)
                    {
                        if (type.modeSprites[i].playerMode == playerMode)
                        {
                            return type.modeSprites[i].tileSprite;
                        }
                    }
                }
            }

            Debug.LogWarning("The LevelBuildSettings haven't a tileype sprite for type " + tileType);
            return null;
        }

        public Color GetModeColor(Player.PlayerController.PlayerModes mode)
        {
            switch(mode)
            {
                case Player.PlayerController.PlayerModes.Rectangle:
                    return rectangleColor;
                case Player.PlayerController.PlayerModes.Bubble:
                    return bubbleColor;
                case Player.PlayerController.PlayerModes.Spikey:
                    return spikeyColor;
                case Player.PlayerController.PlayerModes.Little:
                    return littleColor;
            }

            return Color.white;
        }

    }

    [System.Serializable]
    public class TileType
    {
        public TileCreator.TileTypes tileType;
        public List<TileSprite> modeSprites;
    }

    [System.Serializable]
    public class TileSprite
    {
        public Player.PlayerController.PlayerModes playerMode;
        public Sprite tileSprite;
    }
}

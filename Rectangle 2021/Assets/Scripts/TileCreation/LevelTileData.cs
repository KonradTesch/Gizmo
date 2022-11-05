using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.TileCreation
{
    public class LevelTileData : ScriptableObject
    {
        public List<ChangeData> groundTileChanges;
        public List<ChangeData> platformTileChanges;
        public List<ChangeData> rampTileChanges;

        public Vector2[] collectablePositions;

        public Vector2Int tileSize;
        public TileCreator.TileTypes tileType;
        public Player.PlayerController.PlayerModes playerMode;
    }

    [System.Serializable]
    public class ChangeData
    {
        public Vector3Int position;
        public TileBase tile;
        [HideInInspector]public Matrix4x4 transform;
    }
}

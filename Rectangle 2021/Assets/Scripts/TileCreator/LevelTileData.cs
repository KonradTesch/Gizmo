using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.TileCreater
{
    public class LevelTileData : ScriptableObject
    {
        public List<ChangeData> groundTileChanges;
        public List<ChangeData> nookTileChanges;
        public List<ChangeData> rampTileChanges;

        public Vector2Int tileSize;
        public TileCreater.TileTypes tileType;
        public Player.ModeController.PlayerModes playerMode;
    }

    [System.Serializable]
    public class ChangeData
    {
        public Vector3Int position;
        public TileBase tile;
        [HideInInspector]public Matrix4x4 transform;
    }
}

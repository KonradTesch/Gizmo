using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.LevelBuilder
{
    public class LevelData : ScriptableObject
    {
        public List<AreaTiles> areaData;
        [HideInInspector]public Vector2 grid;
        [HideInInspector]public bool hasDeletedTiles = false;

        public void Init(List<List<Tile>> list)
        {
            int i = 0;
            areaData = new List<AreaTiles>();
            foreach(List<Tile> area in list)
            {
                areaData.Add(new AreaTiles());

                areaData[i].tiles = area;
                i++;
            }
        }

        public List<List<Tile>> GetAreaList()
        {
            List<List<Tile>> areas = new List<List<Tile>>();

            foreach(AreaTiles tileList in areaData)
            {
                areas.Add(tileList.tiles);
            }
            return areas;
        }

    }


    [System.Serializable]
    public class Tile
    {
        public Vector2 coordinates;
        [HideInInspector] public Rect rect;
        [HideInInspector] public bool active = true;

        public int tileIndex;
        public int areaIndex = -1;
        public Mode playerMode;
        [HideInInspector]public Color color = Color.grey;
        [HideInInspector]public string text;

        public enum Mode {Undefinde, Rectangle, Sphere, Spickey, Little}
    }

    [System.Serializable]
    public class AreaTiles
    {
        public List<Tile> tiles;
    }

}
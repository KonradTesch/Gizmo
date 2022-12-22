using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.Level;
using Rectangle.Player;

namespace Rectangle.LevelCreation
{
    [System.Serializable]
    public class LevelData : ScriptableObject
    {
        public LevelGrid gridData;
        public List<PlannedTile> plannedTiles;

        public PlannedTile GetTileByCoordinates(Vector2Int coordinates, AnchorTile anchor)
        {
            if(anchor == null)
            {
                for (int i = 0; i < plannedTiles.Count; i++)
                {
                    if (plannedTiles[i].coordinates == coordinates)
                    {
                        return plannedTiles[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < gridData.anchorTiles.Count; i++)
                {
                    if (gridData.anchorTiles[i] == anchor)
                    {
                        for (int n = 0; n < gridData.anchorTiles[i].collectableTiles.Count; n++)
                        {
                            if (gridData.anchorTiles[i].collectableTiles[n].coordinates == coordinates)
                            {
                                return gridData.anchorTiles[i].collectableTiles[n];
                            }
                        }
                    }
                }
            }
            return null;
        }

        public AnchorTile GetAnchorByCoordinates(Vector2Int coordinates)
        {
            if(gridData.anchorTiles == null)
            {
                gridData.anchorTiles = new();
            }

            for (int i = 0; i < gridData.anchorTiles.Count; i++)
            {
                if (gridData.anchorTiles[i].anchorCoordinates == coordinates)
                {
                    return gridData.anchorTiles[i];
                }
            }

            return null;
        }

        public void ChangePlannedTile(PlannedTile tile)
        {
            for(int i = 0; i < plannedTiles.Count; i++)
            {
                if (plannedTiles[i].coordinates == tile.coordinates)
                {
                    plannedTiles[i] = tile;
                    return;
                }
            }
            plannedTiles.Add(tile);
        }

        public void ChangeCollectableTile(AnchorTile anchor, PlannedTile tile)
        {
            for(int i = 0; i < gridData.anchorTiles.Count; i++)
            {
                if (gridData.anchorTiles[i] == anchor)
                {
                    for(int n = 0; n < gridData.anchorTiles[i].collectableTiles.Count; n++)
                    {
                        if (gridData.anchorTiles[i].collectableTiles[n].coordinates == tile.coordinates)
                        {
                            gridData.anchorTiles[i].collectableTiles[n] = tile;
                            return;
                        }
                    }
                    gridData.anchorTiles[i].collectableTiles.Add(tile);
                }
            }
        }

        public void RemoveTile(PlannedTile tile, AnchorTile anchor)
        {
            if(anchor == null)
            {
                if (plannedTiles.Contains(tile))
                {
                    plannedTiles.Remove(tile);
                }
            }
            else
            {
                for (int i = 0; i < gridData.anchorTiles.Count; i++)
                {
                    if (gridData.anchorTiles[i] == anchor && gridData.anchorTiles[i].collectableTiles.Contains(tile))
                    {
                        gridData.anchorTiles[i].collectableTiles.Remove(tile);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class LevelGrid
    {
        public Dictionary<Vector2Int, LevelSpot> grid;
        public List<AnchorTile> anchorTiles;
        public int height;
        public int width;
        public Vector2Int start;
        public Vector2Int end;
    }

    [System.Serializable]
    public class LevelSpot
    {
        public Vector2Int coordinates;
        [HideInInspector]public Vector2 worldPosition;
        [HideInInspector]public LevelTile placedTile;
        public bool anchor;
        public bool blocked;
        public bool star;

        public LevelSpot(Vector2Int coordinates, Vector2 worldPosition, bool blocked, bool anchor)
        {
            this.coordinates = coordinates;
            this.worldPosition = worldPosition;
            this.blocked = blocked;
            this.anchor = anchor;
        }

        public LevelSpot(Vector2Int coordinates, bool blocked, bool anchor)
        {
            this.coordinates = coordinates;
            this.blocked = blocked;
            this.anchor = anchor;
        }
    }

    [System.Serializable]
    public class PlannedTile
    {
        public Vector2Int coordinates;
        public PlayerController.PlayerModes playerMode;
        public TileCreator.TileTypes tileType;
        [HideInInspector]public AnchorTile anchor;
    }

    [System.Serializable]
    public class AnchorTile
    {
        public List<PlannedTile> collectableTiles = new();
        public Vector2Int anchorCoordinates;
    }
}

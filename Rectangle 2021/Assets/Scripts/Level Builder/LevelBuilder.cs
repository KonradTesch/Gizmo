using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rectangle.Level;

namespace Rectangle.LevelBuilder
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField] private LevelData levelData;
        [SerializeField] private LevelBuilderSettings builderSettings;
        [SerializeField] private Grid tileMapGrid;


        [SerializeField] private bool buildLevel = false;
    
        void Update()
        {
            if(buildLevel)
            {
                buildLevel = false;
                BuildLevel(levelData.areaData);
            }
        }

        private void BuildLevel(List<AreaTiles> areas)
        {
            GameObject gridTileMap = new GameObject("TileMap_Grid");
            gridTileMap.transform.parent = tileMapGrid.transform;
            gridTileMap.transform.localScale = Vector3.one;
            Tilemap tileMap = gridTileMap.AddComponent<Tilemap>();
            gridTileMap.AddComponent<TilemapRenderer>();

            GameObject tileObject = new GameObject();

            int tileCount = 0; ;

            foreach(AreaTiles area in areas)
            {
                tileCount += area.tiles.Count;
            }

            GameObject[] tiles = new GameObject[tileCount];

            // Instantiated tiles in chaotic order and saved them in the tiles array
            foreach (AreaTiles area in areas)
            {

                foreach (Tile tile in area.tiles)
                {
                    GameObject newTile = Instantiate(tileObject);

                    newTile.name = $"Tile {tile.tileIndex} ({tile.playerMode})";
                    newTile.transform.position = tile.coordinates * builderSettings.tileSize;
                    newTile.AddComponent<BoxCollider2D>().size = Vector2.one * builderSettings.tileSize;
                    newTile.AddComponent<IsGridUsed>();

                    tiles[tile.tileIndex] = newTile;

                    Destroy(newTile);

                    CreatedGridTiles(tile.coordinates, tileMap);
                }
            }

            GameObject level = new GameObject("Level");

            //Instantiates tiles in sorted order
            for(int i = 0; i < tileCount; i++)
            {
                GameObject newTile = Instantiate(tiles[i], level.transform);
                newTile.name = tiles[i].name;
                newTile.GetComponent<Collider2D>().enabled = true;
            }

            Destroy(tileObject);

        }

        private void CreatedGridTiles(Vector2 coordinates, Tilemap tileMap)
        {
            int size = Mathf.RoundToInt(builderSettings.tileSize / tileMapGrid.transform.localScale.x);
            Vector3Int offset = new Vector3Int(-size / 2, -size / 2, 0);

            tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size, coordinates.y * size, 0)) + offset, builderSettings.leftBottomCorner);
            tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size + size  - 1 , coordinates.y * size, 0)) + offset, builderSettings.rightBottomCorner);
            tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size, coordinates.y * size + size - 1, 0)) + offset, builderSettings.leftTopCorner);
            tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size + size - 1, coordinates.y * size + size - 1, 0)) + offset, builderSettings.rightTopCorner);

            for(int i = 1; i < size - 1; i++)
            {
                tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size, coordinates.y * size + i, 0)) + offset, builderSettings.leftBorder);
                tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size + i, coordinates.y * size, 0)) + offset, builderSettings.bottomBorder);
                tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size + size - 1, coordinates.y * size + i, 0)) + offset, builderSettings.rightBorder);
                tileMap.SetTile(Vector3Int.RoundToInt(new Vector3(coordinates.x * size + i, coordinates.y * size + size - 1, 0)) + offset, builderSettings.topBorder);

            }
        }
    }
}
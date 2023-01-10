using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rectangle.Level;
using Rectangle.LevelCreation;

namespace Rectangle.UI
{
    public class TilePanel : MonoBehaviour
    {
        [SerializeField] private GameObject tileButtonReference;

        [SerializeField] public List<GridField> usedGridFields;
        public void InitTileButtons(List<TileGroupData> tileGroups, float levelWidth)
        {
            SpriteRenderer rend = GetComponent<SpriteRenderer>();

            transform.position = new Vector3(levelWidth / 2, -rend.size.y / 2 - 3, 0);

            rend.size = new Vector2(levelWidth, rend.size.y);

            foreach(Transform child in transform)
            {
                if(child.gameObject != tileButtonReference)
                    Destroy(child.gameObject);
            }

            int i = 1;

            foreach (TileGroupData tileGroup in tileGroups)
            {
                if(tileGroup.tileCount > 0)
                {
                    TileButton newButton = Instantiate(tileButtonReference, transform).GetComponent<TileButton>();

                    newButton.transform.localPosition = new Vector3( (levelWidth / (tileGroups.Count + 1)) * i - levelWidth / 2 , 0, 0);

                    newButton.tileCount = tileGroup.tileCount;
                    newButton.tileType = tileGroup.tileType;
                    newButton.tileSprite = tileGroup.tileSprite;
                    newButton.playerMode = tileGroup.playerMode;
                    newButton.tileColor = tileGroup.tileColor;

                    SpriteRenderer tileRend = newButton.GetComponent<SpriteRenderer>();

                    tileRend.sprite = tileGroup.tileSprite;
                    tileRend.color = Color.grey;

                    newButton.gameObject.SetActive(true);

                    i++;
                }
            }

        }

        public void ResetUsedGrids(List<LevelTile> placedTiles)
        {
            if(placedTiles != null)
            {
                foreach(LevelTile tile in placedTiles)
                {
                    usedGridFields.Remove(tile.gridCollider);
                }
            }

            foreach (GridField gridCollider in usedGridFields)
            {
                gridCollider.isUsed = false;
                gridCollider.backgroundRend.color = Color.gray;
                gridCollider.GetComponent<BackgroundMode>().playerMode = Player.PlayerController.PlayerModes.None;
            }

            usedGridFields.Clear();
        }



    }
}

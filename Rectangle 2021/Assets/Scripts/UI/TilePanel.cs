using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rectangle.Level;
using Rectangle.TileCreation;

namespace Rectangle.UI
{
    public class TilePanel : MonoBehaviour
    {
        [SerializeField] private GameObject tileButtonReference;

        private TileBuilder tileBuilder;

        public void InitTileButtons(List<TileCreator.TileTypes> tileTypes, LevelBuilderSettings builderSettings)
        {
            tileBuilder = General.GameBehavior.instance.GetComponent<TileBuilder>();

            Dictionary<TileCreator.TileTypes, int> levelTiles = new();

            foreach(TileCreator.TileTypes type in tileTypes)
            {
                if(levelTiles.ContainsKey(type))
                {
                    levelTiles[type]++;
                }
                else
                {
                    levelTiles.Add(type, 1);
                }
            }

            int i = 1;
            float panelHeight = GetComponent<SpriteRenderer>().size.y;

            foreach (KeyValuePair<TileCreator.TileTypes, int> tileType in levelTiles)
            {
                TileButton newButton = Instantiate(tileButtonReference, transform).GetComponent<TileButton>();

                newButton.transform.localPosition = new Vector3(0, i * (panelHeight / (levelTiles.Count + 1)) - panelHeight / 2, 0);

                newButton.tileCount = tileType.Value;
                newButton.tileType = tileType.Key;
                newButton.tileSprite = builderSettings.GetTileTypeSprite(tileType.Key);
                newButton.playerMode = tileBuilder.GetPlayerMode(tileType.Key);
                newButton.tileColor = builderSettings.GetModeColor(newButton.playerMode);

                SpriteRenderer tileRend = newButton.GetComponent<SpriteRenderer>();

                tileRend.sprite = builderSettings.GetTileTypeSprite(tileType.Key);
                tileRend.color = Color.grey;

                newButton.gameObject.SetActive(true);

                i++;
            }

        }


    }
}

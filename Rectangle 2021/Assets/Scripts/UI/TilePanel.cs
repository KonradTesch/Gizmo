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

        public void InitTileButtons(List<TileGroupData> tileGroups)
        {
            foreach(Transform child in transform)
            {
                if(child.gameObject != tileButtonReference)
                    Destroy(child.gameObject);
            }

            int i = 1;
            float panelHeight = GetComponent<SpriteRenderer>().size.y;

            foreach (TileGroupData tileGroup in tileGroups)
            {
                if(tileGroup.tileCount > 0)
                {
                    TileButton newButton = Instantiate(tileButtonReference, transform).GetComponent<TileButton>();

                    newButton.transform.localPosition = new Vector3(0, i * (panelHeight / (tileGroups.Count + 1)) - panelHeight / 2, 0);

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


    }
}

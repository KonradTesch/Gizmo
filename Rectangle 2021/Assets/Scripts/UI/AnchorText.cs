using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Rectangle.Player;

namespace Rectangle.UI
{
    public class AnchorText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro rectangleNumber;
        [SerializeField] private TextMeshPro bubbleNuimber;
        [SerializeField] private TextMeshPro spikeyNumber;

        public void SetTileNumbers(List<PlayerController.PlayerModes> tileModes)
        {
            int rectCount = 0;
            int bubbleCount = 0;
            int spikeyCount = 0;

            foreach(PlayerController.PlayerModes mode in tileModes)
            {
                switch(mode)
                {
                    case PlayerController.PlayerModes.Rectangle:
                        rectCount++;
                        break;
                    case PlayerController.PlayerModes.Bubble:
                        bubbleCount++;
                        break;
                    case PlayerController.PlayerModes.Spikey:
                        spikeyCount++;
                        break;
                }
            }

            rectangleNumber.text = rectCount.ToString();
            bubbleNuimber.text = bubbleCount.ToString();
            spikeyNumber.text = spikeyCount.ToString();

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.GetComponent<PlayerBase>() != null)
            {
                collision.transform.parent.GetComponent<PlayerController>().anchor = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerBase>() != null)
            {
                collision.transform.parent.GetComponent<PlayerController>().anchor = false;
            }
        }
    }
}

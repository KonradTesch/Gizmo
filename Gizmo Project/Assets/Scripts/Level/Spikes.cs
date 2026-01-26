using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.Player;
using Rectangle.General;

namespace Rectangle
{
    /// <summary>
    /// The spikes script, that kills the player
    /// </summary>
    public class Spikes : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameBehavior.instance.gameOverPanel.SetActive(true);

                collision.gameObject.transform.parent.GetComponent<PlayerController>().playerActive = false;

                GameBehavior.onPlayerDeath();
            }
        }
    }
}

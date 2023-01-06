using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.Player;

namespace Rectangle
{
    public class Spikes : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                General.GameBehavior.instance.gameOverPanel.SetActive(true);
                collision.gameObject.transform.parent.GetComponent<PlayerController>().playerActive = false;
            }
        }
    }
}

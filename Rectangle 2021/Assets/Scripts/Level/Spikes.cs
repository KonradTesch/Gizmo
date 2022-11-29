using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle
{
    public class Spikes : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                General.GameBehavior.instance.gameOverPanel.SetActive(true);
            }
        }
    }
}

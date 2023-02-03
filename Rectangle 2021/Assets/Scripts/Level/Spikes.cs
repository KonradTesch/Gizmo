using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.Player;
using Rectangle.General;

namespace Rectangle
{
    public class Spikes : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameBehavior.instance.gameOverPanel.SetActive(true);
                //GameBehavior.instance.uiAudioSource.PlayOneShot(GameBehavior.instance.deathSound);
                collision.gameObject.transform.parent.GetComponent<PlayerController>().playerActive = false;

                GameBehavior.death();
            }
        }
    }
}

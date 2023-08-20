using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Tilemaps;

namespace Rectangle.Level
{
    /// <summary>
    /// Attaches the player to another object (e. g. a platform) so that he moves along with it.
    /// </summary>
    public class StickyPlayer : MonoBehaviour
    {
        /// <summary>
        /// The tilemap of the object
        /// </summary>
        private TilemapCollider2D tilemapCollider;
        /// <summary>
        /// If the object vanished, after the player touches it.
        /// </summary>
        private bool vanishing;
        /// <summary>
        /// The time in seconds after which the oject vanishes after being touched.
        /// </summary>
        private float vanishingTime;
        private void Start()
        {
            tilemapCollider = GetComponent<TilemapCollider2D>();
            if(GetComponent<WaypointFollower>() != null)
            {
                vanishing = GetComponent<WaypointFollower>().vanishing;
            }

            if(TryGetComponent<WaypointFollower>(out WaypointFollower platform))
            {
                vanishing = platform.vanishing;
                vanishingTime = platform.vanishingTime;
            }


        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.transform.CompareTag("Player") && collision.transform.position.y >= tilemapCollider.bounds.max.y && tilemapCollider.transform.position.x < tilemapCollider.bounds.max.x && collision.transform.position.x > tilemapCollider.bounds.min.x)
            {
                //Sets the player as a child of the object.
                collision.transform.parent.SetParent(transform);

                if (vanishing)
                {
                    StartCoroutine("Vanish", collision.gameObject);
                }

            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.transform.CompareTag("Player"))
            {
                collision.transform.parent.SetParent(null);
            }
        }

        /// <summary>
        /// DEstriys the object after the given delay.
        /// </summary>
        private IEnumerator Vanish(GameObject player)
        {
            yield return new WaitForSeconds(vanishingTime);
            player.transform.parent.SetParent(null);

            Destroy(gameObject);
        }
    }

}

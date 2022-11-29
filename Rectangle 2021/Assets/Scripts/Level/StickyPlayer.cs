using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.Level
{
    public class StickyPlayer : MonoBehaviour
    {
        private TilemapCollider2D tilemapCollider;
        private bool vanishing;
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

        private IEnumerator Vanish(GameObject player)
        {
            yield return new WaitForSeconds(vanishingTime);
            player.transform.parent.SetParent(null);

            Destroy(gameObject);
        }
    }

}

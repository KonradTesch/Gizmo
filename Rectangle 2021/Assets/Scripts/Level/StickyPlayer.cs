using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rectangle.Level
{
    public class StickyPlayer : MonoBehaviour
    {
        private TilemapCollider2D tilemapCollider;

        private void Start()
        {
            tilemapCollider = GetComponent<TilemapCollider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.transform.CompareTag("Player") && collision.transform.position.y >= tilemapCollider.bounds.max.y && tilemapCollider.transform.position.x < tilemapCollider.bounds.max.x && collision.transform.position.x > tilemapCollider.bounds.min.x)
            {
                collision.transform.parent.SetParent(transform);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.transform.CompareTag("Player"))
            {
                collision.transform.parent.SetParent(null);
            }
        }
    }

}

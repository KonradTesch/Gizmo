using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Level
{
    public class Star : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Collect Star Code here

            Destroy(this.gameObject);
        }
    }
}

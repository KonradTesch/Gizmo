using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.General;

namespace Rectangle.Level
{
    public class Star : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            SaveGameManager.instance.CollectStar(GameBehavior.instance.levelName);

            Destroy(this.gameObject);
        }
    }
}

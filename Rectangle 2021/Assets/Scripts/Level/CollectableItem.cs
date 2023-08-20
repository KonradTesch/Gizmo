using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rectangle.General;

namespace Rectangle.Level
{
    /// <summary>
    /// The script af an collectable item (nut).
    /// </summary>
    public class CollectableItem : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            SaveGameManager.Singleton.CollectItem(GameBehavior.instance.levelBuilder.levelData);
            GameBehavior.instance.uiAudioSource.PlayOneShot(GameBehavior.instance.nutCatchSound);

            GameBehavior.onCollectItem();

            Destroy(gameObject);
        }
    }
}

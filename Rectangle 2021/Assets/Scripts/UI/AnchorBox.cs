using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rectangle.Player;

namespace Rectangle.UI
{
    public class AnchorBox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.GetComponent<PlayerBase>() != null)
            {
                collision.transform.parent.GetComponent<PlayerController>().anchor = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerBase>() != null)
            {
                collision.transform.parent.GetComponent<PlayerController>().anchor = false;
            }
        }
    }
}

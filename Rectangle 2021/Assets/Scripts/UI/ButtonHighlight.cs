using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Rectangle.UI
{
    public class ButtonHighlight : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        /// <summary>
        /// The button object that should be highlighted.
        /// </summary>
        [Tooltip("The button object that should be highlighted.")]
        [SerializeField] private GameObject highlightObject;

        /// <summary>
        /// The size of the object, when highlighted.
        /// </summary>
        [Tooltip("The size of the object, when highlighted.")]
        [SerializeField] private float highlightSize;

        public void OnPointerEnter(PointerEventData eventData)
        {
            highlightObject.transform.localScale = Vector3.one * highlightSize;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            highlightObject.transform.localScale = Vector3.one;

        }

        public void OnSelect(BaseEventData eventData)
        {
            highlightObject.transform.localScale = Vector3.one * highlightSize;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            highlightObject.transform.localScale = Vector3.one;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class CancelButton : MonoBehaviour, IPointerUpHandler , IPointerDownHandler
    {
        private readonly ExitFireMode _exitFireMode = new ExitFireMode();
        public static bool _buttonPressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            _buttonPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _buttonPressed = false;
            if (eventData.pointerCurrentRaycast.gameObject == null) return;
            if(eventData.pointerCurrentRaycast.gameObject.transform.parent.Equals(transform))
                _exitFireMode.ExitFire();
        }
    }
}

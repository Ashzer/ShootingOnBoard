using UnityEngine;
using UnityEngine.EventSystems;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class PlayAgainButton : MonoBehaviour , IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            GameManager.State = GameState.Begin;
        }
    }
}

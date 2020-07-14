using UnityEngine;
using UnityEngine.EventSystems;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class CancelButton : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            if (FireButton._buttonPressed) return;
            switch (GameManager.State)
            {
                case GameState.BlueTeamFire:
                    GameManager.State = GameState.BlueTeamSelection;
                    break;
                case GameState.RedTeamFire:
                    GameManager.State = GameState.RedTeamSelection;
                    break;
            }
        }
    }
}

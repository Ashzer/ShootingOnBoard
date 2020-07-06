using UnityEngine;
using UnityEngine.EventSystems;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    public class CancelButton : MonoBehaviour, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            if (FireButton._buttonPressed) return;
            switch (GameManager.state)
            {
                case GameState.BlueTeamFire:
                    GameManager.state = GameState.BlueTeamSelection;
                    break;
                case GameState.RedTeamFire:
                    GameManager.state = GameState.RedTeamSelection;
                    break;
            }
        }
    }
}

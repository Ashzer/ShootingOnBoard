using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class MouseScreenRayProvider : MonoBehaviour, IRayProvider
    {
        public Ray CreateRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
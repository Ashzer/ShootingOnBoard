using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    public class MouseScreenRayProvider : MonoBehaviour, IRayProvider
    {
        public Ray CreateRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    class TouchScreenRayProvider : MonoBehaviour, IRayProvider
    {
        public Ray CreateRay()
        {
            return Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        }
    }
}
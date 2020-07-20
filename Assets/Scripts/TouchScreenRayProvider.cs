using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    class TouchScreenRayProvider : MonoBehaviour, IRayProvider
    {
        private CameraController _cameraController;
        private void Start()
        {
            _cameraController = CameraController.Instance;
        }
        public Ray CreateRay()
        {
            return _cameraController.GetCurrentCamera().ScreenPointToRay(Input.GetTouch(0).position);
        }
    }
}
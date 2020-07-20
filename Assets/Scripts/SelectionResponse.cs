using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts
{
    internal class SelectionResponse : MonoBehaviour, ISelectionResponse
    {
        //[SerializeField] public Material highlightMaterial = null;
        //private Material defaultMaterial;
        //private string _defaultTag;
        private Transform _fireCamTransform;
        //private Vector3 _originCamPosition;
        //private Quaternion _originCamRotation;
        private CameraController _cameraController;

        private void Start()
        {
            _cameraController = CameraController.Instance;
        }

        public void OnSelect(Transform selection)
        {
            var selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                //defaultMaterial = selectionRenderer.material;
                //selectionRenderer.material = this.highlightMaterial;
                //_defaultTag = selection.gameObject.tag;
                //selection.gameObject.tag = "Selected";
                selection.transform.rotation = Quaternion.Euler(0,0,0);

                _fireCamTransform = _cameraController.GetFireCamera().transform;
                //_originCamPosition = _fireCamTransform.position;
                //_originCamRotation = _fireCamTransform.rotation;
                _fireCamTransform.position = selection.GetComponent<Transform>().position +
                                            selection.GetComponent<Transform>().position.normalized * 3;

                _fireCamTransform.rotation = Quaternion.LookRotation(selection.position- _fireCamTransform.position);
                _fireCamTransform.position = new Vector3(_fireCamTransform.position.x, _fireCamTransform.position.y+2f, _fireCamTransform.position.z);
                _fireCamTransform.RotateAround(selection.position, Vector3.down, 10f);
                _cameraController.FireCameraOn();
            }
        }

        public void OnDeselect(Transform selection)
        {
            var selectionRenderer = selection.GetComponent<Renderer>();
            _cameraController.MainCameraOn();
            //_fireCamTransform = Camera.main.GetComponent<Transform>();
            //_fireCamTransform.position = _originCamPosition;
            //_fireCamTransform.rotation = _originCamRotation;
            //selectionRenderer.material = this.defaultMaterial;
            // selection.gameObject.tag = defaultTag;
        }
    }
}
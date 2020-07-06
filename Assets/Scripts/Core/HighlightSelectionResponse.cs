using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    internal class HighlightSelectionResponse : MonoBehaviour, ISelectionResponse
    {
        //[SerializeField] public Material highlightMaterial = null;
        //private Material defaultMaterial;
        //private string _defaultTag;
        private Transform _mainCamTransform;
        private Vector3 _originCamPosition;
        private Quaternion _originCamRotation;

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

                _mainCamTransform = Camera.main.GetComponent<Transform>();
                _originCamPosition = _mainCamTransform.position;
                _originCamRotation = _mainCamTransform.rotation;
                _mainCamTransform.position = selection.GetComponent<Transform>().position +
                                            selection.GetComponent<Transform>().position.normalized * 3;

                _mainCamTransform.rotation = Quaternion.LookRotation(selection.position- _mainCamTransform.position);
                _mainCamTransform.position = new Vector3(_mainCamTransform.position.x, _mainCamTransform.position.y+2f, _mainCamTransform.position.z);
                _mainCamTransform.RotateAround(selection.position, Vector3.down, 10f);
            }
        }

        public void OnDeselect(Transform selection)
        {
            var selectionRenderer = selection.GetComponent<Renderer>();
            _mainCamTransform = Camera.main.GetComponent<Transform>();
            _mainCamTransform.position = _originCamPosition;
            _mainCamTransform.rotation = _originCamRotation;
            //selectionRenderer.material = this.defaultMaterial;
            // selection.gameObject.tag = defaultTag;
        }
    }
}
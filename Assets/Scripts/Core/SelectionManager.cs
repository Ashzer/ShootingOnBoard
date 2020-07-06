using UnityEngine;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private ISelectionResponse _selectionResponse;
        private ISelector _selector;
        private IRayProvider _rayProvider;

        private Transform _currentSelection;
        
        private Transform _mainCamTransform;
        private Vector3 _mainCamOriginPosition;
        private Quaternion _mainCamOriginRotation;

        [SerializeField] private GameObject _aim;
        private LineRenderer _aimRenderer;
        private GameObject _aimContainer;
        private Vector3 _aimDirection;

        private void Awake()
        {
            _rayProvider = GetComponent<IRayProvider>();
            _selector = GetComponent<ISelector>();
            _selectionResponse = GetComponent<ISelectionResponse>();
            _mainCamTransform = Camera.main.GetComponent<Transform>();
            _mainCamOriginPosition = Camera.main.GetComponent<Transform>().position;
            _mainCamOriginRotation = Camera.main.GetComponent<Transform>().rotation;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_currentSelection != null)
                {
                    _selectionResponse.OnDeselect(_currentSelection);
                    Destroy(_aimContainer.gameObject);
                }

                if (GameManager.state == GameState.RedTeamSelection)
                {
                    _selector.Check(_rayProvider.CreateRay(), "Red Pieces");
                }else if (GameManager.state == GameState.BlueTeamSelection)
                {
                    _selector.Check(_rayProvider.CreateRay(), "Blue Pieces");
                }

                _currentSelection = _selector.GetSelection();

                _mainCamTransform.position = _mainCamOriginPosition;
                _mainCamTransform.rotation = _mainCamOriginRotation;
                if (_currentSelection != null)
                {
                    _selectionResponse.OnSelect(_currentSelection);
                    //mainCamTransform.position = _currentSelection.GetComponent<Transform>().position+new Vector3(0,5,-10);
                    _aimContainer = (GameObject) Instantiate(_aim, _currentSelection.transform);
                    _aimRenderer = (LineRenderer) _aimContainer.GetComponent(typeof(LineRenderer));
                }
            }

            if (_currentSelection != null)
            {
                _aimDirection = (_currentSelection.position - Camera.main.GetComponent<Transform>().position).normalized * 3;
                _aimRenderer.SetPosition(1, _aimDirection);
                _mainCamTransform.RotateAround(_currentSelection.transform.position, Vector3.up, 0.1f);
            }
        }


    }
} 
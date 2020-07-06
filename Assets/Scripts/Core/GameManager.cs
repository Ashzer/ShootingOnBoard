using System;
using System.Resources;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameState state;

        [SerializeField] private ISelectionResponse _selectionResponse;
        [SerializeField] private GameObject _aim;
        [SerializeField] private Button _fireButton;
        [SerializeField] private Button _canButton;
        [SerializeField] private Text _redScoreText;
        [SerializeField] private Text _blueScoreText;
        [SerializeField] private Text _resultText;
        [SerializeField] private Image _gauge;
        [SerializeField] private Image _gaugeBackground;

        private ISelector _selector;
        private IRayProvider _rayProvider;

        public static Transform _currentSelection;

        private Transform _mainCamTransform;
        
        
        private LineRenderer _aimRenderer;
        private GameObject _aimContainer;
        public static Vector3 _aimDirection;
        

        private static int _blueScore;
        private static int _redScore;

        

        private const float WaitingTime = 1.5f;
        private float _timer;

        private void Awake()
        {
            _rayProvider = GetComponent<IRayProvider>();
            _selector = GetComponent<ISelector>();
            _selectionResponse = GetComponent<ISelectionResponse>();
            _mainCamTransform = Camera.main.GetComponent<Transform>();
        }

        private void Start()
        {
            state = GameState.Begin;
            
            SetFireInterfaces(false);
            _resultText.gameObject.SetActive(false);
            _blueScore = 0;
            _redScore = 0;
            _timer = 0f;
            _redScoreText.text = _redScore.ToString();
            _blueScoreText.text = _blueScore.ToString();
            //Debug.Log($"Start {_redScore}");
            //Debug.Log($"Start {_blueScore}");
        }

        public Transform GetSelection()
        {
            return _currentSelection;
        }

        public Vector3 GetAimDirection()
        {
            return _aimDirection;
        }
        public void FirePiece()
        {
            SetFireInterfaces(false);
            switch (state)
            {
                case GameState.RedTeamFire:
                    state = GameState.PauseFromRedToBlue;
                    //state = GameState.BlueTeamSelection;
                    break;
                case GameState.BlueTeamFire:
                    state = GameState.PauseFromBlueToRed;
                    //state = GameState.RedTeamSelection;
                    break;
            }
            _selectionResponse.OnDeselect(_currentSelection);
            Destroy(_aimContainer.gameObject);
        }
        
        private void Update()
        {
            switch (state)
            {
                case GameState.Begin:
                    state = GameState.RedTeamSelection;
                    break;

                case GameState.RedTeamSelection:
                    SetFireInterfaces(false);

                    if (_currentSelection != null)
                    {
                        _selectionResponse.OnDeselect(_currentSelection);
                        Destroy(_aimContainer.gameObject);
                        //_mainCamTransform.position = _mainCamOriginPosition;
                        //_mainCamTransform.rotation = _mainCamOriginRotation;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        _selector.Check(_rayProvider.CreateRay(), "Red Pieces");
                        
                        _currentSelection = _selector.GetSelection();

                        //_mainCamTransform.position = _mainCamOriginPosition;
                        //_mainCamTransform.rotation = _mainCamOriginRotation;
                        if (_currentSelection != null)
                        {
                            _selectionResponse.OnSelect(_currentSelection);
                            _aimContainer = (GameObject)Instantiate(_aim, _currentSelection.transform);
                            _aimRenderer = (LineRenderer)_aimContainer.GetComponent(typeof(LineRenderer));
                            
                            state = GameState.RedTeamFire;
                        }
                    }
                    break;

                case GameState.RedTeamFire:
                    SetFireInterfaces(true);
                    if (_currentSelection != null)
                    {
                        //_aimDirection = (_currentSelection.position - Camera.main.GetComponent<Transform>().position).normalized * 3;
                        //_aimDirection.y = 0;
                        //_aimRenderer.SetPosition(1, _aimDirection);
                        //RotateCameraAroundTransform(_currentSelection);
                        //mainCamTransform.RotateAround(currentSelection.transform.position, Vector3.up, 0.1f);
                    }

                    break;
                case GameState.PauseFromRedToBlue:
                    _timer += Time.deltaTime;
                    if (_timer > WaitingTime)
                    {
                        state = GameState.BlueTeamSelection;
                        _timer = 0;
                    }
                    break;

                case GameState.BlueTeamSelection:
                    SetFireInterfaces(false);

                    if (_currentSelection != null)
                    {
                        _selectionResponse.OnDeselect(_currentSelection);
                        Destroy(_aimContainer.gameObject);
                        //_mainCamTransform.position = _mainCamOriginPosition;
                        //_mainCamTransform.rotation = _mainCamOriginRotation;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        _selector.Check(_rayProvider.CreateRay(), "Blue Pieces");

                        _currentSelection = _selector.GetSelection();

                        //_mainCamTransform.position = _mainCamOriginPosition;
                        //_mainCamTransform.rotation = _mainCamOriginRotation;

                        if (_currentSelection != null)
                        {
                            _selectionResponse.OnSelect(_currentSelection);
                            _aimContainer = (GameObject)Instantiate(_aim, _currentSelection.transform);
                            _aimRenderer = (LineRenderer)_aimContainer.GetComponent(typeof(LineRenderer));

                            state = GameState.BlueTeamFire;
                        }
                    }   
                    break;

                case GameState.BlueTeamFire:

                    SetFireInterfaces(true);

                    if (_currentSelection != null)
                    {
                        // _aimDirection = (_currentSelection.position - Camera.main.GetComponent<Transform>().position).normalized * 3;
                        // _aimDirection.y = 0;
                        // _aimRenderer.SetPosition(1, _aimDirection);
                        //RotateCameraAroundTransform(_currentSelection);
                        //mainCamTransform.RotateAround(currentSelection.transform.position, Vector3.up, 0.1f);
                    }
                    break;

                case GameState.PauseFromBlueToRed:
                    _timer += Time.deltaTime;
                    if (_timer > WaitingTime)
                    {
                        state = GameState.RedTeamSelection;
                        _timer = 0;
                    }
                    break;

                case GameState.RedTeamWon:
                    state = GameState.End;
                    _resultText.text = "Red team won!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Red team won!");
                    break;

                case GameState.BlueTeamWon:
                    state = GameState.End;
                    _resultText.text = "Blue team won!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Blue team won!");
                    break;

                case GameState.Draw:
                    state = GameState.End;
                    _resultText.text = "Draw!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Draw!");
                    break;

                case GameState.End:
                    Debug.Log("End!");
                    break;
                default:
                    break;
            }
        }

        private void LateUpdate()
        {
            if (state == GameState.BlueTeamFire || state == GameState.RedTeamFire)
            {
                _aimDirection = (_currentSelection.position - Camera.main.GetComponent<Transform>().position);
                _aimDirection.y = 0;
                _aimDirection = _aimDirection.normalized * 3;
                
                _aimRenderer.SetPosition(1, _aimDirection);
                //if (Input.GetMouseButton(0))
                if( Input.touchCount > 0 && !FireButton._buttonPressed)
                {
                    RotateCameraAroundTransform(_currentSelection);
                }
            }
            var redPieces = GameObject.FindGameObjectsWithTag("Red Pieces").Length;
            var bluePieces = GameObject.FindGameObjectsWithTag("Blue Pieces").Length;
            _redScoreText.text = redPieces.ToString();
            _blueScoreText.text = bluePieces.ToString();

            if (state != GameState.PauseFromBlueToRed && state != GameState.PauseFromRedToBlue)
            {
                if (redPieces == 0 && bluePieces == 0)
                {
                    state = GameState.Draw;
                }
                else if (redPieces == 0)
                {
                    state = GameState.BlueTeamWon;
                }
                else if (bluePieces == 0)
                {
                    state = GameState.RedTeamWon;
                }
            }
        }

        private void RotateCameraAroundTransform(Transform trans)
        {
            var touch = Input.GetTouch(0);
            _mainCamTransform.RotateAround(_currentSelection.position, Vector3.up, touch.deltaPosition.x / 2);
        }


        private void SetFireInterfaces(bool visibility)
        {
            _fireButton.gameObject.SetActive(visibility);
            _canButton.gameObject.SetActive(visibility);
            _gauge.gameObject.SetActive(visibility);
            _gaugeBackground.gameObject.SetActive(visibility);
        }
    }
}
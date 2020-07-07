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
        #region Variables

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
        [SerializeField] private Text _currentState;
        private ISelector _selector;
        private IRayProvider _rayProvider;

        private Transform _currentSelection;

        private Transform _mainCamTransform;
        
        private LineRenderer _aimRenderer;
        private GameObject _aimContainer;
        private Vector3 _aimDirection;
        
        private static int _blueScore;
        private static int _redScore;
        private GameObject[] _bluePiecesContainers;
        private GameObject[] _redPiecesContainers;

        private const float WaitingTime = 1.5f;
        private float _timer;

        private bool _stoppingCheck;
        #endregion

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
            _stoppingCheck = false;
            _redScore = GameObject.FindGameObjectsWithTag("Red Pieces").Length;
            _blueScore = GameObject.FindGameObjectsWithTag("Blue Pieces").Length;
        
            _redScoreText.text = _redScore.ToString();
            _blueScoreText.text = _blueScore.ToString();
        }


        public void FirePiece()
        {
            SetFireInterfaces(false);
            switch (state)
            {
                case GameState.RedTeamFire:
                    state = GameState.FromRedToBlue;
                    break;
                case GameState.BlueTeamFire:
                    state = GameState.FromBlueToRed;
                    break;
            }
            _selectionResponse.OnDeselect(_currentSelection);
            Destroy(_aimContainer.gameObject);
        }
        
        private void Update()
        {
            var hasEveryStopped = true;
            
            switch (state)
            {
                case GameState.Begin:
                    _currentState.text = "Begin state";
                    state = GameState.RedTeamSelection;
                    break;

                case GameState.RedTeamSelection:
                    _currentState.text = "RedTeam Selection";
                    SetFireInterfaces(false);

                    if (_currentSelection != null)
                    {
                        _selectionResponse.OnDeselect(_currentSelection);
                        Destroy(_aimContainer.gameObject);
                    }

                    if (Input.touchCount > 0)
                    {
                        _selector.Check(_rayProvider.CreateRay(), "Red Pieces");
                        
                        _currentSelection = _selector.GetSelection();
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
                    _currentState.text = "RedTeam Fire";
                    SetFireInterfaces(true);
                    break;

                case GameState.BlueTeamSelection:
                    _currentState.text = "BlueTeam Selection";
                    SetFireInterfaces(false);

                    if (_currentSelection != null)
                    {
                        _selectionResponse.OnDeselect(_currentSelection);
                        Destroy(_aimContainer.gameObject);
                    }

                    if (Input.touchCount > 0)
                    {
                        _selector.Check(_rayProvider.CreateRay(), "Blue Pieces");

                        _currentSelection = _selector.GetSelection();

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
                    _currentState.text = "BlueTeam Fire";
                    SetFireInterfaces(true);
                    break;

                case GameState.FromRedToBlue:
                    _currentState.text = "Red to Blue";

                    _bluePiecesContainers = GameObject.FindGameObjectsWithTag("Blue Pieces");
                    _redPiecesContainers = GameObject.FindGameObjectsWithTag("Red Pieces");

                    foreach (var piece in _bluePiecesContainers)
                    {
                        //Debug.Log($"{piece.tag} is Moving with {piece.GetComponent<Rigidbody>().velocity.magnitude}");
                        if (piece.GetComponent<Rigidbody>().velocity != new Vector3(0, 0, 0))
                        {
                            hasEveryStopped = false;
                        }
                    }

                    foreach (var piece in _redPiecesContainers)
                    {
                        //Debug.Log($"{piece.tag} is Moving with {piece.GetComponent<Rigidbody>().velocity.magnitude}");
                        if (piece.GetComponent<Rigidbody>().velocity != new Vector3(0, 0, 0))
                        {
                            hasEveryStopped = false;
                        }
                    }

                    if (!hasEveryStopped) _stoppingCheck = true;
                    if (_stoppingCheck == false) return;

                    //Debug.Log("from Red to Blue : Moving");
                    if (hasEveryStopped)
                    {
                        //Debug.Log("from Red to Blue : All Stopped");
                        _redScore = GameObject.FindGameObjectsWithTag("Red Pieces").Length;
                        _blueScore = GameObject.FindGameObjectsWithTag("Blue Pieces").Length;

                        GetFinishState();

                        _stoppingCheck = false;
                    }
                    //_redScore = redPieces.Length;
                    //_blueScore = bluePieces.Length;
                    break;

                case GameState.FromBlueToRed:
                    _currentState.text = "Blue to Red";

                    _bluePiecesContainers = GameObject.FindGameObjectsWithTag("Blue Pieces");
                    _redPiecesContainers = GameObject.FindGameObjectsWithTag("Red Pieces");

                    foreach (var piece in _bluePiecesContainers)
                    {
                        //Debug.Log($"{piece.tag} is Moving with {piece.GetComponent<Rigidbody>().velocity.magnitude}");
                        if (piece.GetComponent<Rigidbody>().velocity != new Vector3(0, 0, 0))
                        {
                            hasEveryStopped = false;
                            break;
                        }
                    }

                    foreach (var piece in _redPiecesContainers)
                    {
                        //Debug.Log($"{piece.tag} is Moving with {piece.GetComponent<Rigidbody>().velocity.magnitude}");
                        if (piece.GetComponent<Rigidbody>().velocity != new Vector3(0, 0, 0))
                        {
                            hasEveryStopped = false;
                            break;
                        }
                    }

                    if (!hasEveryStopped) _stoppingCheck = true;
                    if (_stoppingCheck == false) return;

                    //Debug.Log("from Blue to Red : Moving");
                    if (hasEveryStopped)
                    {
                        //Debug.Log("from Blue to Red : All Stopped");
                        _redScore = GameObject.FindGameObjectsWithTag("Red Pieces").Length;
                        _blueScore = GameObject.FindGameObjectsWithTag("Blue Pieces").Length;

                        GetFinishState();

                        _stoppingCheck = false;
                    }

                    //_redScore = redPieces.Length;
                    //_blueScore = bluePieces.Length;
                    break;

                case GameState.RedTeamWon:
                    _currentState.text = "Red won";
                    state = GameState.EndOfGame;
                    _resultText.text = "Red team won!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Red team won!");
                    break;

                case GameState.BlueTeamWon:
                    _currentState.text = "Blue won";
                    state = GameState.EndOfGame;
                    _resultText.text = "Blue team won!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Blue team won!");
                    break;

                case GameState.Draw:
                    _currentState.text = "Draw";
                    state = GameState.EndOfGame;
                    _resultText.text = "Draw!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Draw!");
                    break;

                case GameState.EndOfGame:
                    _currentState.text = "EndOfGame";
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

                if (_aimRenderer != null) _aimRenderer.SetPosition(1, _aimDirection);

                if (Input.touchCount > 0 && !FireButton._buttonPressed)
                {
                    RotateCameraAroundTransform(_currentSelection);
                }
            }

            _redScore = GameObject.FindGameObjectsWithTag("Red Pieces").Length;
            _blueScore = GameObject.FindGameObjectsWithTag("Blue Pieces").Length;

            _redScoreText.text = _redScore.ToString();
            _blueScoreText.text = _blueScore.ToString();
        }

        private static void GetFinishState()
        {
            if (_redScore == 0 && _blueScore == 0)
            {
                state = GameState.Draw;
            }
            else if (_redScore == 0)
            {
                state = GameState.BlueTeamWon;
            }
            else if (_blueScore == 0)
            {
                state = GameState.RedTeamWon;
            }
            else
            {
                state = (state == GameState.FromBlueToRed)
                    ? GameState.RedTeamSelection
                    : GameState.BlueTeamSelection;
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

        public Transform GetSelection()
        {
            return _currentSelection;
        }

        public Vector3 GetAimDirection()
        {
            return _aimDirection;
        }

    }
}
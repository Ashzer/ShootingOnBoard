using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        #region Variables

        private GameMode _gameMode;
        private CameraController _cameraController;
        public static GameState State;

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
        [SerializeField] private GameObject _redContainer;
        [SerializeField] private GameObject _blueContainer;

        private ISelector _selector;
        private IRayProvider _rayProvider;
        private IGameInitializer _gameInitializer;

        private Transform _currentSelection;
        private Transform _currentTarget;
        

        private LineRenderer _aimRenderer;
        private GameObject _aimContainer;
        private Vector3 _aimDirection;

        private static int _blueScore;
        private static int _redScore;
        private GameObject[] _bluePiecesContainers;
        private GameObject[] _redPiecesContainers;
        private string _redPieceTag;
        private string _bluePieceTag;
        

        private bool _stoppingCheck;
        private readonly ExitFireMode _exitFireMode = new ExitFireMode();
        private GameObject _board;

        #endregion

        private void Awake()
        {
            _rayProvider = GetComponent<IRayProvider>();
            _selector = GetComponent<ISelector>();
            _selectionResponse = GetComponent<ISelectionResponse>();
            _gameInitializer = GetComponent<IGameInitializer>();
        }

        private void Start()
        {
            _gameMode = GameModeController.Instance.GetGameMode();
            _cameraController = CameraController.Instance;

            State = GameState.Begin;

            SetFireInterfaces(false);
            _resultText.gameObject.SetActive(false);
            _blueScore = 0;
            _redScore = 0;
            _stoppingCheck = false;
            _redPieceTag = "Red Pieces";
            _bluePieceTag = "Blue Pieces";

            UpdateScore();

            _redScoreText.text = _redScore.ToString();
            _blueScoreText.text = _blueScore.ToString();

            _board = GameObject.FindGameObjectWithTag("Board");
            Debug.Log($"Current Game Mode = {_gameMode}");
        }

        public void FirePiece()
        {
            SetFireInterfaces(false);
            switch (State)
            {
                case GameState.RedTeamFire:
                    State = GameState.FromRedToBlue;
                    break;
                case GameState.BlueTeamFire:
                    State = GameState.FromBlueToRed;
                    break;
            }
            _selectionResponse.OnDeselect(_currentSelection);
            _currentTarget = null;
            Destroy(_aimContainer.gameObject);
        }


        private void Update()
        {
            var hasEveryStopped = true;

            switch (State)
            {
                case GameState.Begin:
                    _currentState.text = "Begin state";
                    _resultText.gameObject.SetActive(false);
                    _gameInitializer.InitializeGame();
                    State = GameState.RedTeamSelection;
                    break;

                //Red team states
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
                        _selector.Check(_rayProvider.CreateRay(), _redPieceTag);

                        _currentSelection = _selector.GetSelection();
                        if (_currentSelection != null)
                        {
                            _selectionResponse.OnSelect(_currentSelection);
                            _aimContainer = (GameObject)Instantiate(_aim, _currentSelection.transform);
                            _aimRenderer = (LineRenderer)_aimContainer.GetComponent(typeof(LineRenderer));

                            switch (State)
                            {
                                case GameState.RedTeamSelection:
                                    State = GameState.RedTeamFire;
                                    break;
                                case GameState.BlueTeamSelection:
                                    State = GameState.BlueTeamFire;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }


                    //PieceSelectionByPlayer(_redPieceTag);
                    break;

                case GameState.RedTeamFire:
                    _currentState.text = "RedTeam Fire";
                    SetFireInterfaces(true);
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        if(Input.GetKeyDown(KeyCode.Escape)) _exitFireMode.ExitFire();
                    }

                    if (Input.touchCount > 0)
                    {
                        var touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {

                            //if (touch.phase == TouchPhase.Moved) return;
                            _selector.Check(_rayProvider.CreateRay(), _redPieceTag);

                            if (_selector.GetSelection() != null)
                            {
                                Debug.Log($"{_selector.GetSelection()}");
                                _selectionResponse.OnDeselect(_currentSelection);
                                Destroy(_aimContainer.gameObject);
                                _currentSelection = _selector.GetSelection();
                                _selectionResponse.OnSelect(_currentSelection);

                                if (_aimContainer != null)
                                {
                                    Destroy(_aimContainer.gameObject);
                                }

                                _aimContainer = (GameObject) Instantiate(_aim, _currentSelection.transform);
                                _aimRenderer = (LineRenderer) _aimContainer.GetComponent(typeof(LineRenderer));
                            }
                            
                        }
                    }

                    //PieceSelectionByPlayer(_redPieceTag);
                    break;
                
                case GameState.FromRedToBlue:
                    _currentState.text = "Red to Blue";

                    hasEveryStopped = IsAllStopped();

                    if (!hasEveryStopped) _stoppingCheck = true;
                    if (_stoppingCheck == false) return;

                    if (hasEveryStopped)
                    {
                        UpdateScore();
                        GetFinishState();
                        _stoppingCheck = false;
                    }
                    break;

                //Blue team states
                case GameState.BlueTeamSelection:
                    _currentState.text = "BlueTeam Selection";
                    SetFireInterfaces(false);
                    switch (_gameMode)
                    {
                        case GameMode.Vs2P:
                            if (_currentSelection != null)
                            {
                                _selectionResponse.OnDeselect(_currentSelection);
                                Destroy(_aimContainer.gameObject);
                            }
                            PieceSelectionByPlayer(_bluePieceTag);
                            break;
                        case GameMode.VsCom:
                            PieceSelectionByBot();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;

                case GameState.BlueTeamFire:
                    _currentState.text = "BlueTeam Fire";
                    switch (_gameMode)
                    {
                        case GameMode.Vs2P:
                            SetFireInterfaces(true);
                            if (Application.platform == RuntimePlatform.Android)
                            {
                                if (Input.GetKeyDown(KeyCode.Escape)) _exitFireMode.ExitFire();
                            }
                            break;
                        case GameMode.VsCom:
                            var direction = (_currentTarget.position - _currentSelection.position).normalized;
                            direction.y = 0f;
                            _currentSelection.GetComponent<Rigidbody>()
                                .AddRelativeForce(direction*90f, ForceMode.VelocityChange );
                            FirePiece();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;

                case GameState.FromBlueToRed:
                    _currentState.text = "Blue to Red";

                    hasEveryStopped = IsAllStopped();

                    if (!hasEveryStopped) _stoppingCheck = true;
                    if (_stoppingCheck == false) return;

                    if (hasEveryStopped)
                    {
                        UpdateScore();
                        GetFinishState();
                        _stoppingCheck = false;
                    }
                    break;

                //End game states
                case GameState.RedTeamWon:
                    _currentState.text = "Red won";
                    State = GameState.EndOfGame;
                    _resultText.text = "Red team won!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Red team won!");
                    break;

                case GameState.BlueTeamWon:
                    _currentState.text = "Blue won";
                    State = GameState.EndOfGame;
                    _resultText.text = "Blue team won!";
                    _resultText.gameObject.SetActive(true);
                    Debug.Log("Blue team won!");
                    break;

                case GameState.Draw:
                    _currentState.text = "Draw";
                    State = GameState.EndOfGame;
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
            if (State == GameState.BlueTeamFire || State == GameState.RedTeamFire)
            {
                AimControl();
                if (Input.touchCount > 0 && !FireButton._buttonPressed && !CancelButton._buttonPressed)
                    _cameraController.FireCameraHandling(_currentSelection);
            }

            _cameraController.MainCameraHandling();
            UpdateScore();
            _redScoreText.text = _redScore.ToString();
            _blueScoreText.text = _blueScore.ToString();
        }

        private void AimControl()
        {
            _aimDirection = _currentSelection.position - _cameraController.GetCurrentCamera().transform.position;
            _aimDirection.y = 0;
            _aimDirection = _aimDirection.normalized * 3;

            if (_aimRenderer != null) _aimRenderer.SetPosition(1, _aimDirection);
        }

        #region Getter
        public Transform GetSelection()
        {
            return _currentSelection;
        }

        public Vector3 GetAimDirection()
        {
            return _aimDirection;
        }
        #endregion


        private void UpdateScore()
        {
            _redScore = GameObject.FindGameObjectsWithTag(_redPieceTag).Length;
            _blueScore = GameObject.FindGameObjectsWithTag(_bluePieceTag).Length;
        }

        private bool IsAllStopped()
        {
            _bluePiecesContainers = GameObject.FindGameObjectsWithTag("Blue Pieces");
            _redPiecesContainers = GameObject.FindGameObjectsWithTag("Red Pieces");
            foreach (var piece in _bluePiecesContainers)
            {
                var pieceRb = piece.GetComponent<Rigidbody>();
                if (pieceRb.velocity.magnitude > 0f) return false;
                else pieceRb.angularVelocity = new Vector3(0,0,0);
            }

            foreach (var piece in _redPiecesContainers)
            {
                var pieceRb = piece.GetComponent<Rigidbody>();
                if (pieceRb.velocity.magnitude > 0f) return false;
                else pieceRb.angularVelocity = new Vector3(0, 0, 0);
            }

            return true;
        }

        private void PieceSelectionByPlayer(string selectableTag)
        {
            if (Input.touchCount > 0)
            {
                _selector.Check(_rayProvider.CreateRay(), selectableTag);

                _currentSelection = _selector.GetSelection();
                if (_currentSelection != null)
                {
                    _selectionResponse.OnSelect(_currentSelection);
                    _aimContainer = (GameObject)Instantiate(_aim, _currentSelection.transform);
                    _aimRenderer = (LineRenderer)_aimContainer.GetComponent(typeof(LineRenderer));

                    switch (State)
                    {
                        case GameState.RedTeamSelection:
                            State = GameState.RedTeamFire;
                            break;
                        case GameState.BlueTeamSelection:
                            State = GameState.BlueTeamFire;
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        private void PieceSelectionByBot()
        {
            if (!IsAllStopped()) return;

            var minDistance = float.PositiveInfinity;
            if (_currentSelection != null)
            {
                _selectionResponse.OnDeselect(_currentSelection);
                Destroy(_aimContainer.gameObject);
            }

            foreach (var selection in _blueContainer.GetComponentsInChildren<Transform>())
            {
                if (selection.name == _blueContainer.name) continue;
                foreach (var target in _redContainer.GetComponentsInChildren<Transform>())
                {
                    if (target.name == _redContainer.name) continue;

                    var distance = Vector3.Distance(
                        new Vector3(selection.position.x, 0f, selection.position.z),  
                        new Vector3(target.position.x, 0f , target.position.z));

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        _currentSelection = selection;
                        _currentTarget = target;
                    }
                }
            }

            State = GameState.BlueTeamFire;
        }

        private static void GetFinishState()
        {
            if (_redScore == 0 && _blueScore == 0)
                State = GameState.Draw;
            else if (_redScore == 0)
                State = GameState.BlueTeamWon;
            else if (_blueScore == 0)
                State = GameState.RedTeamWon;
            else
                State = State == GameState.FromBlueToRed
                    ? GameState.RedTeamSelection
                    : GameState.BlueTeamSelection;
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
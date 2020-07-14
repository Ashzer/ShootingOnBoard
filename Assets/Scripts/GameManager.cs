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
        [SerializeField] private GameObject _redPiece;
        [SerializeField] private GameObject _bluePiece;

        private ISelector _selector;
        private IRayProvider _rayProvider;

        private Transform _currentSelection;
        private Transform _currentTarget;

        private Transform _mainCamTransform;

        private LineRenderer _aimRenderer;
        private GameObject _aimContainer;
        private Vector3 _aimDirection;

        private static int _blueScore;
        private static int _redScore;
        private GameObject[] _bluePiecesContainers;
        private GameObject[] _redPiecesContainers;
        private string _redPieceTag;
        private string _bluePieceTag;

        private const float WaitingTime = 1.5f;

        private bool _stoppingCheck;

        private GameObject _board;
        #endregion

        private void Awake()
        {
            _rayProvider = GetComponent<IRayProvider>();
            _selector = GetComponent<ISelector>();
            _selectionResponse = GetComponent<ISelectionResponse>();
            _mainCamTransform = Camera.main.GetComponent<Transform>();
            _gameMode = GameModeController.Instance.GetGameMode();
        }

        private void Start()
        {
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
                    InitializeGame();
                    State = GameState.RedTeamSelection;
                    break;

                //Red team states
                case GameState.RedTeamSelection:
                    _currentState.text = "RedTeam Selection";
                    SetFireInterfaces(false);
                    PieceSelectionByPlayer(_redPieceTag);
                    break;

                case GameState.RedTeamFire:
                    _currentState.text = "RedTeam Fire";
                    SetFireInterfaces(true);
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
                if (Input.touchCount > 0 && !FireButton._buttonPressed) RotateCameraAroundTransform(_currentSelection);
            }

            UpdateScore();
            _redScoreText.text = _redScore.ToString();
            _blueScoreText.text = _blueScore.ToString();
        }

        private void AimControl()
        {
            _aimDirection = _currentSelection.position - Camera.main.GetComponent<Transform>().position;
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
            if (_currentSelection != null)
            {
                _selectionResponse.OnDeselect(_currentSelection);
                Destroy(_aimContainer.gameObject);
            }

            if (Input.touchCount > 0)
            {
                _selector.Check(_rayProvider.CreateRay(), selectableTag);

                _currentSelection = _selector.GetSelection();
                if (_currentSelection != null)
                {
                    _selectionResponse.OnSelect(_currentSelection);
                    _aimContainer = (GameObject)Instantiate(_aim, _currentSelection.transform);
                    _aimRenderer = (LineRenderer)_aimContainer.GetComponent(typeof(LineRenderer));

                    State = (State == GameState.RedTeamSelection) ? GameState.RedTeamFire : GameState.BlueTeamFire;
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

        private void InitializeGame()
        {
            var piecesInitLocation = new int[10, 2] { { 2, 4 }, { 8, 4 }, { 12, 4 }, { 18, 4 }, { 2, 8 }, { 8, 8 }, { 12, 8 }, { 18, 8 }, { 5, 6 }, { 15, 6 } };

            foreach (var child in _blueContainer.GetComponentsInChildren<Transform>())
            {
                if (child.name == _blueContainer.name) continue;
                Destroy(child.gameObject);
            }

            foreach (var child in _redContainer.GetComponentsInChildren<Transform>())
            {
                if (child.name == _redContainer.name) continue;
                Destroy(child.gameObject);
            }

            for (var i = 0; i < piecesInitLocation.GetLength(0); i++)
            {
                Instantiate(_redPiece, _redContainer.transform.position + new Vector3(piecesInitLocation[i, 0], 0, -piecesInitLocation[i, 1]), Quaternion.identity, _redContainer.transform);
                Instantiate(_bluePiece, _blueContainer.transform.position + new Vector3(piecesInitLocation[i, 0], 0, piecesInitLocation[i, 1]), Quaternion.identity, _blueContainer.transform);
            }
        }
    }
}
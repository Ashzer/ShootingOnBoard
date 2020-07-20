using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class FireButton : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
    {
        public static bool _buttonPressed;
        private float _pressedPower;
        private GameManager _gameManager;

        [SerializeField] private Image _gauge;

        private bool _isIncreasing;

        private const float MaxPower = 3f;
        private const float MinPower = 0f;

        // Start is called before the first frame update
        private void Start()
        {
            _buttonPressed = false;
            _isIncreasing = true;
            _pressedPower = MinPower;
            _gauge.rectTransform.localScale = new Vector3(1f,0f,1f);
            _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_buttonPressed)
            {
                if (_isIncreasing)
                {
                    _pressedPower += Time.deltaTime;
                    _pressedPower *= 1.06f;
                    if (_pressedPower > MaxPower)
                    {
                        _isIncreasing = false;
                        _pressedPower = MaxPower;
                    }
                }
                else
                {
                    _pressedPower -= Time.deltaTime;
                    _pressedPower /= 1.06f;
                    if (_pressedPower <= MinPower)
                    {
                        _isIncreasing = true;
                        _pressedPower = MinPower;
                    }
                }
            }
        }

        private void LateUpdate()
        {
            _gauge.rectTransform.localScale = new Vector3(1f, _pressedPower / MaxPower, 1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _buttonPressed = true;
            _pressedPower = MinPower;
            _gauge.rectTransform.localScale = new Vector3(1f, 0f, 1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var currentSelection = _gameManager.GetSelection();
            if (currentSelection != null)
            {
                var aimDirection = _gameManager.GetAimDirection();
                currentSelection.GetComponent<Rigidbody>().AddRelativeForce(aimDirection * 30f * (_pressedPower / MaxPower), ForceMode.VelocityChange);
            }
            
            _gauge.rectTransform.localScale = new Vector3(1f, 0f, 1f);
            _pressedPower = MinPower;
            _buttonPressed = false;
            _isIncreasing = true;
            _gameManager.FirePiece();
        }
    }
}

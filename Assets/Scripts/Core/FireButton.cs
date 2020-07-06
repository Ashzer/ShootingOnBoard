using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevJJ.Entertainment.Assets.Scripts.Core
{
    public class FireButton : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
    {
        public static bool _buttonPressed;
        private float _timePressed;
        private GameManager _gameManager;

        [SerializeField] private Image _gauge;

        private bool _isIncreasing;
        // Start is called before the first frame update
        private void Start()
        {
            _buttonPressed = false;
            _isIncreasing = true;
            _gauge.rectTransform.localScale = new Vector3(1f,0f,1f);

           
        }

        // Update is called once per frame
        private void Update()
        {
            if (_buttonPressed)
            {
                if (_isIncreasing)
                {
                    _timePressed += Time.deltaTime;
                    if (_timePressed > 2) _isIncreasing = false;
                }
                else
                {
                    _timePressed -= Time.deltaTime;
                    if (_timePressed < 0) _isIncreasing = true;
                }

                _gauge.rectTransform.localScale = new Vector3(1f, _timePressed/2f, 1f);

                //Debug.Log(_timePressed);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"Button pressed {GameManager.state}");
            _buttonPressed = true;
            _timePressed = 0;
            _gauge.rectTransform.localScale = new Vector3(1f, 0f, 1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log($"Button has pressed for {_timePressed}");
            
            

            var currentSelection = GameManager._currentSelection;
            if (currentSelection != null)
            {
                var aimDirection = GameManager._aimDirection;
                currentSelection.GetComponent<Rigidbody>().AddRelativeForce(aimDirection * 30f * (_timePressed / 2f), ForceMode.VelocityChange);
                //currentSelection.GetComponent<Rigidbody>().AddRelativeForce(aimDirection * 30f, ForceMode.VelocityChange);
            }
            _gauge.rectTransform.localScale = new Vector3(1f, 0f, 1f);
            _timePressed = 0;
            _buttonPressed = false;
        }
    }
}

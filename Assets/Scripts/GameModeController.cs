using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DevJJ.Entertainment.Assets.Scripts
{
    public class GameModeController : MonoBehaviour
    {
        [SerializeField] private Button _1PVs2PButton;
        [SerializeField] private Button _1PVsComButton;

        private static GameMode _gameMode;
        public static GameModeController Instance { get; set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        private void Start()
        {
            _1PVs2PButton.onClick.AddListener(Vs2pPressed);
            _1PVsComButton.onClick.AddListener(VsComPressed);
        }

        private void Vs2pPressed()
        {
            _gameMode = GameMode.Vs2P;
            SceneManager.LoadScene("Main Game");
        }

        private void VsComPressed()
        {
            _gameMode = GameMode.VsCom;
            SceneManager.LoadScene("Main Game");
        }

        public GameMode GetGameMode()
        {
            return _gameMode;
        }
    }
}
